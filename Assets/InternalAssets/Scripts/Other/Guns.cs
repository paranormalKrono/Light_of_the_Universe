using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour
{
    [SerializeField] private float shootTime = 0.55f;
    [SerializeField] private bool isTearShooting = false;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private bool isReverseForce = true;

    public float ShootTime { get => shootTime; set { shootTime = value; } }

    public int gunsCount { get; private set; }

    private bool isShoot;
    private bool isLockMove;

    private int tearShootGunNum;


    private Vector3 vector3Zero = Vector3.zero;

    private Vector3 shootReverseForce;
    private Vector3 v3_2;


    public delegate void ShootDelegate(Vector3 velocity, Transform target, out Vector3 shootReverseForce);

    private List<ShootDelegate> shootDelegates = new List<ShootDelegate>();


    private void Awake()
    {
        IGun[] guns = GetComponentsInChildren<IGun>();
        Collider[] ChildrenColliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < guns.Length; ++i)
        {
            ShootDelegate shootDelegate;
            guns[i].Initialise(ChildrenColliders, rigidbody.constraints, out shootDelegate);
            shootDelegates.Add(shootDelegate);
        }
        gunsCount = shootDelegates.Count;
    }


    public delegate void ShootEvent(Transform Target);
    public delegate IEnumerator IShootEvent(Transform Target);

    internal ShootEvent GetShootEvent() => Shoot;
    internal IShootEvent GetIShootEvent() => IShoot;

    private void Shoot(Transform Target)
    {
        if (!isShoot)
        {
            StartCoroutine(IShoot(Target));
        }
    }
    private IEnumerator IShoot(Transform Target)
    {
        isShoot = true;
        if (isTearShooting)
        {
            shootDelegates[tearShootGunNum](rigidbody.velocity, Target, out shootReverseForce);
            if (!isLockMove && isReverseForce)
            {
                rigidbody.AddForce(shootReverseForce, ForceMode.Impulse);
            }
            tearShootGunNum += 1;
            if (tearShootGunNum == gunsCount)
            {
                tearShootGunNum = 0;
            }
            yield return new WaitForSeconds(ShootTime / gunsCount);
        }
        else
        {
            shootReverseForce = vector3Zero;
            for (int i = 0; i < gunsCount; ++i)
            {
                shootDelegates[i](rigidbody.velocity, Target, out v3_2);
                shootReverseForce += v3_2;
            }
            if (!isLockMove && isReverseForce)
            {
                rigidbody.AddForce(shootReverseForce, ForceMode.Impulse);
            }
            yield return new WaitForSeconds(ShootTime);
        }
        isShoot = false;
    }



    internal void SetLockMove(bool t)
    {
        isLockMove = t;
    }
}
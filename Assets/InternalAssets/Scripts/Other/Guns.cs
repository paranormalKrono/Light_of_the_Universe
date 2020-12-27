using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour
{
    [SerializeField] private float shootTime = 0.55f;
    [SerializeField] private bool isTearShooting = false;
    [SerializeField] private Rigidbody rigidbody;

    public float ShootTime { get => shootTime; set { shootTime = value; } }
    public float MaxShootSpeed { get; private set; }

    private bool isShoot;
    private bool isLockMove;

    private int tearShootGunNum;


    private Vector3 vector3Zero = Vector3.zero;

    private Vector3 shootReverseForce;
    private Vector3 v3_2;


    public delegate void ShootEvent();
    public delegate void ShootDelegate(RigidbodyConstraints rigidbodyConstraints, Vector3 velocity, out Vector3 shootReverseForce);

    private List<ShootDelegate> shootDelegates = new List<ShootDelegate>();



    internal void Initialize(out ShootEvent shoot) => shoot = Shoot;

    private void Shoot()
    {
        if (!isShoot)
        {
            StartCoroutine(IShoot());
        }
    }

    private IEnumerator IShoot()
    {
        isShoot = true;
        if (isTearShooting)
        {
            shootDelegates[tearShootGunNum](rigidbody.constraints, rigidbody.velocity, out shootReverseForce);
            if (!isLockMove)
            {
                rigidbody.AddForce(shootReverseForce, ForceMode.Impulse);
            }
            tearShootGunNum += 1;
            if (tearShootGunNum == shootDelegates.Count)
            {
                tearShootGunNum = 0;
            }
            yield return new WaitForSeconds(ShootTime / shootDelegates.Count);
        }
        else
        {
            shootReverseForce = vector3Zero;
            foreach (ShootDelegate shootD in shootDelegates)
            {
                shootD(rigidbody.constraints, rigidbody.velocity, out v3_2);
                shootReverseForce += v3_2;
            }
            if (!isLockMove)
            {
                rigidbody.AddForce(shootReverseForce, ForceMode.Impulse);
            }
            yield return new WaitForSeconds(ShootTime);
        }
        isShoot = false;
    }


    internal void InitializeGun(ShootDelegate shoot, float maxShootSpeed, out Collider[] ChildrenColliders)
    {
        if (MaxShootSpeed < maxShootSpeed)
        {
            MaxShootSpeed = maxShootSpeed;
        }
        shootDelegates.Add(shoot);
        ChildrenColliders = GetComponentsInChildren<Collider>();
    }

    internal void SetLockMove(bool t)
    {
        isLockMove = t;
    }
}
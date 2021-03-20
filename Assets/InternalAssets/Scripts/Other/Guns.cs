using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour
{
    [SerializeField] private float shootTime = 0.55f;
    [SerializeField] private bool isTearShooting = false;
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private bool isReverseForce = true;

    private GunRocket[] gunRockets;

    public float ShootTime { get => shootTime; set { shootTime = value; } }

    public int gunsCount { get; private set; }

    private bool isShoot;
    private bool isLockMove;

    private int tearShootGunNum;


    private Vector3 vector3Zero = Vector3.zero;

    private Vector3 shootReverseForce;
    private Vector3 v3_2;


    public delegate void ShootDelegate(Vector3 velocity, out Vector3 shootReverseForce);

    private List<ShootDelegate> shootDelegates = new List<ShootDelegate>();


    private void Awake()
    {
        gunRockets = GetComponentsInChildren<GunRocket>();
        IGun[] guns = GetComponentsInChildren<IGun>();
        Collider[] ChildrenColliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < guns.Length; ++i)
        {
            ShootDelegate shootDelegate;
            guns[i].Initialise(ChildrenColliders, Rigidbody.constraints, out shootDelegate);
            shootDelegates.Add(shootDelegate);
        }
        gunsCount = shootDelegates.Count;
    }


    public delegate void ShootEvent();
    public delegate IEnumerator IShootEvent();

    internal ShootEvent GetShootEvent() => Shoot;
    internal IShootEvent GetIShootEvent() => IShoot;

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
            shootDelegates[tearShootGunNum](Rigidbody.velocity, out shootReverseForce);
            if (!isLockMove && isReverseForce)
            {
                Rigidbody.AddForce(shootReverseForce, ForceMode.Impulse);
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
                shootDelegates[i](Rigidbody.velocity, out v3_2);
                shootReverseForce += v3_2;
            }
            if (!isLockMove && isReverseForce)
            {
                Rigidbody.AddForce(shootReverseForce, ForceMode.Impulse);
            }
            yield return new WaitForSeconds(ShootTime);
        }
        isShoot = false;
    }

    public float GetGunsMaxShootSpeed()
    {
        GunBullet[] gunBullets = GetComponentsInChildren<GunBullet>();
        float max = 0;
        float d;
        for (int i = 0; i < gunBullets.Length; ++i)
        {
            d = gunBullets[i].shootSpeed();
            if (max < d)
            {
                max = d;
            }
        }
        return max;
    }
    public void SetTarget(Transform Target)
    {
        for (int i = 0; i < gunRockets.Length; ++i)
        {
            gunRockets[i].SetTarget(Target);
        }
    }

    internal void SetLockMove(bool t)
    {
        isLockMove = t;
    }
}
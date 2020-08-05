using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour
{
    [SerializeField] private float shootTime = 0.55f;
    [SerializeField] private bool isTearShooting = false;

    public float ShootTime { get => shootTime; set { shootTime = value; } }
    public float MaxShootSpeed { get; private set; }

    private bool isShoot;
    private bool isLockMove;

    private int TearShootGunID;

    private Collider[] colliders;
    private Rigidbody rb;

    private Vector3 vector3Zero = Vector3.zero;

    private Vector3 shootReverseForce;
    private Vector3 v3_2;


    public delegate void ShootEvent();
    public delegate void ShootDelegate(RigidbodyConstraints rigidbodyConstraints, Vector3 velocity, out Vector3 shootReverseForce);
    private List<ShootDelegate> shootDelegates = new List<ShootDelegate>();


    private void Awake() => colliders = GetComponentsInChildren<Collider>();

    internal void Initialize(Rigidbody rigidbody, out ShootEvent shoot)
    {
        rb = rigidbody;
        shoot = Shoot;
    }


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
            shootDelegates[TearShootGunID](rb.constraints, rb.velocity, out shootReverseForce);
            if (!isLockMove)
            {
                rb.AddForce(shootReverseForce, ForceMode.Impulse);
            }
            TearShootGunID += 1;
            if (TearShootGunID == shootDelegates.Count)
            {
                TearShootGunID = 0;
            }
            yield return new WaitForSeconds(ShootTime / shootDelegates.Count);
        }
        else
        {
            shootReverseForce = vector3Zero;
            foreach (ShootDelegate shootD in shootDelegates)
            {
                shootD(rb.constraints, rb.velocity, out v3_2);
                shootReverseForce += v3_2;
            }
            if (!isLockMove)
            {
                rb.AddForce(shootReverseForce, ForceMode.Impulse);
            }
            yield return new WaitForSeconds(ShootTime);
        }
        isShoot = false;
    }


    internal void InitializeGun(ShootDelegate shoot, float maxShootSpeed, out Collider[] colliders)
    {
        if (MaxShootSpeed < maxShootSpeed)
        {
            MaxShootSpeed = maxShootSpeed;
        }
        shootDelegates.Add(shoot);
        colliders = this.colliders;
    }

    internal void SetLockMove(bool t)
    {
        isLockMove = t;
    }
}
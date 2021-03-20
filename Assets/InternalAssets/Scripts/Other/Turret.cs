using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private float gunsRotateSpeed = 2;
    [SerializeField] private float mainRotateSpeed = 15;
    [SerializeField] private Guns guns;
    [SerializeField] private Transform MainTr;
    [SerializeField] private Transform GunsTr;
    [SerializeField] private Transform AimTr;

    private Guns.IShootEvent IShoot;
    private int gunsCount;

    private bool isAttack;
    private bool isAim;

    private Transform[] Targets;
    private Transform target;

    private void Start()
    {
        IShoot = guns.GetIShootEvent();
        gunsCount = guns.gunsCount;
    }

    private Vector3 LookDirection;
    private void FixedUpdate()
    {
        if (isAim)
        {
            FindBetterTarget();
            if (target == null)
            {
                StopAim();
            }
            LookDirection = (target.position - MainTr.position).normalized;
            LookDirection.y = 0;
            MainTr.rotation = Quaternion.RotateTowards(MainTr.rotation, Quaternion.LookRotation(LookDirection, Vector3.up), mainRotateSpeed * Time.fixedDeltaTime);
        }
    }

    public void StartAim(Transform[] Targets)
    {
        this.Targets = Targets;
        StartCoroutine(AimGuns());
        isAim = true;
    }

    private IEnumerator AimGuns()
    {
        while (Quaternion.Angle(AimTr.rotation, GunsTr.rotation) > 0.1f)
        {
            GunsTr.rotation = Quaternion.RotateTowards(GunsTr.rotation, AimTr.rotation, gunsRotateSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void StopAim()
    {
        StopAllCoroutines();
        isAim = false;
    }



    public void Attack()
    {
        if (!isAttack)
        {
            StartCoroutine(IAttack());
        }
    }

    private IEnumerator IAttack()
    {
        isAttack = true;
        for (int i = 0; i < gunsCount; ++i)
        {
            yield return new WaitForSeconds(Random.Range(0, 0.2f));
            guns.SetTarget(target);
            yield return IShoot();
        }
        isAttack = false;
    }

    private float minDistance;
    private float dis;
    private void FindBetterTarget()
    {
        minDistance = float.MaxValue;
        for (int i = 0; i < Targets.Length; ++i)
        {
            if (Targets[i] == null)
            {
                continue;
            }
            dis = Vector3.Distance(Targets[i].position, transform.position);
            if (minDistance > dis)
            {
                target = Targets[i];
                minDistance = dis;
            }
        }
    }
}

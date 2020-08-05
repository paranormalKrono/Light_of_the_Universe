using System.Collections;
using UnityEngine;

public class Starship_AI : MonoBehaviour
{
    [SerializeField] private float minDistance = 12;
    [SerializeField] private float attackDistance = 18;
    [SerializeField] private float findDistance = 30;
    [SerializeField] private float lostDistance = 65;
    [SerializeField] private float rotateForce = 18;
    [SerializeField] private float rotateSpeedMax = 18;
    [SerializeField] private float moveForce = 4000;
    [SerializeField] private float moveFriction = 4;
    [SerializeField] private float maxMoveSpeed = 20;
    [SerializeField] private float sideMoveTime = 1.5f;

    [SerializeField] private float attackDistanceThr = 2;
    [SerializeField] private float minDistanceThr = 2;
    [SerializeField] private float sideMoveTimeThr = 1;

    [SerializeField] private bool isSideMove = true;
    [SerializeField] private bool isControlLock = false;
    [SerializeField] private bool isAttackPlayer = true;
    [SerializeField] private bool isHuntPlayer = true;

    [SerializeField] private Transform RotPointTr;

    private Guns.ShootEvent Shoot;

    private Transform playerTr;
    private Rigidbody rb;
    private Rigidbody starshipRb;
    private Vector3 moveDirection;
    private Vector3 forward = Vector3.forward;
    private Vector3 localUp = Vector3.up;
    private Vector3 vector3Zero = Vector3.zero;
    private bool isFindPlayer;
    private bool isPlayerDead;

    private Vector3 v3;
    private float t;
    private float newSideMoveTime;
    private bool isT;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        starshipRb = RotPointTr.GetComponent<Rigidbody>();
        GetComponent<Guns>().Initialize(rb, out Shoot);
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        playerTr.GetComponent<Health>().DeathEvent += PlayerDead;
        localUp = transform.TransformDirection(localUp);
        minDistance += Random.Range(-minDistanceThr, minDistanceThr);
        attackDistance += Random.Range(-attackDistanceThr, attackDistanceThr);
        newSideMoveTime += sideMoveTime + Random.Range(-sideMoveTimeThr, sideMoveTimeThr);
        isT = Random.Range(-1, 2) > 0;
        if (isT)
        {
            t = newSideMoveTime;
        }
    }

    private void Update()
    {
        if (!isControlLock && !isPlayerDead && isHuntPlayer)
        {
            if (!isFindPlayer)
            {
                MoveFriction();
                isFindPlayer = Vector3.Distance(playerTr.position, transform.position) < findDistance;
            }
            else
            {
                LookAt(playerTr.position);

                if (Vector3.Distance(playerTr.position, transform.position) > attackDistance)
                {
                    MoveToPlayer(forward);
                }
                else
                {
                    if (isAttackPlayer)
                    {
                        Shoot();
                    }
                    if (Vector3.Distance(playerTr.position, transform.position) > minDistance)
                    {
                        MoveFriction();
                    }
                    else
                    {
                        MoveToPlayer(-forward);
                    }
                }
                isFindPlayer = Vector3.Distance(playerTr.position, transform.position) < lostDistance;
            }
        }
    }
    private void FixedUpdate()
    {
        if (!isControlLock)
        {
            if (isHuntPlayer)
            {
                if (isFindPlayer)
                {
                    if (isSideMove)
                    {
                        SideMove();
                    }
                }
            }
        }
    }

    private void PlayerDead()
    {
        isPlayerDead = true;
    }

    private float LookAt(Vector3 V3)
    {
        float ang = Vector3.Angle((V3 - transform.position).normalized, RotPointTr.forward);
        Vector3 rotVect = -Vector3.Cross((V3 - transform.position).normalized, RotPointTr.forward).normalized;
        rotVect.y = 0;
        rotVect.z = 0;

        if (ang < 5)
        {
            starshipRb.AddTorque(Time.fixedDeltaTime * rotateForce * ang * rotVect / 10, ForceMode.Acceleration);
            starshipRb.angularVelocity = Vector3.ClampMagnitude(starshipRb.angularVelocity, rotateSpeedMax / 10);
        }
        else
        {
            starshipRb.AddTorque(Time.fixedDeltaTime * rotateForce * ang * rotVect, ForceMode.Acceleration);
            starshipRb.angularVelocity = Vector3.ClampMagnitude(starshipRb.angularVelocity, rotateSpeedMax);
        }
        return ang;
    }

    private void MoveToPlayer(Vector3 moveDirection)
    {
        moveDirection = RotPointTr.TransformDirection(moveDirection);
        rb.AddForce(moveDirection * moveForce * Time.deltaTime, ForceMode.Impulse);
        v3 = rb.velocity;
        v3 = Vector3.ClampMagnitude(v3, maxMoveSpeed);
        rb.velocity = v3;
    }
    private void MoveFriction()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, vector3Zero, Time.deltaTime * moveFriction);
    }

    private void SideMove()
    {
        v3 = vector3Zero;
        if (!isT)
        {
            t += Time.fixedDeltaTime;
            v3 += localUp / 1.5f;
            if (t > newSideMoveTime)
            {
                newSideMoveTime = sideMoveTime + Random.Range(-sideMoveTimeThr, sideMoveTimeThr);
                isT = true;
            }
        }
        else
        {
            t -= Time.fixedDeltaTime;
            v3 -= localUp / 1.5f;
            if (t < 0)
            {
                isT = false;
            }
        }
        moveDirection = v3;
        moveDirection = RotPointTr.TransformDirection(moveDirection);
        rb.AddForce(moveDirection * moveForce * Time.fixedDeltaTime, ForceMode.Impulse);
    }

    internal void SetControlLock(bool t)
    {
        isControlLock = t;
        rb.velocity = vector3Zero;
        starshipRb.angularVelocity = vector3Zero;
    }
    internal void SetPlayerHunt(bool t)
    {
        isHuntPlayer = t;
    }
}
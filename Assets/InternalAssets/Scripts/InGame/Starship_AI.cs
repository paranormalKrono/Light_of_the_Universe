using UnityEngine;

public class Starship_AI : MonoBehaviour
{
    [SerializeField] private float minDistance = 12;
    [SerializeField] private float attackDistance = 18;
    [SerializeField] private float findDistance = 30;
    [SerializeField] private float lostDistance = 65;
    [SerializeField] private float sideMoveTime = 1.5f;

    [SerializeField] private float attackDistanceThr = 2;
    [SerializeField] private float minDistanceThr = 2;
    [SerializeField] private float sideMoveTimeThr = 1;

    [SerializeField] private bool isSideMove = true;
    [SerializeField] private bool isControlLock = false;
    [SerializeField] private bool isAttackPlayer = true;
    [SerializeField] private bool isHuntPlayer = true;

    [SerializeField] private Starship_Engine Engine;
    [SerializeField] private Starship_RotationEngine RotationEngine;

    private Guns.ShootEvent Shoot;

    private Transform playerTr;
    private Vector3 forward = Vector3.forward;
    private Vector3 side = Vector3.right;
    private Vector3 vector3Zero = Vector3.zero;
    private bool isFindPlayer;
    private bool isPlayerDead;

    private Vector3 v3;
    private float t;
    private float newSideMoveTime;
    private bool isT;


    private void Awake()
    {

        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        playerTr.GetComponent<Health>().OnDeath += PlayerDead;

        GetComponent<Guns>().Initialize(out Shoot);

        minDistance += Random.Range(-minDistanceThr, minDistanceThr);
        attackDistance += Random.Range(-attackDistanceThr, attackDistanceThr);
        newSideMoveTime += sideMoveTime + Random.Range(-sideMoveTimeThr, sideMoveTimeThr);
        isT = Random.Range(-1, 2) > 0;
        if (isT)
        {
            t = newSideMoveTime;
        }
    }

    private void FixedUpdate()
    {
        if (!isControlLock && isHuntPlayer && !isPlayerDead)
        {
            if (!isFindPlayer)
            {
                SlowDown();
                isFindPlayer = DistanceToPlayer() < findDistance;
            }
            else
            {
                if (isSideMove)
                {
                    SideMove();
                }

                LookAt(playerTr.position);

                if (DistanceToPlayer() > attackDistance)
                {
                    Move(forward);
                }
                else
                {
                    if (isAttackPlayer)
                    {
                        Shoot();
                    }
                    if (DistanceToPlayer() > minDistance)
                    {
                        SlowDown();
                    }
                    else
                    {
                        Move(-forward);
                    }
                }
                isFindPlayer = DistanceToPlayer() < lostDistance;
            }
        }
    }

    private void SideMove()
    {
        v3 = vector3Zero;
        if (!isT)
        {
            t += Time.fixedDeltaTime;
            v3 += side * Random.Range(0.6f, 1.2f);
            if (t > newSideMoveTime)
            {
                newSideMoveTime = sideMoveTime + Random.Range(-sideMoveTimeThr, sideMoveTimeThr);
                isT = true;
            }
        }
        else
        {
            t -= Time.fixedDeltaTime;
            v3 -= side * Random.Range(0.6f, 1.2f);
            if (t < 0)
            {
                isT = false;
            }
        }
        Move(v3);
    }

    private float DistanceToPlayer() => Vector3.Distance(playerTr.position, transform.position);

    private void LookAt(Vector3 Target) => RotationEngine.RotateToTarget(Target);

    private void Move(Vector3 moveDirection) => Engine.Move(moveDirection);

    private void SlowDown() => Engine.SlowDown();

    private void PlayerDead() => isPlayerDead = true;

    internal void SetControlLock(bool t)
    {
        isControlLock = t;
        Engine.SetLockMove(t);
        RotationEngine.SetLockRotate(t);
    }
    internal void SetPlayerHunt(bool t) => isHuntPlayer = t;
}
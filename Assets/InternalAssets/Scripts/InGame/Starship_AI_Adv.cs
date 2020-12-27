using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Guns), typeof(Rigidbody))]
public class Starship_AI_Adv : MonoBehaviour
{
    [SerializeField] private Mastery mastery;
    [SerializeField] private float minDistance = 9;
    [SerializeField] private float attackDistance = 11;
    [SerializeField] private float findDistance = 26;
    [SerializeField] private float lostDistance = 40;
    [SerializeField] private float leavePointDistance = 4;
    [SerializeField] private float moveAngle = 15; // Угол, выше которого снижается мощность двигателя
    [SerializeField] private float shootAngle = 70;
    [SerializeField] private float FearTime = 1.5f;
    [SerializeField] private float MoveToPointVelocityConsideration = 0.5f;
    [SerializeField] private float MinDistanceToNavMeshCorner = 0.5f;

    [SerializeField] private bool isControlLock = false;
    [SerializeField] private bool isAttack = true;
    [SerializeField] private bool isFollowTarget = true;

    [SerializeField] private Transform RotPointTr;

    [SerializeField] private Starship_Engine Engine;
    [SerializeField] private Starship_RotationEngine RotationEngine;

    [SerializeField] private NavMeshPath NavMeshPath;

    private Guns.ShootEvent Shoot;

    private Rigidbody rb; // Основное
    private Rigidbody EnemyRb;

    private Transform EnemyTarget;
    private Transform PreviousTarget;
    private Transform CurrentTarget;
    private Transform NextTarget;
    private Vector3 EnemyTargetV3; // Не менять на transform, ссылка может пропасть

    public delegate void EventTransformHandler(Transform Tr);
    public event EventTransformHandler DeathEvent;

    private bool isFindTarget;
    private bool isFear;
    private bool isMoveToPoint;
    private bool isTargetMissed;

    private float MasteryAimingKoef = 0;

    private float FearTimeNow;
    private float distanceToEnemy;
    private float maxShootSpeed;

    private Vector3 MoveTargetV3;
    private Vector3 localUp = Vector3.up;

    private RaycastHit enemyRaycastHitInfo;

    private void Awake()
    {
        MoveToPointVelocityConsideration *= Random.Range(0.9f, 1.1f);
        switch (mastery)
        {
            case Mastery.Noob:
                minDistance *= Random.Range(1, 1.5f);
                attackDistance *= Random.Range(1, 1.5f);
                findDistance *= Random.Range(0.5f, 1f);
                lostDistance *= Random.Range(0.5f, 1f);
                leavePointDistance *= Random.Range(0.5f, 1f);
                moveAngle *= Random.Range(0.5f, 1f);
                shootAngle *= Random.Range(1f, 1.6f);
                FearTime *= Random.Range(1f, 1.6f);
                MasteryAimingKoef = Random.Range(2f, 3f);
                break;
            case Mastery.Normal:
                minDistance *= Random.Range(1, 1.25f);
                attackDistance *= Random.Range(1, 1.25f);
                findDistance *= Random.Range(0.75f, 1f);
                lostDistance *= Random.Range(0.75f, 1f);
                leavePointDistance *= Random.Range(0.75f, 1f);
                moveAngle *= Random.Range(0.75f, 1f);
                shootAngle *= Random.Range(1f, 1.4f);
                FearTime *= Random.Range(1f, 1.3f);
                MasteryAimingKoef = Random.Range(1.5f, 2.5f);
                break;

            case Mastery.Pro:
                minDistance *= Random.Range(0.9f, 1.2f);
                attackDistance *= Random.Range(0.9f, 1.2f);
                findDistance *= Random.Range(0.8f, 1.1f);
                lostDistance *= Random.Range(0.8f, 1.1f);
                leavePointDistance *= Random.Range(0.9f, 1.1f);
                moveAngle *= Random.Range(0.9f, 1.2f);
                shootAngle *= Random.Range(0.9f, 1.2f);
                FearTime *= Random.Range(0.8f, 1.2f);
                MasteryAimingKoef = Random.Range(0.75f, 1f);
                break;
            case Mastery.Elite:
                minDistance *= Random.Range(0.95f, 1.1f);
                attackDistance *= Random.Range(0.95f, 1.1f);
                findDistance *= Random.Range(0.95f, 1.3f);
                lostDistance *= Random.Range(0.95f, 1.3f);
                leavePointDistance *= Random.Range(0.9f, 1.2f);
                moveAngle *= Random.Range(0.95f, 1.3f);
                shootAngle *= Random.Range(0.9f, 1.1f);
                FearTime *= Random.Range(0.9f, 1.1f);
                MasteryAimingKoef = Random.Range(0.5f, 0.7f);
                break;
        }

        NavMeshPath = new NavMeshPath();

        rb = GetComponent<Rigidbody>();
        Starship starship = GetComponent<Starship>();
        starship.SetEnemyTarget = SetEnemyTarget;
        starship.SetFollowTarget = SetFollowTarget;
        starship.SetLockControl = SetLockControl;
        starship.SetAttack = SetAttack;

        localUp = transform.TransformDirection(localUp);

        SetLockControl(true);
        isMoveToPoint = true;

    }

    private void Start()
    {
        Guns guns = GetComponent<Guns>();
        guns.Initialize(out Shoot);
        maxShootSpeed = guns.MaxShootSpeed;
    }

    private void FixedUpdate()
    {
        if (!isControlLock) // Если управление не отключено
        {
            if (!isFindTarget) // Если цель не найдена
            {
                if (isMoveToPoint) // Если можно двигаться к точке
                {
                    MoveToPoint(); // Двигаемся к точке
                }
                else
                {
                    SlowDown();
                }
                if (isFollowTarget && !isTargetMissed) // Если ИИ ищет цель
                {
                    if (EnemyTarget != null)
                    {
                        isFindTarget = Vector3.Distance(EnemyTarget.position, transform.position) < findDistance; // Проверяем видит ли ИИ противника
                    }
                    else
                    {
                        isTargetMissed = true;
                    }
                }
            }
            else
            {
                if (EnemyTarget != null)
                {
                    EnemyTargetV3 = EnemyTarget.position;

                    // ИИ видит цель
                    distanceToEnemy = Vector3.Distance(transform.position, EnemyTargetV3); // Получаем дистанцию до цели

                    if (!isFear) // Если не испугался
                    {
                        UpdatePath(EnemyTargetV3);
                        if (Physics.SphereCast(transform.position, 1, (EnemyTargetV3 - transform.position).normalized, out enemyRaycastHitInfo) && enemyRaycastHitInfo.transform.parent == EnemyTarget)
                        {
                            LookAt(EnemyTargetV3 + EnemyRb.velocity * (Vector3.Distance(transform.position, EnemyTargetV3 + EnemyRb.velocity) / maxShootSpeed) * (1 + Random.Range(-MasteryAimingKoef, MasteryAimingKoef))); // Поворачиваем корабль в сторону цели
                            
                            if (distanceToEnemy < attackDistance)
                            {
                                SlowDown();
                            }
                            else
                            {
                                Move(MoveDirectionConsideringAngle(1, AngleTo(MoveTargetV3)));
                            }

                            if (isAttack && AngleTo(EnemyTargetV3) < shootAngle) // Если ИИ может атаковать, и угол до цели меньше угла прицеливания
                            {
                                Shoot(); // Стреляем
                            }
                        }
                        else
                        {
                            LookAt(MoveTargetV3); // Поворачиваем корабль в сторону нужного движения
                            Move(MoveDirectionConsideringAngle(1, AngleTo(MoveTargetV3)));
                        }

                        if (distanceToEnemy < minDistance) // Если застрял или дистанция до цели меньше минимльной дистанции
                        {
                            FindClosestPoint(); // Находим ближайшую точку
                            isFear = true; // Испуг
                        }
                    }
                    else
                    {
                        // Отсчёт страха
                        FearTimeNow += Time.fixedDeltaTime;
                        if (FearTimeNow >= FearTime)
                        {
                            isFear = false;
                            FearTimeNow = 0;
                        }
                        MoveToPoint(); // Двигаемся к точке
                    }
                    isFindTarget = distanceToEnemy < lostDistance; // Проверяем видит ли ИИ противника
                    if (!isFindTarget) // Если ИИ потерял противника
                    {
                        LostTarget();
                    }
                }
                else
                {
                    isTargetMissed = true;
                    isFindTarget = false;
                    LostTarget();
                }
            }
        }
    }

    // Комплексные методы
    #region ComplexMethods

    private void MoveToPoint() // Движение к точке
    {
        UpdatePath(NextTarget.position);

        LookAt(MoveTargetV3 - rb.velocity * MoveToPointVelocityConsideration); // Поворачиваем корабль к точке, учитывая скорость

        Move(MoveDirectionConsideringAngle(1, AngleTo(MoveTargetV3 - rb.velocity / 2))); // Двигаемся

        if (Vector3.Distance(transform.position, NextTarget.position) < leavePointDistance) // Если мы приблизились к точке достаточно близко
        {
            GetNextPoint(); // Следующая точка
        }
    }

    private void LostTarget()
    {
        FindClosestPoint(); // Находим ближайшую точку
        isFear = false; // Испуг заканчивается
        FearTimeNow = 0; // Время испуга обнуляется
        isMoveToPoint = true; // Начать движение к точке
    }

    #endregion

    // Просто методы
    #region Methods

    private void Move(float direction) => Engine.Move(direction); // Движение

    private void SlowDown() => Engine.SlowDown(); // Замедление

    private void LookAt(Vector3 target) => RotationEngine.RotateToTarget(target); // Повернуть корабль в сторону точки

    private void GetNextPoint() // Получаем следующую контрольную точку для движения
    {
        PreviousTarget = CurrentTarget;
        CurrentTarget = NextTarget;
        NextTarget = System_Waypoints.GetNextPoint(CurrentTarget, PreviousTarget);
    }

    private void FindClosestPoint()
    {
        PreviousTarget = CurrentTarget;
        CurrentTarget = NextTarget;
        NextTarget = System_Waypoints.GetClosePoint(transform);
    }

    int v;
    private void UpdatePath(Vector3 Target)
    {
        NavMesh.CalculatePath(transform.position, Target, NavMesh.AllAreas, NavMeshPath);

        if (NavMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            v = 1;
            while (v < NavMeshPath.corners.Length - 1 && Vector3.Distance(NavMeshPath.corners[v], transform.position) < MinDistanceToNavMeshCorner)
            {
                v += 1;
            }
            MoveTargetV3 = NavMeshPath.corners[v];
        }
    }

    #endregion

    // Что-то возвращают
    #region Return

    private float MoveDirectionConsideringAngle(float moveDirection, float angle)
    {
        if (angle > moveAngle) // Если угол больше
        {
            if (angle > 90) // Если угол больше 90, то меняем направление скорости на противоположное
            {
                moveDirection *= -1;
            }
            moveDirection /= angle / moveAngle; // Уменьшаем вектор направления для снижения скорости
        }
        return moveDirection;
    }

    private float AngleTo(Vector3 V3) => Vector3.Angle((V3 - transform.position).normalized, RotPointTr.forward); // Угол между направлением к цели и передом корабля

    #endregion

    //Что-то устанавливают
    #region Setters

    public void SetLockControl(bool t) // Вкл/выкл управление кораблём
    {
        isControlLock = t;
        if (!t)
        {
            FindClosestPoint(); // Если включают управление, то ищем ближнюю точку
        }
        Engine.SetLockMove(t);
        RotationEngine.SetLockRotate(t);
    }

    public void SetFollowTarget(bool t) // Вкл/выкл поиск противника
    {
        isFollowTarget = t;
        isFindTarget = false;
    }

    public void SetAttack(bool t) // Вкл/выкл поиск противника
    {
        isAttack = t;
    }

    public void SetEnemyTarget(Transform Tr)
    {
        isTargetMissed = false;
        if (Tr != EnemyTarget)
        {
            EnemyTarget = Tr; // Установка противника, цели
            EnemyRb = Tr.GetComponent<Rigidbody>();
        }
    }

    #endregion

    private enum Mastery
    {
        Noob,
        Normal,
        Pro,
        Elite,
    }
}
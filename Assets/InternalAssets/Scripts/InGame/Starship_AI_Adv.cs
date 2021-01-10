using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Guns), typeof(Rigidbody))]
public class Starship_AI_Adv : MonoBehaviour
{
    [SerializeField] private Mastery mastery;
    [SerializeField] private float fearDistance = 9;
    [SerializeField] private float followMinDistance = 15;
    [SerializeField] private float followMaxDistance = 40;
    [SerializeField] private float showPathMinDistance = 14;
    [SerializeField] private float showPathMaxDistance = 20;
    [SerializeField] private float betterPointMinDistance = 10;
    [SerializeField] private float attackDistance = 11;
    [SerializeField] private float findDistance = 26;
    [SerializeField] private float lostDistance = 40;
    [SerializeField] private float leavePointDistance = 4;
    [SerializeField] private float moveAngle = 15; // Угол, выше которого снижается мощность двигателя
    [SerializeField] private float shootAngle = 70;
    [SerializeField] private float FearTime = 1.5f;
    [SerializeField] private float MoveToPointVelocityConsideration = 0.5f;
    [SerializeField] private float MinDistanceToNavMeshCorner = 0.5f;
    [SerializeField] private float maxShootSpeed = 200;

    [SerializeField] private bool isControlLock = false;
    [SerializeField] private bool isAttack = true;
    [SerializeField] private bool isFollowEnemy = true;

    [SerializeField] private Transform RotPointTr;

    [SerializeField] private Starship_Engine Engine;
    [SerializeField] private Starship_RotationEngine RotationEngine;

    [SerializeField] private NavMeshPath NavMeshPath;

    private Guns.ShootEvent Shoot;

    private Rigidbody rb; // Основное
    private Rigidbody EnemyRb;

    private Transform EnemyTarget;
    private Transform FollowTarget;
    private Transform ShowPathTarget;
    private Transform ShowPathDestination;
    private Transform PreviousTarget;
    private Transform CurrentTarget;
    private Transform NextTarget;
    private Vector3 EnemyTargetV3; // Не менять на transform, ссылка может пропасть

    private bool isFindEnemy;
    private bool isFear;
    private bool isMoveToPoint;
    private bool isFollowTarget;
    private bool isShowPathTarget;
    private bool isMaxDistanceFollow;
    private bool isTargetMissed;

    private float TerrorAimingKoef = 0;

    private float FearTimeNow;
    private float distanceToEnemy;

    private Vector3 MoveTargetV3;
    private Vector3 localUp = Vector3.up;

    private RaycastHit enemyRaycastHitInfo;

    private void Awake()
    {
        MoveToPointVelocityConsideration *= Random.Range(0.9f, 1.1f);
        switch (mastery)
        {
            case Mastery.Noob:
                fearDistance *= Random.Range(1, 1.3f);
                attackDistance *= Random.Range(0.8f, 1f);
                findDistance *= Random.Range(0.7f, 1f);
                lostDistance *= Random.Range(0.7f, 1f);
                leavePointDistance *= Random.Range(0.7f, 1f);
                moveAngle *= Random.Range(0.7f, 1f);
                shootAngle *= Random.Range(1f, 1.3f);
                FearTime *= Random.Range(1f, 1.3f);
                TerrorAimingKoef = Random.Range(1.25f, 1.5f);
                break;
            case Mastery.Normal:
                fearDistance *= Random.Range(1, 1.2f);
                attackDistance *= Random.Range(0.9f, 1f);
                findDistance *= Random.Range(0.9f, 1f);
                lostDistance *= Random.Range(0.9f, 1f);
                leavePointDistance *= Random.Range(0.9f, 1f);
                moveAngle *= Random.Range(0.8f, 1f);
                shootAngle *= Random.Range(1f, 1.2f);
                FearTime *= Random.Range(1f, 1.2f);
                TerrorAimingKoef = Random.Range(1f, 1.2f);
                break;

            case Mastery.Pro:
                fearDistance *= Random.Range(1f, 1.1f);
                attackDistance *= Random.Range(1f, 1.1f);
                findDistance *= Random.Range(1f, 1.1f);
                lostDistance *= Random.Range(1f, 1.1f);
                leavePointDistance *= Random.Range(1f, 1.1f);
                moveAngle *= Random.Range(0.9f, 1.2f);
                shootAngle *= Random.Range(0.9f, 1.1f);
                FearTime *= Random.Range(0.9f, 1.1f);
                TerrorAimingKoef = Random.Range(0.4f, 0.6f);
                break;
            case Mastery.Elite:
                fearDistance *= Random.Range(1f, 1.1f);
                attackDistance *= Random.Range(1f, 1.1f);
                findDistance *= Random.Range(1f, 1.3f);
                lostDistance *= Random.Range(1f, 1.3f);
                leavePointDistance *= Random.Range(1f, 1.2f);
                moveAngle *= Random.Range(1f, 1.3f);
                shootAngle *= Random.Range(1f, 1.1f);
                FearTime *= Random.Range(1f, 1.05f);
                TerrorAimingKoef = Random.Range(0.25f, 0.35f);
                break;
            case Mastery.Beta:
                fearDistance *= Random.Range(0.99f, 1.01f);
                attackDistance *= Random.Range(1f, 1.2f);
                findDistance *= Random.Range(1f, 1.4f);
                lostDistance *= Random.Range(1f, 1.4f);
                leavePointDistance *= Random.Range(1f, 1.3f);
                moveAngle *= Random.Range(1f, 1.35f);
                shootAngle *= Random.Range(0.7f, 1f);
                FearTime *= Random.Range(0.95f, 1f);
                TerrorAimingKoef = Random.Range(0.1f, 0.2f);
                break;
            case Mastery.Legend:
                TerrorAimingKoef = 0;
                break;
        }

        NavMeshPath = new NavMeshPath();

        rb = GetComponent<Rigidbody>();
        Starship starship = GetComponent<Starship>();
        starship.SetEnemyTarget = SetEnemyTarget;
        starship.SetFollowEnemy = SetFollowEnemy;
        starship.SetLockControl = SetLockControl;
        starship.SetAttack = SetAttack;
        starship.SetFollowTarget = SetTargetToFollow;

        localUp = transform.TransformDirection(localUp);

        SetLockControl(true);
        isMoveToPoint = true;

    }

    private void Start()
    {
        Guns guns = GetComponent<Guns>();
        Shoot = guns.GetShootEvent();
    }

    private void FixedUpdate()
    {
        if (!isControlLock) // Если управление не отключено
        {
            CheckNulls();
            if (isShowPathTarget)
            {
                ShowPath();
            }
            else
            if (!isFindEnemy || isMaxDistanceFollow && isFollowTarget && Vector3.Distance(FollowTarget.position, transform.position) > followMaxDistance) // Если цель не найдена или мы должны поддерживать дистанцию до преследуемой цели
            {
                if (isFollowTarget)
                {
                    MoveToFollowTarget();
                }
                else
                {
                    if (isMoveToPoint) // Если можно двигаться к точке
                    {
                        MoveToPoint(); // Двигаемся к точке
                    }
                    else
                    {
                        Engine.SlowDown();
                        RotationEngine.SlowDown();
                    }
                }
                if (isFollowEnemy && !isTargetMissed) // Если ИИ ищет цель
                {
                    if (EnemyTarget != null)
                    {
                        isFindEnemy = Vector3.Distance(EnemyTarget.position, transform.position) < findDistance; // Проверяем видит ли ИИ противника
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
                            LookAt(EnemyPositionConsidetingEverything()); // Поворачиваем корабль в сторону цели, учитывая всё

                            Move(MoveDirectionConsideringAngle(1, AngleTo(MoveTargetV3 - MasteryVelocity())));

                            if (isAttack && AngleTo(EnemyPositionConsidetingEverything()) < shootAngle && Vector3.Distance(transform.position, EnemyTargetV3) < attackDistance) // Если ИИ может атаковать, и угол до цели, учитывая всё, меньше угла прицеливания
                            {
                                Shoot(EnemyTarget); // Стреляем
                            }
                        }
                        else
                        {
                            LookAt(MoveTargetV3 - MasteryVelocity()); // Поворачиваем корабль в сторону нужного движения
                            Move(MoveDirectionConsideringAngle(1, AngleTo(MoveTargetV3 - MasteryVelocity())));
                        }

                        if (distanceToEnemy < fearDistance) // Если дистанция до цели меньше минимальной дистанции
                        {
                            FindBetterPoint(); // Находим лучшую точку
                            isFear = true; // Испуг
                        }
                    }
                    else
                    {
                        if (isAttack && AngleTo(EnemyTargetV3) < shootAngle && Vector3.Distance(transform.position, EnemyTargetV3) < attackDistance) // Если ИИ может атаковать, и угол до цели меньше угла прицеливания
                        {
                            Shoot(EnemyTarget); // Стреляем
                        }
                        // Отсчёт страха
                        FearTimeNow += Time.fixedDeltaTime;
                        if (FearTimeNow >= FearTime)
                        {
                            isFear = false;
                            FearTimeNow = 0;
                        }
                        if (isMoveToPoint)
                        {
                            MoveToPoint(); // Двигаемся к точке
                        }
                        else
                        {
                            FindClosestPoint();
                            isMoveToPoint = true;
                        }
                    }
                    isFindEnemy = distanceToEnemy < lostDistance; // Проверяем видит ли ИИ противника
                    if (!isFindEnemy) // Если ИИ потерял противника
                    {
                        LostTarget();
                    }
                }
                else
                {
                    isTargetMissed = true;
                    isFindEnemy = false;
                    LostTarget();
                }
            }
        }
    }

    // Комплексные методы
    #region ComplexMethods

    private void CheckNulls()
    {
        if (isFollowTarget && FollowTarget == null)
        {
            Unfollow();
        }
        if (isShowPathTarget && ShowPathTarget == null)
        {
            UnshowPath();
        }
    }

    private void MoveToPoint() // Движение к точке
    {
        UpdatePath(NextTarget.position);

        LookAt(MoveTargetV3 - MasteryVelocity()); // Поворачиваем корабль к точке, учитывая скорость

        Move(MoveDirectionConsideringAngle(1, AngleTo(MoveTargetV3 - MasteryVelocity()))); // Двигаемся

        if (Vector3.Distance(transform.position, NextTarget.position) < leavePointDistance) // Если мы приблизились к точке достаточно близко
        {
            GetNextPoint(); // Следующая точка
        }
    }

    private void MoveToFollowTarget()
    {
        if (Vector3.Distance(transform.position, FollowTarget.position) > followMinDistance)
        {
            UpdatePath(FollowTarget.position);

            LookAt(MoveTargetV3 - MasteryVelocity()); // Поворачиваем корабль к точке, учитывая скорость

            Move(MoveDirectionConsideringAngle(1, AngleTo(MoveTargetV3 - MasteryVelocity()))); // Двигаемся
        }
        else
        {
            Engine.SlowDown();
            RotationEngine.SlowDown();
        }
    }

    private void ShowPath()
    {
        if (Vector3.Distance(transform.position, ShowPathTarget.position) > showPathMinDistance && Vector3.Distance(ShowPathDestination.position, transform.position) < Vector3.Distance(ShowPathDestination.position, ShowPathTarget.position))
        {
            if (Vector3.Distance(transform.position, ShowPathTarget.position) < showPathMaxDistance)
            {
                Engine.SlowDown();
                RotationEngine.SlowDown();
            }
            else
            {
                UpdatePath(ShowPathTarget.position);

                LookAt(MoveTargetV3 - MasteryVelocity()); // Поворачиваем корабль к точке, учитывая скорость

                Move(MoveDirectionConsideringAngle(1, AngleTo(MoveTargetV3 - MasteryVelocity()))); // Двигаемся
            }

        }
        else
        {
            UpdatePath(ShowPathDestination.position);

            LookAt(MoveTargetV3 - MasteryVelocity()); // Поворачиваем корабль к точке, учитывая скорость

            Move(MoveDirectionConsideringAngle(1, AngleTo(MoveTargetV3 - MasteryVelocity()))); // Двигаемся
        }
    }

    private Vector3 MasteryVelocity() => rb.velocity * MoveToPointVelocityConsideration + rb.velocity * MoveToPointVelocityConsideration * Random.Range(-TerrorAimingKoef, TerrorAimingKoef);

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

    private void LookAt(Vector3 target) => RotationEngine.RotateToTargetWithPlaneLimiter(target); // Повернуть корабль в сторону точки

    private void GetNextPoint() // Получаем следующую контрольную точку для движения
    {
        PreviousTarget = CurrentTarget;
        CurrentTarget = NextTarget;
        NextTarget = System_Waypoints.GetNextPoint(CurrentTarget, PreviousTarget);
        if (NextTarget == null)
        {
            isMoveToPoint = false;
        }
    }

    private void FindClosestPoint()
    {
        PreviousTarget = CurrentTarget;
        CurrentTarget = NextTarget;
        NextTarget = System_Waypoints.GetClosestPoint(transform);
    }
    private void FindBetterPoint()
    {
        PreviousTarget = CurrentTarget;
        CurrentTarget = NextTarget;
        NextTarget = System_Waypoints.GetBetterPointInDistance(transform, betterPointMinDistance); 
    }

    int v;
    private void UpdatePath(Vector3 Target)
    {
        v = 0;
        NavMesh.CalculatePath(transform.position, Target, NavMesh.AllAreas, NavMeshPath);

        if (NavMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            v = 1;
            while (v < NavMeshPath.corners.Length - 1 && Vector3.Distance(NavMeshPath.corners[v], transform.position) < MinDistanceToNavMeshCorner)
            {
                v += 1;
            }
            if (NavMeshPath.corners.Length > 0)
            {
                MoveTargetV3 = NavMeshPath.corners[v];
            }
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
                if (180 - angle > moveAngle)
                {
                    moveDirection /= (180 - angle) / moveAngle; // Уменьшаем вектор направления для снижения скорости
                }
                moveDirection *= -1;
            }
            else
            {
                moveDirection /= angle / moveAngle; // Уменьшаем вектор направления для снижения скорости
            }
        }
        return moveDirection;
    }

    private float AngleTo(Vector3 V3) => Vector3.Angle((V3 - transform.position).normalized, RotPointTr.forward); // Угол между направлением к цели и передом корабля

    private Vector3 EnemyPositionConsidetingEverything() => EnemyTargetV3 - MasteryVelocity() + EnemyRb.velocity * (Vector3.Distance(transform.position, EnemyTargetV3 + EnemyRb.velocity) / maxShootSpeed) + EnemyRb.velocity * (Vector3.Distance(transform.position, EnemyTargetV3 + EnemyRb.velocity) / maxShootSpeed) * (1 + Random.Range(-TerrorAimingKoef, TerrorAimingKoef));

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

    public void SetFollowEnemy(bool t) // Вкл/выкл поиск противника
    {
        isFollowEnemy = t;
        isFindEnemy = false;
    }

    public void SetAttack(bool t) // Вкл/выкл поиск противника
    {
        isAttack = t;
    }

    public void SetEnemyTarget(Transform Enemy)
    {
        isTargetMissed = false;
        if (Enemy != EnemyTarget)
        {
            EnemyTarget = Enemy; // Установка противника, цели
            EnemyRb = Enemy.GetComponent<Rigidbody>();
        }
    }

    public void SetTargetToFollowWithMaxDistance(Transform Target)
    {
        FollowTarget = Target; // Установка цели для преследования
        isMaxDistanceFollow = true;
        isFollowTarget = true;
    }
    public void SetTargetToFollow(Transform Target)
    {
        FollowTarget = Target; // Установка цели для преследования
        isMaxDistanceFollow = false;
        isFollowTarget = true;
    }
    public void Unfollow()
    {
        isMaxDistanceFollow = false;
        isFollowTarget = false;
    }

    public void SetTargetAndDestinationToShowPath(Transform target, Transform destination)
    {
        ShowPathDestination = destination;
        ShowPathTarget = target;
        isShowPathTarget = true;
    }
    public void UnshowPath()
    {
        isShowPathTarget = false;
    }

    #endregion

    private enum Mastery
    {
        Noob,
        Normal,
        Pro,
        Elite,
        Beta,
        Legend,
    }
}
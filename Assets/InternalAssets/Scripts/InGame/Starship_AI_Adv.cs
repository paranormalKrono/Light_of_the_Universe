using UnityEngine;

[RequireComponent(typeof(Guns), typeof(Rigidbody))]
public class Starship_AI_Adv : MonoBehaviour
{
    [SerializeField] private float minDistance = 9;
    [SerializeField] private float findDistance = 26;
    [SerializeField] private float lostDistance = 40;
    [SerializeField] private float leavePointDistance = 4;
    [SerializeField] private float moveForce = 700;
    [SerializeField] private float moveSpeedMax = 19;
    [SerializeField] private float moveFriction = 20;
    [SerializeField] private float moveAngle = 15; // Угол, выше которого снижается мощность двигателя
    [SerializeField] private float rotateForce = 600;
    [SerializeField] private float rotateSpeedMax = 2.6f;
    [SerializeField] private float shootAngle = 70;
    [SerializeField] private float FearTime = 1.5f;
    [SerializeField] private float stayTimeMin = 0.2f;
    [SerializeField] private float stayTimeMax = 1f;
    [SerializeField] private float trappedTime = 2;
    [SerializeField] private float trappedDistance = 0.1f;

    [SerializeField] private bool isControlLock = false;
    [SerializeField] private bool isAttack = true;
    [SerializeField] private bool isFollowTarget = true;

    [SerializeField] private Transform RotPointTr;

    private Guns.ShootEvent Shoot;

    private Rigidbody rb; // Основное
    private Rigidbody starshipRb; // Отвечает за поворот
    private Rigidbody EnemyRb;

    private Transform EnemyTarget;
    private Vector3 EnemyTargetV3; // Не менять на transform, ссылка может пропасть

    public delegate void EventTransformHandler(Transform Tr);
    public event EventTransformHandler DeathEvent;

    private bool isFindTarget;
    private bool isFear;
    private bool isTrapped;
    private bool isMoveToPoint;
    private bool isTargetMissed;

    private float stayTime;
    private float stayTimeNow;
    private float FearTimeNow;
    private float trappedTimeNow;
    private float distanceToTarget;
    private float maxShootSpeed;


    private Vector3 vector3Zero = Vector3.zero;
    private Vector3 localUp = Vector3.up;
    private Vector3 forward = Vector3.forward;


    private Transform PreviousPoint;
    private Transform CurrentPoint;
    private Transform NextPoint;
    private Vector3 CurrentPointV3;
    private Vector3 NextPointV3;
    private Vector3 PreviousPos;

    private Vector3 v3, point;
    private float f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Starship starship = GetComponent<Starship>();
        starship.SetEnemyTarget = SetEnemyTarget;
        starship.SetFollowTarget = SetFollowTarget;
        starship.SetLockControl = SetLockControl;
        starship.SetAttack = SetAttack;
        starshipRb = RotPointTr.GetComponent<Rigidbody>();

        localUp = transform.TransformDirection(localUp);

        SetLockControl(true);
        isMoveToPoint = true;
    }
    private void Start()
    {
        Guns guns = GetComponent<Guns>();
        guns.Initialize(rb, out Shoot);
        maxShootSpeed = guns.MaxShootSpeed;
    }

    private void FixedUpdate()
    {
        if (!isControlLock) // Если управление не отключено
        {
            if (isTrapped) // Если ИИ застрял
            {
                Trap();
            }
            else 
            {
                if (!isFindTarget) // Если цель не найдена
                {
                    if (isMoveToPoint) // Если можно двигаться к точке
                    {
                        TryTrap(); // Проверяем застрял ли ИИ
                        MoveToPoint(); // Двигаемся к точке
                    }
                    else
                    {
                        Stay();
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
                        distanceToTarget = Vector3.Distance(EnemyTargetV3, transform.position); // Получаем дистанцию до цели

                        f = Angle(EnemyTargetV3); // Получаем угол до цели
                        if (isAttack && f < shootAngle) // Если ИИ может атаковать и угол меньше угла прицеливания
                        {
                            Shoot(); // Стреляем
                        }
                        if (!isFear) // Если не испугался
                        {
                            if (distanceToTarget > findDistance)
                            {
                                f = Angle(EnemyTargetV3 + EnemyRb.velocity - rb.velocity); // Получаем угол до цели
                                LookAt(EnemyTargetV3 + EnemyRb.velocity - rb.velocity, f); // Поворачиваем корабль в сторону цели относительно угла
                            }
                            else
                            {
                                if (f < 90)
                                {
                                    f = Angle(EnemyTargetV3 + (EnemyRb.velocity - rb.velocity) * (Vector3.Distance(transform.position, EnemyTargetV3) / maxShootSpeed)); // Получаем угол до цели
                                    LookAt(EnemyTargetV3 + (EnemyRb.velocity - rb.velocity) * (Vector3.Distance(transform.position, EnemyTargetV3) / maxShootSpeed), f); // Поворачиваем корабль в сторону цели относительно угла
                                }
                                else
                                {
                                    f = Angle(EnemyTargetV3); // Получаем угол до цели
                                    LookAt(EnemyTargetV3, f); // Поворачиваем корабль в сторону цели относительно угла
                                }
                            }
                            Move(forward); // Двигаемся вперёд

                            isTrapped = TryTrap(); // Проверяем застрял ли ИИ
                            if (isTrapped || distanceToTarget < minDistance) // Если застрял или дистанция до цели меньше минимльной дистанции
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
                            isTrapped = TryTrap(); // Проверяем застрял ли ИИ
                        }
                        isFindTarget = distanceToTarget < lostDistance; // Проверяем видит ли ИИ противника
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
    }

    // Комплексные методы
    #region ComplexMethods

    private void MoveToPoint() // Движение к точке
    {
        point = Vector3.Lerp(CurrentPointV3, NextPointV3, leavePointDistance / Vector3.Distance(transform.position, CurrentPointV3));
        if (Vector3.Distance(point - rb.velocity, NextPointV3) > Vector3.Distance(transform.position, NextPointV3))
        {
            f = Angle(point); // Получаем угол
            LookAt(point, f); // Поворачиваем корабль к точке
        }
        else
        {
            f = Angle(point - rb.velocity); // Получаем угол
            LookAt(point - rb.velocity, f); // Поворачиваем корабль к точке
        }

        v3 = forward; // Определяем направление
        if (f > moveAngle) // Если угол больше, то уменьшаем мощность двигателя
        {
            v3 /= f / moveAngle;
        }
        Move(v3); // Двигаемся вперёд относительно угла
        if (Vector3.Distance(transform.position, CurrentPointV3) < leavePointDistance || Vector3.Distance(transform.position, NextPointV3) < leavePointDistance || Vector3.Distance (transform.position, point) < leavePointDistance * 2) // Если мы приблизились к точке достаточно близко
        {
            GetNextPoint(); // Следующая точка
            if (Random.Range(0, 5) == 1) // Выюираем то, сколько стоять на месте
            {
                stayTime = stayTimeMax;
            }
            else
            {
                stayTime = stayTimeMin;
            }
            isMoveToPoint = false; // Прекратить движение к точке
        }
    }

    private void LookAt(Vector3 V3, float ang) // Повернуть корабль в сторону точки, зная угол
    {
        Vector3 rotVect = -Vector3.Cross((V3 - transform.position).normalized, RotPointTr.forward).normalized;

        // В плоскости
        rotVect.y = 0;
        rotVect.z = 0;

        if (ang < 5) // Если угол меньше 5, то уменьшаем ускорение и максимальную скорость
        {
            Rotate(Time.fixedDeltaTime * rotateForce * ang * rotVect / 10, rotateSpeedMax / 12);
        }
        else
        {
            Rotate(Time.fixedDeltaTime * rotateForce * ang * rotVect, rotateSpeedMax);
        }
    }

    private void Stay() // Встать на месте
    {
        stayTimeNow += Time.fixedDeltaTime; // Отсчёт времени стояния
        if (stayTimeNow >= stayTime)
        {
            stayTimeNow = 0;
            isMoveToPoint = true;
        }
        MoveFriction(); // Остановка
    }

    private void Trap() // Застревание 
    {
        // Отсчёт
        trappedTimeNow += Time.fixedDeltaTime;
        if (trappedTimeNow < trappedTime)
        {
            Move(-forward / 4); // Двигаться назад
        }
        else
        {
            trappedTimeNow = 0;
            PreviousPos = transform.position;
            FindClosestPoint();
            isTrapped = false;
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

    private void Move(Vector3 moveDirection) // Движение в какую-то сторону
    {
        moveDirection = RotPointTr.TransformDirection(moveDirection);
        rb.AddForce(moveDirection * moveForce * Time.fixedDeltaTime, ForceMode.Impulse);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeedMax);
    }

    private void MoveFriction() => rb.velocity = Vector3.Lerp(rb.velocity, vector3Zero, Time.fixedDeltaTime * moveFriction); // Трение при движении
    
    private void Rotate(Vector3 torque, float maxSpeed) // Поворот с ограничением по скорости
    {
        starshipRb.AddTorque(torque, ForceMode.Acceleration); 
        starshipRb.angularVelocity = Vector3.ClampMagnitude(starshipRb.angularVelocity, maxSpeed);
    }

    private void GetNextPoint() // Следующая точка
    {
        // Заменяем прошлую точку на текущую, екущую на следующую и получаем последующую точку для движения
        PreviousPoint = CurrentPoint;
        CurrentPoint = NextPoint;
        NextPoint = System_Waypoints.GetNextPoint(CurrentPoint, PreviousPoint);
        CurrentPointV3 = CurrentPoint.position;
        NextPointV3 = NextPoint.position;
    }

    private void FindClosestPoint()
    {
        CurrentPoint = System_Waypoints.GetClosePoint(transform); // Получаем ближайшую точку
        NextPoint = CurrentPoint;
        CurrentPointV3 = CurrentPoint.position;
        NextPointV3 = CurrentPointV3;
    }

    #endregion

    // Что-то возвращают
    #region Return

    private bool TryTrap() // Проверка не застрял ли корабль
    {
        if (Vector3.Distance(transform.position, PreviousPos) < trappedDistance) // Проверяем пролетел ли корабль хоть какую-либо дистанцию
        {
            // Если не пролетел, начинаем отсчёт
            trappedTimeNow += Time.deltaTime;
            if (trappedTimeNow >= trappedTime) // Если отсчёт закончился, но корабль не вылетел из мёртвой зоны, то он застрял
            {
                trappedTimeNow = 0;
                PreviousPos = transform.position;
                return true; // Застрял
            }
        }
        else
        {
            trappedTimeNow = 0; // Отсчёт обнуляется
        }
        PreviousPos = transform.position;
        return false; // Не застрял
    }

    private float Angle(Vector3 V3) => Vector3.Angle((V3 - transform.position).normalized, RotPointTr.forward); // Угол между направлением к цели и передом корабля

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
        else
        {
            rb.velocity = vector3Zero;
            rb.angularVelocity = vector3Zero;
        }
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

}
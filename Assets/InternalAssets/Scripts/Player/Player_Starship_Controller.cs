using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Guns), typeof(Rigidbody))]
public class Player_Starship_Controller : MonoBehaviour
{
    [SerializeField] private float moveForce = 700;
    [SerializeField] private float moveSpeedMax = 19;
    [SerializeField] private float moveFriction = 20;
    [SerializeField] private float rotateForce = 600;
    [SerializeField] private float rotateSpeedMax = 2.6f;
    [SerializeField] private float rotateSpeedFriction = 3.5f;

    [SerializeField] private bool isLockShoot = false;

    [SerializeField] private Image StarshipActivatedImage;
    [SerializeField] private Image FeatherActivatedImage;
    [SerializeField] private Image ChainActivatedImage;

    [SerializeField] private Transform RotPointTr;

    public float MoveForce
    {
        get
        {
            if (isFreeFly)
            {
                return moveForce / 1.1f;
            }
            else if (isLockFly)
            {
                return moveForce * 1.25f;
            }
            else
            {
                return moveForce;
            }
        }
        set
        {
            moveForce = value;
        }
    }

    public float MoveSpeedMax
    {
        get
        {
            if (isFreeFly)
            {
                return moveSpeedMax * 1.4f;
            }
            else if (isLockFly)
            {
                return moveSpeedMax / 1.25f;
            }
            else
            {
                return moveSpeedMax;
            }
        }
        set
        {
            moveSpeedMax = value;
        }
    }

    public float MoveFriction
    {
        get
        {
            if (isLockFly)
            {
                return moveFriction * 2;
            }
            else
            {
                return moveFriction;
            }
        }
        set
        {
            moveFriction = value;
        }
    }

    public float RotateForce
    {
        get
        {
            if (isLockFly)
            {
                return rotateForce * 1.25f;
            }
            else
            {
                return rotateForce;
            }
        }
        set
        {
            rotateForce = value;
        }
    }

    public float RotateSpeedMax
    {
        get
        {
            if (isLockFly)
            {
                return rotateSpeedMax * 1.25f;
            }
            else
            {
                return rotateSpeedMax;
            }
        }
        set
        {
            rotateSpeedMax = value;
        }
    }

    private Rigidbody rb;
    private Rigidbody starshipRb;
    private Camera camera;

    private Guns Guns;
    private Guns.ShootEvent Shoot;

    private bool isLockControl;
    private bool isLockMove;
    private bool isLockRotate;
    private bool isGameMenu;

    private bool isLockFly = false;
    private bool isFreeFly = false;

    private Vector3 vector3Zero = Vector3.zero;
    private Vector3 localUp = Vector3.up;
    private Vector3 forward = Vector3.forward;

    private Vector3 moveDirection;
    private Vector3 rotateDirection;


    private Vector3 v3;
    private float f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        starshipRb = RotPointTr.GetComponent<Rigidbody>();
        Guns = GetComponent<Guns>();
        Guns.Initialize(rb, out Shoot);

        StarshipActivatedImage.enabled = true;

        localUp = transform.TransformDirection(localUp);

        GameMenu.OnMenuOpen += OnMenuOpen;
        GameMenu.OnMenuClose += OnMenuClose;
    }

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (!isLockControl && !isGameMenu)
        {
            if (!isLockShoot)
            {
                if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
                {
                    Shoot();
                }
            }
            if (!isLockMove)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    isLockFly = false;
                    ChainActivatedImage.enabled = false;
                    isFreeFly = !isFreeFly;
                    if (isFreeFly)
                    {
                        StarshipActivatedImage.enabled = false;
                        FeatherActivatedImage.enabled = true;
                    }
                    else
                    {
                        StarshipActivatedImage.enabled = true;
                        FeatherActivatedImage.enabled = false;
                    }
                }
                if (Input.GetKeyDown(KeyCode.V))
                {
                    isFreeFly = false;
                    FeatherActivatedImage.enabled = false;
                    isLockFly = !isLockFly;
                    if (isLockFly)
                    {
                        StarshipActivatedImage.enabled = false;
                        ChainActivatedImage.enabled = true;
                    }
                    else
                    {
                        StarshipActivatedImage.enabled = true;
                        ChainActivatedImage.enabled = false;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isLockControl && !isGameMenu)
        {
            if (!isLockRotate)
            {
                if (Input.GetMouseButton(1))
                {
                    RotateMouse();
                }
                else
                {
                    Rotate();
                }
            }
            else
            {
                starshipRb.angularVelocity = vector3Zero;
            }
            if (!isLockMove)
            {
                Move();
            }
            else
            {
                rb.velocity = vector3Zero;
            }
        }
        else
        {
            rb.velocity = vector3Zero;
            starshipRb.angularVelocity = vector3Zero;
        }
    }


    private void Rotate()
    {
        rotateDirection.Set(0, Input.GetAxis("Horizontal"), 0);
        if (!rotateDirection.Equals(vector3Zero))
        {
            rotateDirection = transform.TransformDirection(rotateDirection);
            starshipRb.AddTorque(Time.fixedDeltaTime * rotateDirection * 20 * RotateForce, ForceMode.Acceleration);
            starshipRb.angularVelocity = Vector3.ClampMagnitude(starshipRb.angularVelocity, RotateSpeedMax);
        }
        else
        {
            RotateFriction();
        }
    }

    private void RotateFriction() => starshipRb.angularVelocity = Vector3.MoveTowards(starshipRb.angularVelocity, vector3Zero, Time.fixedDeltaTime * 30 * rotateSpeedFriction);

    private void RotateMouse()
    {
        v3 = camera.ScreenPointToRay(Input.mousePosition).direction.normalized;
        v3.x = 0;

        float ang = Vector3.Angle(RotPointTr.forward, v3);
        Vector3 rotVect = -Vector3.Cross(v3, RotPointTr.forward).normalized;
        if (ang < 5)
        {
            starshipRb.angularVelocity = Vector3.ClampMagnitude(starshipRb.angularVelocity, ang / 10);
        }

        starshipRb.AddTorque(Time.fixedDeltaTime * RotateForce * ang * rotVect, ForceMode.Acceleration);
        starshipRb.angularVelocity = Vector3.ClampMagnitude(starshipRb.angularVelocity, RotateSpeedMax);
    }


    private void Move()
    {
        f = Input.GetAxis("Vertical");
        if (f < 0)
        {
            f /= 2;
        }
        if (f != 0)
        {
            moveDirection = forward * f;
            moveDirection = RotPointTr.TransformDirection(moveDirection);
            rb.AddForce(moveDirection * MoveForce * Time.fixedDeltaTime, ForceMode.Impulse);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, MoveSpeedMax);
        }
        else if (!isFreeFly)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, vector3Zero, Time.fixedDeltaTime * MoveFriction);
        }
    }

    internal void SetLockControl(bool t)
    {
        isLockControl = t;
        rb.velocity = vector3Zero;
        starshipRb.angularVelocity = vector3Zero;
        Guns.SetLockMove(t);
    }
    internal void SetLockControl(bool isLockMovet, bool isLockTorquet, bool isLockShoott)
    {
        rb.velocity = vector3Zero;
        starshipRb.angularVelocity = vector3Zero;
        isLockMove = isLockMovet;
        Guns.SetLockMove(isLockMove);
        isLockRotate = isLockTorquet;
        isLockShoot = isLockShoott;
    }
    internal void GetPositionAndRotationTransforms(out Transform PositionTr, out Transform RotationTr)
    {
        PositionTr = transform;
        RotationTr = RotPointTr;
    }


    private void OnMenuOpen() => isGameMenu = true;

    private void OnMenuClose() => isGameMenu = false;
}
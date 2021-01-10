using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Guns), typeof(Rigidbody))]
public class Player_Starship_Controller : MonoBehaviour
{

    [SerializeField] private bool isAbsoluteLockShoot = false;

    [SerializeField] private Image StarshipActivatedImage;
    [SerializeField] private Image FeatherActivatedImage;
    [SerializeField] private Image ChainActivatedImage;

    [SerializeField] private Transform rotationTransform;

    [SerializeField] private Starship_Engine engine;
    [SerializeField] private Starship_RotationEngine rotationEngine;

    [SerializeField] private GameObject canvas;

    private Camera camera;

    private Guns Guns;
    private Guns.ShootEvent Shoot;

    private FlightMode currentFlightMode = FlightMode.Normal;

    private bool isLockMove;
    private bool isLockRotate;
    private bool isLockShoot;
    private bool isGameMenu;

    private Vector3 localUp = Vector3.up;
    private Vector3 localDown = Vector3.down;
    private Vector3 forward = Vector3.forward;

    private Vector3 v3;

    private float verticalInput;
    private float horizontalInput;

    private float hitDist;
    private Ray ray;
    private Plane plane = new Plane();

    private float engineForce;
    private float engineSpeedMax;
    private float engineFriction;
    private float rotationEngineForce;
    private float rotationEngineSpeedMax;
    private float rotationEngineFriction;


    private void Awake()
    {
        Guns = GetComponent<Guns>();
        Shoot = Guns.GetShootEvent();

        StarshipActivatedImage.enabled = true;

        localUp = transform.TransformDirection(localUp);
        localDown = transform.TransformDirection(localDown);

        GameMenu.OnMenuOpen += OnMenuOpen;
        GameMenu.OnMenuClose += OnMenuClose;

    }

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();

        engineForce = engine.force;
        engineSpeedMax = engine.speedMax;
        engineFriction = engine.friction;
        rotationEngineForce = rotationEngine.force;
        rotationEngineSpeedMax = rotationEngine.speedMax;
        rotationEngineFriction = rotationEngine.friction;
    }

    private void Update()
    {
        if (!isGameMenu)
        {
            if (!isLockShoot && !isAbsoluteLockShoot)
            {
                if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
                {
                    Shoot(null);
                }
            }
            if (!isLockMove)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    if (currentFlightMode != FlightMode.Feather)
                    {
                        SwitchFlightMode(currentFlightMode, FlightMode.Feather);
                    }
                    else
                    {
                        SwitchFlightMode(currentFlightMode, FlightMode.Normal);
                    }
                }
                if (Input.GetKeyDown(KeyCode.V))
                {
                    if (currentFlightMode != FlightMode.Chain)
                    {
                        SwitchFlightMode(currentFlightMode, FlightMode.Chain);
                    }
                    else
                    {
                        SwitchFlightMode(currentFlightMode, FlightMode.Normal);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isGameMenu)
        {
            if (!isLockRotate)
            {
                horizontalInput = Input.GetAxis(AxisOptions.Horizontal.ToString());
                if (Input.GetMouseButton(1))
                {
                    plane.SetNormalAndPosition(localDown, transform.position);
                    ray = camera.ScreenPointToRay(Input.mousePosition);

                    if (plane.Raycast(ray, out hitDist))
                    {
                        v3 = ray.GetPoint(hitDist);
                    }
                    else
                    {
                        v3 = transform.position + transform.TransformPoint(forward);
                    }
                    rotationEngine.RotateToTargetWithPlaneLimiter(v3);
                }
                else if (Mathf.Abs(horizontalInput) > 0)
                {
                    rotationEngine.Rotate(horizontalInput);
                }
                else
                {
                    rotationEngine.SlowDown();
                }
            }
            if (!isLockMove)
            {
                verticalInput = Input.GetAxis(AxisOptions.Vertical.ToString());
                if (verticalInput > 0)
                {
                    engine.Move(verticalInput);
                }
                else if (verticalInput < 0)
                {
                    engine.Move(verticalInput / 2);
                }
                else
                {
                    engine.SlowDown();
                }
            }
        }
    }

    internal void SetLockControl(bool t)
    {
        SetLockControl(t, t, t);
    }
    internal void SetLockControl(bool isLockMove, bool isLockRotate, bool isLockShoot)
    {
        this.isLockMove = isLockMove;
        this.isLockRotate = isLockRotate;
        this.isLockShoot = isLockShoot;
        Guns.SetLockMove(isLockMove);
        engine.SetLockMove(isLockMove);
        rotationEngine.SetLockRotate(isLockRotate);

        if (isLockMove)
        {
            SwitchFlightMode(currentFlightMode, FlightMode.Normal);
        }
    }

    internal void GetPositionAndRotationTransforms(out Transform pos, out Transform rot)
    {
        pos = transform;
        rot = rotationTransform;
    }

    public void SetActiveCanvas(bool t)
    {
        canvas.SetActive(t);
    }


    private void SwitchFlightMode(FlightMode currentFlightMode, FlightMode newFlightMode)
    {
        if (currentFlightMode != newFlightMode)
        {
            switch (currentFlightMode)
            {
                case FlightMode.Feather:
                    FeatherActivatedImage.enabled = false;
                    break;
                case FlightMode.Normal:
                    StarshipActivatedImage.enabled = false;
                    break;
                case FlightMode.Chain:
                    ChainActivatedImage.enabled = false;
                    break;
            }

            switch (newFlightMode)
            {
                case FlightMode.Feather:
                    FeatherActivatedImage.enabled = true;
                    SetEngineCoefficients(0.65f, 1.6f, 0);
                    SetRotationEngineCoefficients(0.6f, 0.7f, 1.4f);
                    break;
                case FlightMode.Normal:
                    StarshipActivatedImage.enabled = true;
                    SetEngineCoefficients(1, 1, 1);
                    SetRotationEngineCoefficients(1, 1, 1);
                    break;
                case FlightMode.Chain:
                    ChainActivatedImage.enabled = true;
                    SetEngineCoefficients(1.5f, 0.75f, 1.3f);
                    SetRotationEngineCoefficients(1.4f, 1.05f, 1.3f);
                    break;
            }
            this.currentFlightMode = newFlightMode;
        }
    }

    private void SetEngineCoefficients(float forceC, float speedMaxC, float frictionC) => engine.SetParameters(engineForce * forceC, engineSpeedMax * speedMaxC, engineFriction * frictionC);
    private void SetRotationEngineCoefficients(float forceC, float speedMaxC, float frictionC) => rotationEngine.SetParameters(rotationEngineForce * forceC, rotationEngineSpeedMax * speedMaxC, rotationEngineFriction * frictionC);


    private enum FlightMode
    {
        Feather,
        Normal,
        Chain
    }


    private void OnMenuOpen() => isGameMenu = true;

    private void OnMenuClose() => isGameMenu = false;

}
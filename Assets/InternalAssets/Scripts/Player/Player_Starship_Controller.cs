using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Guns), typeof(Rigidbody))]
public class Player_Starship_Controller : MonoBehaviour
{

    [SerializeField] private bool isAbsoluteLockShoot = false;

    [SerializeField] private Image StarshipActivatedImage;
    [SerializeField] private Image FeatherActivatedImage;
    [SerializeField] private Image ChainActivatedImage;
    [SerializeField] private Image SwordActivatedImage;

    [SerializeField] private Transform rotationTransform;

    [SerializeField] private Starship_Engine engine;
    [SerializeField] private Starship_RotationEngine rotationEngine;

    [SerializeField] private GameObject canvas;

    [SerializeField] private ParticleSystem shockParticleSystem;

    private Camera Camera;

    private Guns Guns;
    private Guns.ShootEvent Shoot;

    private StarshipFlightModeSwitcher flightModeSwitcher;
    private FlightMode currentFlightMode => flightModeSwitcher.CurrentFlightMode;

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


    public delegate void MethodV3(Vector3 mouseV3);
    public event MethodV3 OnMouseOver;
    public delegate void Method();
    public event Method OnMouseOverEnd;

    private bool isMouseOver;
    private bool isLostControl;

    private void Awake()
    {
        Guns = GetComponent<Guns>();
        Shoot = Guns.GetShootEvent();

        flightModeSwitcher = GetComponent<StarshipFlightModeSwitcher>();

        StarshipActivatedImage.enabled = true;

        localUp = transform.TransformDirection(localUp);
        localDown = transform.TransformDirection(localDown);

        GetComponent<Starship>().LostControl = SetLostControl;

        GameMenu.OnMenuOpen += OnMenuOpen;
        GameMenu.OnMenuClose += OnMenuClose;
    }

    private void Start()
    {
        Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (!isGameMenu && !isLostControl)
        {
            if (!isLockShoot && !isAbsoluteLockShoot)
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
                    if (currentFlightMode != FlightMode.Feather)
                    {
                        SwitchFlightMode(currentFlightMode, FlightMode.Feather);
                    }
                    else
                    {
                        SwitchFlightMode(currentFlightMode, FlightMode.Normal);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.V))
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
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    if (currentFlightMode != FlightMode.Sword)
                    {
                        SwitchFlightMode(currentFlightMode, FlightMode.Sword);
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
        if (!isGameMenu && !isLostControl)
        {
            if (!isLockRotate)
            {
                horizontalInput = Input.GetAxis(AxisOptions.Horizontal.ToString());
                if (Input.GetMouseButton(1))
                {
                    plane.SetNormalAndPosition(localDown, transform.position);
                    ray = Camera.ScreenPointToRay(Input.mousePosition);

                    if (plane.Raycast(ray, out hitDist))
                    {
                        v3 = ray.GetPoint(hitDist);
                    }
                    else
                    {
                        v3 = transform.position + transform.TransformPoint(forward);
                    }
                    if (FlightMode.Sword == currentFlightMode)
                    {
                        isMouseOver = true;
                        OnMouseOver?.Invoke(v3);
                    }
                    rotationEngine.RotateToTargetWithPlaneLimiter(v3);
                }
                else if (isMouseOver)
                {
                    OnMouseOverEnd?.Invoke();
                    isMouseOver = false;
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

    public void SetLostControl(bool t) // Потеря управления
    {
        isLostControl = t;
        if (t)
        {
            engine.SlowDown();
            rotationEngine.SlowDown();
            shockParticleSystem.Play(true);
        }
        else
        {
            shockParticleSystem.Stop(true);
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

    public void SwitchFlightMode(FlightMode currentFlightMode, FlightMode newFlightMode)
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
                case FlightMode.Sword:
                    OnMouseOverEnd?.Invoke();
                    SwordActivatedImage.enabled = false;
                    break;
            }

            switch (newFlightMode)
            {
                case FlightMode.Feather:
                    FeatherActivatedImage.enabled = true;
                    break;
                case FlightMode.Normal:
                    StarshipActivatedImage.enabled = true;
                    break;
                case FlightMode.Chain:
                    ChainActivatedImage.enabled = true;
                    break;
                case FlightMode.Sword:
                    SwordActivatedImage.enabled = true;
                    break;
            }
            flightModeSwitcher.SwitchFlightMode(newFlightMode);
        }
    }

    private void OnMenuOpen() => isGameMenu = true;

    private void OnMenuClose() => isGameMenu = false;

}
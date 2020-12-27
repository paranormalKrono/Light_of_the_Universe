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

    private Camera camera;

    private Guns Guns;
    private Guns.ShootEvent Shoot;

    private bool isLockMove;
    private bool isLockRotate;
    private bool isLockShoot;
    private bool isGameMenu;

    private bool isLockFly = false;
    private bool isFreeFly = false;
    private Vector3 localUp = Vector3.up;
    private Vector3 localDown = Vector3.down;
    private Vector3 forward = Vector3.forward;

    private Vector3 v3;

    private float verticalInput;
    private float horizontalInput;

    private float hitDist;
    private Ray ray;
    private Plane plane = new Plane();


    private void Awake()
    {
        Guns = GetComponent<Guns>();
        Guns.Initialize(out Shoot);

        StarshipActivatedImage.enabled = true;

        localUp = transform.TransformDirection(localUp);
        localDown = transform.TransformDirection(localDown);

        GameMenu.OnMenuOpen += OnMenuOpen;
        GameMenu.OnMenuClose += OnMenuClose;
    }

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (!isGameMenu)
        {
            if (!isLockShoot && !isAbsoluteLockShoot)
            {
                if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
                {
                    Shoot();
                }
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                isLockFly = false;
                ChainActivatedImage.enabled = false;
                isFreeFly = !isFreeFly;
                if (isFreeFly)
                {
                    ActivateFeatherFly();
                    StarshipActivatedImage.enabled = false;
                }
                else
                {
                    ActivateNormalFly();
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
                    ActivateChainFly();
                    StarshipActivatedImage.enabled = false;
                }
                else
                {
                    ActivateNormalFly();
                    ChainActivatedImage.enabled = false;
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
                    rotationEngine.RotateToTarget(v3);
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
    }

    internal void GetPositionAndRotationTransforms(out Transform pos, out Transform rot)
    {
        pos = transform;
        rot = rotationTransform;
    }

    private void ActivateNormalFly()
    {
        StarshipActivatedImage.enabled = true;
        engine.SetCoefficients(1, 1, 1);
        rotationEngine.SetCoefficients(1, 1, 1);
    }
    private void ActivateFeatherFly()
    {
        FeatherActivatedImage.enabled = true;
        engine.SetCoefficients(0.65f, 1.6f, 0);
        rotationEngine.SetCoefficients(0.6f, 0.7f, 1.4f);
    }
    private void ActivateChainFly()
    {
        ChainActivatedImage.enabled = true;
        engine.SetCoefficients(1.5f, 0.75f, 1.3f);
        rotationEngine.SetCoefficients(1.4f, 1.05f, 1.3f);
    }


    private void OnMenuOpen() => isGameMenu = true;

    private void OnMenuClose() => isGameMenu = false;
}
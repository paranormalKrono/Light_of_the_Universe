using UnityEngine;

[RequireComponent (typeof (CharacterController))]
public class Player_Character_Controller : MonoBehaviour 
{

    //Параметры
    [SerializeField] private float StepSpeed = 3.0f;      // Скорость ходьбы
    [SerializeField] private float RunSpeed = 5.0f;       // Скорость бега
    [SerializeField] private float JumpSpeed = 3;         // Скорость прыжка
    [SerializeField] private float JumpToJumpTime = 1;    // Время от прыжка до прыжка
    [SerializeField] private float gravity = 1;           // Гравитация

    private CharacterController CharacterController;

    private Vector3 MoveDirection;

    private float Horizontal;
    private float Vertical;
    private float speed;
    private float JumpTimer;

    private bool isGameMenu;    // Открыто ли главное меню
    private bool isLockMove;

    private void Start()
    {
        JumpTimer = JumpToJumpTime;
        CharacterController = GetComponent<CharacterController>();

        GameMenu.OnMenuOpen += OnMenuOpen;
        GameMenu.OnMenuClose += OnMenuClose;

        //XSensitivity = StaticSettings.XSens; // Назначаем чуствительность мыши по вертикали
        //YSensitivity = StaticSettings.YSens; // Назначаем чуствительность мыши по горизонтали
    }

    private void Update()
    {

        if (!isGameMenu && !isLockMove)
        {
            Horizontal = Input.GetAxis("Horizontal"); // Назначаем движение по X
            Vertical = Input.GetAxis("Vertical"); // Назначаем движение по Z
        }
        else
        {
            Horizontal = 0;
            Vertical = 0;
        }

        if (Horizontal != 0 || Vertical != 0)
        {
            // Назначаем движение
            MoveDirection.x = Horizontal;
            MoveDirection.z = Vertical;

            MoveDirection = transform.TransformDirection(MoveDirection);
            //Если зажат Shift
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = RunSpeed;
            }
            else
            {
                speed = StepSpeed;
            }
            // Умножаем вектор на скорость движения
            MoveDirection.x *= speed;
            MoveDirection.z *= speed;
        }
        else
        {
            MoveDirection.x = 0;
            MoveDirection.z = 0;
        }
        if (CharacterController.isGrounded) // Если перонаж стоит на земле
        {
            MoveDirection.y = 0;
            if (JumpTimer < JumpToJumpTime) // Если время от последнего прыжка меньше чем от прыжка до прыжка, то прибавляем время
            {
                JumpTimer += Time.deltaTime;
            } 
            else if (Input.GetKey(KeyCode.Space)) // Если нажата клавиша Space и если персонаж не прыгнул
            {
                if (!isGameMenu) // Если движение не заблокировано
                {
                    JumpTimer = 0; // Время от последнего прыжка = 0
                    MoveDirection.y = JumpSpeed; // Скорость по y = скорости прыжка
                }
            }
        }
        else
        {
            MoveDirection.y -= gravity * Time.deltaTime;
        }
        CharacterController.Move(MoveDirection * Time.deltaTime); // Движение
    }


    internal void SetLockMove(bool t) => isLockMove = t;

    private void OnMenuOpen() => isGameMenu = true;

    private void OnMenuClose() => isGameMenu = false;
}
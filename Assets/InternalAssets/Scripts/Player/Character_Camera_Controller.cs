using UnityEngine;

public class Character_Camera_Controller : MonoBehaviour
{

    [SerializeField] private float Sensitivity = 1.6f;         // Чуствительность мыши
    [SerializeField] private float MinimumX = -90F;             // Минимум по вертикали
    [SerializeField] private float MaximumX = 90F;              // Максимум по вертикали

    [SerializeField] private bool clampVerticalRotation = true; // Есть ли ограничение по вертикали

    private Transform character_t; // Transform персонажа
    private Transform cam_t;       // Transform камеры

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    private float yRot;
    private float xRot;

    private bool isGameMenu;     // Открыто ли игровое меню
    private bool isLockLook;

    void Awake()
    {
        character_t = GetComponentInParent<CharacterController>().transform;
        cam_t = transform;

        GameMenu.OnMenuOpen += OnMenuOpen;
        GameMenu.OnMenuClose += OnMenuClose;
    }

    void Update()
    {

        if (!isGameMenu && !isLockLook)
        {
            m_CharacterTargetRot = character_t.localRotation; // Поворот игрока сейчас
            m_CameraTargetRot = cam_t.localRotation; // Поворот камеры сейчас

            //Запись движения мышки
            xRot = Input.GetAxis("Mouse X") * Sensitivity;
            yRot = Input.GetAxis("Mouse Y") * Sensitivity;

            if (clampVerticalRotation)
            {
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
            }

            //Присваивание значения поворота
            m_CharacterTargetRot *= Quaternion.Euler(0f, xRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-yRot, 0f, 0f);

            //Поворот
            character_t.localRotation = m_CharacterTargetRot;
            cam_t.localRotation = m_CameraTargetRot;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    internal void SetLockLook(bool t)
    {
        isLockLook = t;
    }

    public void SetSensitivity(float value) => Sensitivity = value;

    private void OnMenuOpen() => isGameMenu = true;

    private void OnMenuClose() => isGameMenu = false;
}
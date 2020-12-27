using UnityEngine;

public class Starship_Engine : MonoBehaviour
{
    [SerializeField] internal float force = 600;
    [SerializeField] private float speedMax = 18;
    [SerializeField] private float friction = 22;
    [SerializeField] private Transform RotationPointTransform;
    [SerializeField] private Rigidbody moveRigidbody;

    internal delegate void MethodV3();
    internal event MethodV3 OnSlowDown;
    internal delegate void Method(Vector3 direction);
    internal event Method OnMove;

    private bool isLockMove;

    private float forceCoefficient = 1;
    private float speedMaxCoefficient = 1;
    private float frictionCoefficient = 1;

    private readonly Vector3 forward = Vector3.forward;
    private readonly Vector3 vector3Zero = Vector3.zero;

    private void FixedUpdate()
    {
        if (isLockMove)
        {
            moveRigidbody.velocity = vector3Zero;
        }
    }

    internal void Move(float direction)
    {
        Move(forward * direction);
    }
    internal void Move(Vector3 direction)
    {
        OnMove?.Invoke(direction);
        direction = RotationPointTransform.TransformDirection(direction);
        moveRigidbody.AddForce(direction * Time.fixedDeltaTime * force * forceCoefficient, ForceMode.Impulse);
        moveRigidbody.velocity = Vector3.ClampMagnitude(moveRigidbody.velocity, speedMax * speedMaxCoefficient);
    }

    internal void SlowDown()
    {
        OnSlowDown?.Invoke();
        moveRigidbody.velocity = Vector3.Lerp(moveRigidbody.velocity, vector3Zero, Time.fixedDeltaTime * friction * frictionCoefficient);
    }

    internal void SetCoefficients(float forceCoefficient, float speedMaxCoefficient, float frictionCoefficient)
    {
        this.forceCoefficient = forceCoefficient;
        this.speedMaxCoefficient = speedMaxCoefficient;
        this.frictionCoefficient = frictionCoefficient;
    }

    internal void SetLockMove(bool isLockMove)
    {
        this.isLockMove = isLockMove;
        if (isLockMove)
        {
            OnSlowDown?.Invoke();
            moveRigidbody.velocity = vector3Zero;
        }
    }
}
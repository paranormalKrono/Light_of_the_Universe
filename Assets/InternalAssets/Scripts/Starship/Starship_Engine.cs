using UnityEngine;

public class Starship_Engine : MonoBehaviour
{
    [SerializeField] internal float force = 600;
    [SerializeField] internal float speedMax = 18;
    [SerializeField] internal float friction = 22;
    [SerializeField] private Transform EngineTransform;
    [SerializeField] private Transform RotationPointTransform;
    [SerializeField] private Rigidbody moveRigidbody;

    internal delegate void MethodV3();
    internal event MethodV3 OnSlowDown;
    internal delegate void Method(Vector3 direction);
    internal event Method OnMove;

    private bool isLockMove;

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
        MoveForce(-EngineTransform.forward * direction);
    }
    internal void Move(Vector3 direction)
    {
        MoveForce(RotationPointTransform.TransformDirection(direction));
    }

    private void MoveForce(Vector3 direction)
    {
        OnMove?.Invoke(direction);
        moveRigidbody.AddForce(direction * Time.fixedDeltaTime * force, ForceMode.Impulse);
        moveRigidbody.velocity = Vector3.ClampMagnitude(moveRigidbody.velocity, speedMax);
    }

    internal void SlowDown()
    {
        OnSlowDown?.Invoke();
        moveRigidbody.velocity = Vector3.Lerp(moveRigidbody.velocity, vector3Zero, Time.fixedDeltaTime * friction);
    }

    internal void SetParameters(float force, float speedMax, float friction)
    {
        this.force = force;
        this.speedMax = speedMax;
        this.friction = friction;
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
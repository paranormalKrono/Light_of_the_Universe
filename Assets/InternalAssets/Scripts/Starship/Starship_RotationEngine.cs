using UnityEngine;

public class Starship_RotationEngine : MonoBehaviour
{
    [SerializeField] internal float force = 7000;
    [SerializeField] private float speedMax = 3.6f;
    [SerializeField] private float friction = 105;
    [SerializeField] private Rigidbody rotationRigidbody;

    private Transform RotationPointTr;

    private float forceCoefficient = 1;
    private float speedMaxCoefficient = 1;
    private float frictionCoefficient = 1;

    private bool isLockRotate;

    private readonly Vector3 vector3Zero = Vector3.zero;
    private Vector3 rotateDirection;

    private float angle;
    private Vector3 rotationVector;

    private void Awake()
    {
        RotationPointTr = rotationRigidbody.transform;
    }

    private void FixedUpdate()
    {
        if (isLockRotate)
        {
            rotationRigidbody.angularVelocity = vector3Zero;
        }
    }

    internal void Rotate(float direction)
    {
        rotateDirection.Set(0, direction, 0);
        Rotate(transform.TransformDirection(rotateDirection));
    }
    internal void Rotate(Vector3 direction)
    {
        AddRotateTorque(direction * force);
        LimitRotateSpeed(speedMax * speedMaxCoefficient);
    }

    internal void RotateToTarget(Vector3 target)
    {
        angle = Vector3.Angle((target - transform.position).normalized, RotationPointTr.forward);
        rotationVector = -Vector3.Cross((target - transform.position).normalized, RotationPointTr.forward).normalized;

        if (angle < 10)
        {
            LimitRotateSpeed(angle / 20);
        }
        else
        {
            LimitRotateSpeed(speedMax * speedMaxCoefficient * 1.2f);
        }

        if (angle < 15)
        {
            AddRotateTorque(rotationVector * force * forceCoefficient * angle / 15);
        }
        else
        {
            AddRotateTorque(rotationVector * force * forceCoefficient);
        }
    }

    private void AddRotateTorque(Vector3 torque) => rotationRigidbody.AddTorque(torque * Time.fixedDeltaTime, ForceMode.Acceleration);
    private void LimitRotateSpeed(float limiter) => rotationRigidbody.angularVelocity = Vector3.ClampMagnitude(rotationRigidbody.angularVelocity, limiter);

    internal void SlowDown() => rotationRigidbody.angularVelocity = Vector3.MoveTowards(rotationRigidbody.angularVelocity, vector3Zero, friction * frictionCoefficient * Time.fixedDeltaTime);

    internal void SetCoefficients(float forceCoefficient, float speedMaxCoefficient, float frictionCoefficient)
    {
        this.forceCoefficient = forceCoefficient;
        this.speedMaxCoefficient = speedMaxCoefficient;
        this.frictionCoefficient = frictionCoefficient;
    }

    internal void SetLockRotate(bool isLockRotate)
    {
        this.isLockRotate = isLockRotate;
        if (isLockRotate)
        {
            rotationRigidbody.angularVelocity = vector3Zero;
        }
    }
}

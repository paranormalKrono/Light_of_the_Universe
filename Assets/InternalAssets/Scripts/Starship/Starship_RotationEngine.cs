using UnityEngine;

public class Starship_RotationEngine : MonoBehaviour
{
    [SerializeField] internal float force = 7000;
    [SerializeField] internal float speedMax = 3.6f;
    [SerializeField] internal float friction = 105;
    [SerializeField] private Rigidbody rotationRigidbody;
    private Transform RotationPointTr;

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
        LimitRotateSpeed(speedMax);
    }

    internal void RotateToTargetWithPlaneLimiter(Vector3 target)
    {
        angle = AngleToTarget(target);
        rotationVector = -Vector3.Cross((target - transform.position).normalized, RotationPointTr.forward).normalized;

        AddRotateTorqueConsideringAngle();

        LimitRotateSpeedConsideringAngle();
    }

    internal void RotateToTarget(Vector3 target)
    {
        angle = AngleToTarget(target);
        rotationVector = -Vector3.Cross((target - transform.position).normalized, RotationPointTr.forward).normalized;

        AddRotateTorqueConsideringAngle();

        LimitRotateSpeedConsideringAngle();
    }

    


    private float AngleToTarget(Vector3 Target) => Vector3.Angle((Target - transform.position).normalized, RotationPointTr.forward);

    private void AddRotateTorqueConsideringAngle()
    {
        if (angle < 15)
        {
            AddRotateTorque(rotationVector * force * angle / 15);
        }
        else
        {
            AddRotateTorque(rotationVector * force);
        }
    }
    private void AddRotateTorque(Vector3 torque) => rotationRigidbody.AddTorque(torque * Time.fixedDeltaTime, ForceMode.Acceleration); 
    
    private void LimitRotateSpeedConsideringAngle()
    {
        if (angle < 10)
        {
            LimitRotateSpeed(angle / 20);
        }
        else
        {
            LimitRotateSpeed(speedMax * 1.2f);
        }
    }
    private void LimitRotateSpeed(float limiter) => rotationRigidbody.angularVelocity = Vector3.ClampMagnitude(rotationRigidbody.angularVelocity, limiter);

    internal void SlowDown() => rotationRigidbody.angularVelocity = Vector3.MoveTowards(rotationRigidbody.angularVelocity, vector3Zero, friction * Time.fixedDeltaTime);

    internal void SetParameters(float force, float speedMax, float friction)
    {
        this.force = force;
        this.speedMax = speedMax;
        this.friction = friction;
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

using System.Collections;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private float upTime = 1;
    [SerializeField] private float boostTime = 2;
    [SerializeField] private float distanceToTarget = 2;
    [SerializeField] private float timeToDestroy = 30;
    [SerializeField] private float magnitudeToStartFly = 0.1f;
    [SerializeField] private float angularMagnitudeToStartFly = 0.1f;
    [SerializeField] private float angleToTarget = 0.2f;
    [SerializeField] private Rigidbody rocketRigidbody;
    [SerializeField] private PropExplosion propExplosion;
    [SerializeField] private Starship_Engine engine;
    [SerializeField] private Starship_RotationEngine rotationEngine;
    [SerializeField] private LayerMask obstacleLayer;

    private Vector3 target;

    public void Initialise(Transform Target)
    {
        if (Target != null)
        {
            target = Target.position;
        }
        else
        {
            target = transform.position;
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();
        StartCoroutine(IDestroy());
        while (upTime > 0)
        {
            upTime -= Time.fixedDeltaTime;
            engine.Move(1);
            yield return new WaitForFixedUpdate();
        }
        while (GetAngleToTarget() > angleToTarget)
        {
            if (rocketRigidbody.velocity.magnitude > magnitudeToStartFly)
            {
                engine.SlowDown();
            }
            rotationEngine.RotateToTarget(target);
            yield return new WaitForFixedUpdate();
        }
        while (rocketRigidbody.angularVelocity.magnitude > angularMagnitudeToStartFly)
        {
            rotationEngine.SlowDown();
            yield return new WaitForFixedUpdate();
        }
        while (boostTime > 0)
        {
            boostTime -= Time.fixedDeltaTime;
            engine.Move(1);
            yield return new WaitForFixedUpdate();
        }
        while (Vector3.Distance(transform.position, target) > distanceToTarget)
        {
            yield return new WaitForFixedUpdate();
        }
        while (Physics.Raycast(transform.position, (target - transform.position).normalized, Vector3.Distance(target, transform.position), obstacleLayer))
        {
            yield return new WaitForFixedUpdate();
        }
        propExplosion.BlowUp();
    }
    private IEnumerator IDestroy()
    {
        yield return new WaitForSeconds(timeToDestroy);
        propExplosion.BlowUp();
    }

    private float GetAngleToTarget() => Vector3.Angle((target - transform.position).normalized, transform.forward);
}
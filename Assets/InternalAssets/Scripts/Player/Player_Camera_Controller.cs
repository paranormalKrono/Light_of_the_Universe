using UnityEngine;
using static UnityEngine.Physics;

public class Player_Camera_Controller : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float velocityKoef = 0.25f;
    [SerializeField] private float minDistanceToObstacle = 1f;
    [SerializeField] internal Vector3 offset;
    private Transform starshipTr;
    private Rigidbody starshipRb;
    private Transform targetTr;
    private bool isLockMove = false;
    private bool isTargetMove = false;

    private RaycastHit hitInfo;

    private void Awake()
    {
        starshipTr = GameObject.FindGameObjectWithTag("Player").transform;
        starshipRb = starshipTr.GetComponent<Rigidbody>();
        starshipTr.GetComponent<Health>().OnDeath += PlayerDead;
    }

    void Update()
    {
        if (!isLockMove)
        {
            if (SphereCast(starshipTr.position + offset, minDistanceToObstacle, starshipRb.velocity.normalized, out hitInfo, Vector3.Magnitude(starshipRb.velocity * velocityKoef)))
            {
                //Debug.DrawLine(starshipTr.position + offset, starshipTr.position + offset + Vector3.ClampMagnitude(starshipRb.velocity * velocityKoef, hitInfo.distance) + Vector3.up, Color.green, 0.1f); // newCameraPosition
                //Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.blue, 0.1f); // cameraPosition
                //Debug.DrawLine(starshipTr.position + offset, starshipTr.position + offset + Vector3.up, Color.red, 0.1f); // StarshipPosition + offset
                transform.position = Vector3.Lerp(transform.position, starshipTr.position + offset + Vector3.ClampMagnitude(starshipRb.velocity * velocityKoef, hitInfo.distance), Time.deltaTime * moveSpeed);
            }
            else if (Raycast(starshipTr.position + offset, starshipRb.velocity.normalized, minDistanceToObstacle))
            {
                transform.position = Vector3.Lerp(transform.position, starshipTr.position + offset, Time.deltaTime * moveSpeed);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, starshipTr.position + offset + starshipRb.velocity * velocityKoef, Time.deltaTime * moveSpeed);
            }
        }
        else if (isTargetMove)
        {
            transform.position = Vector3.Lerp(transform.position, targetTr.position, Time.deltaTime * moveSpeed);
        }
    }
    private void PlayerDead() => SetLockMove(true);

    internal void SetLockMove(bool t) => isLockMove = t;
    internal void SetPositionWithOffset(Vector3 position) => transform.position = position + offset;

    internal void UpdatePlayerLookPosition() => transform.position = starshipTr.position + offset;

    internal void DisableTargetMove()
    {
        SetLockMove(false);
        isTargetMove = false;
    }
    internal void EnableTargetMove(Transform Tr)
    {
        isTargetMove = true;
        targetTr = Tr;
        SetLockMove(true);
    }
}
using UnityEngine;
using static UnityEngine.Physics;

public class Player_Camera_Controller : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float velocityKoef = 0.25f;
    [SerializeField] private float minDistanceToObstacle = 1f;
    [SerializeField] internal Vector3 offset;
    [SerializeField] private float offsetModifierRange = 8;
    [SerializeField] private float ScrollWheelSpeed = 120;
    [SerializeField] private LayerMask offsetMask;

    private Transform starshipTr;
    private Rigidbody starshipRb;
    private Transform targetTr;
    private bool isLockMove = false;
    private bool isTargetMove = false;

    private Vector3 currentOffset;
    private float currentOffsetModifier = 0;

    private RaycastHit hitInfo;

    private void Awake()
    {
        starshipTr = GameObject.FindGameObjectWithTag("Player").transform;
        starshipRb = starshipTr.GetComponent<Rigidbody>();
        starshipTr.GetComponent<Health>().OnDeath += PlayerDead;
        currentOffset = offset;
    }

    private float mouseScrollWheel;
    private Vector3 newOffset;

    private void Update()
    {
        if (!isLockMove)
        {
            mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (mouseScrollWheel != 0)
            {
                if (mouseScrollWheel < 0)
                {
                    if (currentOffsetModifier + Time.deltaTime * ScrollWheelSpeed < offsetModifierRange)
                    {
                        currentOffsetModifier += Time.deltaTime * ScrollWheelSpeed;
                    }
                    else
                    {
                        currentOffsetModifier = offsetModifierRange;
                    }
                }
                else if (mouseScrollWheel > 0)
                {
                    if (currentOffsetModifier - Time.deltaTime * ScrollWheelSpeed > -offsetModifierRange)
                    {
                        currentOffsetModifier -= Time.deltaTime * ScrollWheelSpeed;
                    }
                    else
                    {
                        currentOffsetModifier = -offsetModifierRange;
                    }
                }
                currentOffset.y = offset.y + currentOffsetModifier;
            }
            if (Raycast(starshipTr.position, currentOffset.normalized, out hitInfo, currentOffset.y + 1, offsetMask))
            {
                newOffset = starshipTr.position;
                newOffset.y = hitInfo.point.y - 1;
                transform.position = newOffset;
                newOffset = currentOffset;
                newOffset.y = hitInfo.point.y - 1;
            }
            else
            {
                newOffset = currentOffset;
            }
            if (SphereCast(starshipTr.position + newOffset, minDistanceToObstacle, starshipRb.velocity.normalized, out hitInfo, Vector3.Magnitude(starshipRb.velocity * velocityKoef)))
            {
                transform.position = Vector3.Lerp(transform.position, starshipTr.position + newOffset + Vector3.ClampMagnitude(starshipRb.velocity * velocityKoef, hitInfo.distance), Time.deltaTime * moveSpeed);
            }
            else if (Raycast(starshipTr.position + newOffset, starshipRb.velocity.normalized, minDistanceToObstacle))
            {
                transform.position = Vector3.Lerp(transform.position, starshipTr.position + newOffset, Time.deltaTime * moveSpeed);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, starshipTr.position + newOffset + starshipRb.velocity * velocityKoef, Time.deltaTime * moveSpeed);
            }
        }
        else if (isTargetMove)
        {
            transform.position = Vector3.Lerp(transform.position, targetTr.position, Time.deltaTime * moveSpeed);
        }
    }
    private void PlayerDead() => SetLockMove(true);

    internal void SetLockMove(bool t) => isLockMove = t;
    internal void SetPositionWithOffset(Vector3 position)
    {
        currentOffset = offset;
        transform.position = position + offset;
    }

    internal void UpdatePlayerLookPosition()
    {
        currentOffset = offset;
        transform.position = starshipTr.position + offset;
    }

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
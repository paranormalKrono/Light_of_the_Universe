using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] private float LiftMoveSpeed = 6;
    [SerializeField] private float starshipMoveSpeed = 10;
    [SerializeField] private float starshipRotateSpeed = 80;
    [SerializeField] private Transform liftTr;
    [SerializeField] private Transform endTr;
    [SerializeField] private Transform liftStarshipTr;
    private Transform playerPositionTr;
    private Transform playerRotationTr;
    private Player_Starship_Controller player_Controller;
    private bool isActivated;
    private bool isStarshipMove;
    private bool isLiftMove;

    private void Update()
    {
        if (isActivated)
        {
            if (isStarshipMove)
            {
                if (Quaternion.Angle(liftStarshipTr.rotation, playerRotationTr.rotation) < 1 && Vector3.Distance(liftStarshipTr.position, playerPositionTr.position) < 0.01f)
                {
                    isStarshipMove = false;
                    isLiftMove = true;
                }
                else
                {
                    playerPositionTr.position = Vector3.MoveTowards(playerPositionTr.position, liftStarshipTr.position, Time.deltaTime * starshipMoveSpeed);
                    playerRotationTr.rotation = Quaternion.RotateTowards(playerRotationTr.rotation, liftStarshipTr.rotation, Time.deltaTime * starshipRotateSpeed);
                }
            }
            if (isLiftMove)
            {
                if (Vector3.Distance(liftTr.position, endTr.position) < 0.01f)
                {
                    isLiftMove = false;
                    player_Controller.SetLockControl(false);
                }
                else
                {
                    liftTr.position = Vector3.MoveTowards(liftTr.position, endTr.position, Time.deltaTime * LiftMoveSpeed);
                    playerPositionTr.position = liftStarshipTr.position;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated)
        {
            player_Controller = other.gameObject.GetComponentInParent<Player_Starship_Controller>();
            if (player_Controller != null)
            {
                player_Controller.GetPositionAndRotationTransforms(out playerPositionTr, out playerRotationTr);
                player_Controller.SetLockControl(true);
                isStarshipMove = true;
                isActivated = true;
            }
        }
    }
}

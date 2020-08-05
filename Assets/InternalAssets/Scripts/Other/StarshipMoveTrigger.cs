using System.Collections;
using UnityEngine;

public class StarshipMoveTrigger : MonoBehaviour
{
    [SerializeField] private float starshipMoveSpeed = 10;
    [SerializeField] private float starshipRotateSpeed = 80;
    [SerializeField] private Transform startTr;
    [SerializeField] private Transform endTr;
    [SerializeField] internal bool isTrigger;
    private Transform playerPositionTr;
    private Transform playerRotationTr;
    private Player_Starship_Controller playerController;
    private Player_Camera_Controller playerCamera;
    private bool isActivated;

    internal delegate void TriggerDelegate();
    internal TriggerDelegate TriggerEvent;


    private void Awake()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        IMove = IStarshipMove();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger && !isActivated && other.GetComponentInParent<Player_Starship_Controller>() != null)
        {
            playerController = other.GetComponentInParent<Player_Starship_Controller>();
            playerController.GetPositionAndRotationTransforms(out playerPositionTr, out playerRotationTr);
            SetStarshipLock(true);
            StartCoroutine(IMove);
            TriggerEvent();
        }
    }

    internal void StartMove(Player_Starship_Controller playerController)
    {
        playerController.GetPositionAndRotationTransforms(out playerPositionTr, out playerRotationTr);
        this.playerController = playerController;
        playerPositionTr.position = startTr.position;
        playerRotationTr.rotation = endTr.rotation;
        playerCamera.transform.position = endTr.position + playerCamera.offset;
        StartCoroutine(IMove);
    }

    internal void StopMove()
    {
        StopCoroutine(IMove);
        SetStarshipLock(false);
    }
    internal void MoveInEndPosition(Player_Starship_Controller playerController)
    {
        playerController.GetPositionAndRotationTransforms(out playerPositionTr, out playerRotationTr);
        this.playerController = playerController;
        playerPositionTr.position = endTr.position;
        playerRotationTr.rotation = endTr.rotation;
        playerCamera.transform.position = endTr.position + playerCamera.offset;
    }

    private IEnumerator IMove;
    private IEnumerator IStarshipMove()
    {
        isActivated = true;
        while (Vector3.Distance(playerPositionTr.position, endTr.position) > 0.1f)
        {
            playerPositionTr.position = Vector3.MoveTowards(playerPositionTr.position, endTr.position, Time.deltaTime * starshipMoveSpeed);
            playerRotationTr.rotation = Quaternion.RotateTowards(playerRotationTr.rotation, endTr.rotation, Time.deltaTime * starshipRotateSpeed);
            yield return null;
        }
        isActivated = false;
    }
    private void SetStarshipLock(bool t)
    {
        playerController.SetLockControl(t);
        playerCamera.SetLockMove(t);
    }
}
using UnityEngine;

public class Event_Targets : MonoBehaviour
{
    [SerializeField] private Target[] targets;
    [SerializeField] private Transform DoorTr;
    [SerializeField] private Transform DoorEndTr;
    [SerializeField] private Transform PlayerEndTr;
    [SerializeField] private Transform CameraTr;
    [SerializeField] private float doorSpeed = 10;
    [SerializeField] private float playerSpeed = 14;
    private Transform PlayerStarshipTr;
    private Player_Starship_Controller player_Controller;
    private Player_Camera_Controller mainCamera;
    private int disabledTargets;
    private bool isOpenDoor;
    private bool isActivated;
    private bool isMovePlayer;

    void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        disabledTargets = targets.Length;
        for (int i = 0; i < targets.Length;++i)
        {
            targets[i].TargetEvent = TargetHit;
        }
    }
    private void Update()
    {
        if (isMovePlayer)
        {
            PlayerStarshipTr.position = Vector3.MoveTowards(PlayerStarshipTr.position, PlayerEndTr.position, Time.deltaTime * playerSpeed);
            if (PlayerStarshipTr.position == PlayerEndTr.position)
            {
                isMovePlayer = false;
            }
        }
        if (isOpenDoor)
        {
            DoorTr.position = Vector3.MoveTowards(DoorTr.position, DoorEndTr.position, Time.deltaTime * doorSpeed);
            if (DoorTr.position == DoorEndTr.position)
            {
                isOpenDoor = false;
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
                isActivated = true; 
                NextStage();
                PlayerStarshipTr = other.transform.parent;
                player_Controller.SetLockControl(true, false, false);
                isMovePlayer = true;
                mainCamera.EnableTargetMove(CameraTr);
            }
        }
    }
    private void TargetHit()
    {
        disabledTargets -= 1;
        if (disabledTargets == 0)
        {
            player_Controller.SetLockControl(false, false, false);
            mainCamera.DisableTargetMove();
            isOpenDoor = true;
            NextStage();
        }
    }
    private void NextStage()
    {
        GameDialogs.NextInGameDialogEvent();
        GameGoals.NextGoalEvent();
    }
}
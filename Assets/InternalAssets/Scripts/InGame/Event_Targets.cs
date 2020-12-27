using UnityEngine;

public class Event_Targets : MonoBehaviour
{
    [SerializeField] private Target[] targets;
    [SerializeField] private Door ExitDoor;
    [SerializeField] private PlayerStarshipTrigger playerTrigger;
    [SerializeField] private Transform PlayerEndTr;
    [SerializeField] private Transform CameraTr;
    [SerializeField] private float playerSpeed = 14;
    private Transform PlayerStarshipTr;
    private Player_Starship_Controller player_Controller;
    private Player_Camera_Controller mainCamera;

    private int disabledTargets;
    private bool isMovePlayer;

    public delegate void Method();
    public event Method OnStart;
    public event Method OnEnd;

    void Awake()
    {
        player_Controller = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Starship_Controller>();
        PlayerStarshipTr = player_Controller.transform;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        disabledTargets = targets.Length;
        playerTrigger.OnPlayerStarshipEnter += StartTarget;
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
        }
    }
    private void StartTarget()
    {
        playerTrigger.OnPlayerStarshipEnter -= StartTarget;
        OnStart?.Invoke();
        player_Controller.SetLockControl(true, false, false);
        isMovePlayer = true;
        mainCamera.EnableTargetMove(CameraTr);
    }
    private void TargetHit()
    {
        disabledTargets -= 1;
        if (disabledTargets == 0)
        {
            player_Controller.SetLockControl(false, false, false);
            mainCamera.DisableTargetMove();
            ExitDoor.Move();
            isMovePlayer = false;
            OnEnd?.Invoke();
        }
    }
}
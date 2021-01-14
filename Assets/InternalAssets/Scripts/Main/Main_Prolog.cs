using System.Collections;
using UnityEngine;

public class Main_Prolog : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] internal StarshipMover MoverStart; // Скрипт в начале для движения корабля игрока
    [SerializeField] internal StarshipMover MoverEnd; // Скрипт в конце для движения корабля игрока
    [SerializeField] internal PlayerStarshipTrigger EndTrigger; // Триггер в конце 

    protected Player_Starship_Controller player_Starship_Controller;
    protected Player_Camera_Controller player_Camera_Controller;
    private Starship playerStarship;

    private bool isEnd;
    private bool isRestart;

    private void Awake()
    {
        GameManager.Initialize();

        player_Camera_Controller = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        player_Starship_Controller = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Player_Starship_Controller>();
        playerStarship = player_Starship_Controller.GetComponent<Starship>();
        player_Starship_Controller.GetComponent<Health>().OnDeath += Restart;

        GameText.DeactivateEvent();
        GameText.SetInGameTextNowEvent(textAsset);

        GameAudio.StartAudioEvent(audioClip, 0.7f, true);
        if (StaticSettings.isRestart)
        {
            isRestart = true;
            StaticSettings.isRestart = false;
        }

        EndTrigger.OnPlayerStarshipEnter += OnPlayerStarshipEnterEndTrigger;
    }
    private void Start()
    {
        MStart();
        Checkpoint(StaticSettings.checkpointID);

        if (!isRestart && StaticSettings.checkpointID == 0)
        {
            player_Starship_Controller.SetLockControl(true);
            player_Starship_Controller.SetActiveCanvas(false);
            player_Camera_Controller.SetPositionWithOffset(MoverStart.GetEndPosition());

            MoverStart.Move(playerStarship);

            GameDialogs.StartDialogEvent(EndDialogEvent);
        }
        else
        {
            isRestart = false;
            GameGoals.SetActiveGoalEvent(true);
            if (StaticSettings.checkpointID == 0)
            {
                MoverStart.ToEndPosition(playerStarship);
            }
        }
    }

    protected virtual void MStart() { }
    protected virtual void Checkpoint(int checkpointID) { }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            EndEvent();
        }
    }

    private void OnPlayerStarshipEnterEndTrigger()
    {
        player_Starship_Controller.SetLockControl(true);
        player_Camera_Controller.SetLockMove(true);
        MoverEnd.MoveLine(playerStarship);
        EndEvent();
    }

    protected void EndEvent()
    {
        if (!isRestart && !isEnd)
        {
            isEnd = true;
            StaticSettings.checkpointID = 0;
            SceneController.LoadNextStoryScene();
        }
    }

    protected void SetCheckpoint(int checkpointID)
    {
        StaticSettings.checkpointID = checkpointID;
    }
    
    protected void MovePlayerToCheckpoint(Transform CheckpointTr) => player_Starship_Controller.transform.SetPositionAndRotation(CheckpointTr.position, CheckpointTr.rotation);

    protected void Restart()
    {
        if (!isRestart && !isEnd)
        {
            isRestart = true;
            StaticSettings.isRestart = true;
            SceneController.RestartScene();
        }
    }

    private void EndDialogEvent()
    {
        GameGoals.SetActiveGoalEvent(true);
        player_Starship_Controller.SetLockControl(false);
        player_Starship_Controller.SetActiveCanvas(true);
        player_Camera_Controller.SetLockMove(false);
        MoverStart.StopMove();
    }
}
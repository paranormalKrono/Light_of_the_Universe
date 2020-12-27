using System.Collections;
using UnityEngine;

public class Main_Prolog : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] internal PlayerStarshipMover MoverStart; // Скрипт в начале для движения корабля игрока
    [SerializeField] internal PlayerStarshipMover MoverEnd; // Скрипт в конце для движения корабля игрока
    [SerializeField] internal PlayerStarshipTrigger EndTrigger; // Триггер в конце 

    protected Player_Starship_Controller player_Starship_Controller;
    protected Player_Camera_Controller player_Camera_Controller;

    private bool isEnd;
    private bool isRestart;

    private void Awake()
    {
        GameManager.Initialize();
        GameScreenDark.SetDarkEvent(true);

        player_Camera_Controller = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        player_Starship_Controller = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Player_Starship_Controller>();
        player_Starship_Controller.GetComponent<Health>().OnDeath += Restart;

        GameText.DeactivateEvent();
        GameText.SetInGameTextNowEvent(textAsset);

        if (!StaticSettings.isRestart)
        {
            GameAudio.StartAudioEvent(audioClip, true);
        }
        else
        {
            isRestart = true;
            StaticSettings.isRestart = false;
        }

        EndTrigger.OnPlayerStarshipEnter += OnPlayerStarshipEnterEndTrigger;
    }
    private IEnumerator Start()
    {
        MStart();
        Checkpoint(StaticSettings.checkpointID);

        if (!isRestart && StaticSettings.checkpointID == 0)
        {
            player_Starship_Controller.SetLockControl(true);
            player_Camera_Controller.SetLockMove(true);
            player_Camera_Controller.SetPositionWithOffset(MoverStart.GetEndPosition());

            MoverStart.Move(player_Starship_Controller);

            GameDialogs.StartDialogEvent(EndDialogEvent);
            yield return StartCoroutine(GameScreenDark.ITransparentEvent());
        }
        else
        {
            isRestart = false;
            GameGoals.SetActiveGoalEvent(true);
            if (StaticSettings.checkpointID == 0)
            {
                MoverStart.StarshipToEndPosition(player_Starship_Controller);
            }
            yield return StartCoroutine(GameScreenDark.ITransparentEvent());
        }
    }

    protected virtual void MStart() { }
    protected virtual void Checkpoint(int checkpointID) { }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (!isEnd)
            {
                StartCoroutine(IEndEvent());
            }
        }
    }

    private void OnPlayerStarshipEnterEndTrigger()
    {
        player_Starship_Controller.SetLockControl(true);
        player_Camera_Controller.SetLockMove(true);
        MoverEnd.MoveLine(player_Starship_Controller);
        EndEvent();
    }

    protected void EndEvent()
    {
        if (!isRestart && !isEnd)
        {
            StartCoroutine(IEndEvent());
        }
    }
    private IEnumerator IEndEvent()
    {
        isEnd = true;
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        GameAudio.StopAudioEvent();
        StaticSettings.checkpointID = 0;
        SceneController.LoadNextStoryScene();
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
            StartCoroutine(IRestart());
        }
    }
    private IEnumerator IRestart()
    {
        isRestart = true;
        StaticSettings.isRestart = true;
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        SceneController.RestartScene();
    }

    private void EndDialogEvent()
    {
        GameGoals.SetActiveGoalEvent(true);
        player_Starship_Controller.SetLockControl(false);
        player_Camera_Controller.SetLockMove(false);
        MoverStart.StopMove();
    }
}
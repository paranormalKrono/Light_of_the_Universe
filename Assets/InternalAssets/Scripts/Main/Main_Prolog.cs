using System.Collections;
using UnityEngine;

public class Main_Prolog : MonoBehaviour
{
    [SerializeField] private Scenes nextScene;
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private int nextSlidesID;
    [SerializeField] private int nextEquationsID;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] internal StarshipMoveTrigger starshipMoveStart; // Скрипт в начале для движения корабля игрока
    [SerializeField] internal StarshipMoveTrigger starshipMoveEnd; // Скрипт в конце для движения корабля игрока

    private Player_Starship_Controller playerController;
    private Player_Camera_Controller playerCamera;

    private bool isEnd;
    private bool isRestart;

    private void Awake()
    {
        GameManager.Initialize();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<Player_Starship_Controller>();
        playerController.GetComponent<Health>().DeathEvent += Restart;

        GameText.DeactivateEvent();
        GameText.SetInGameTextNowEvent(textAsset);

        if (!StaticSettings.isRestart)
        {
            GameAudio.StartAudioEvent(audioClip, true);
        }
        else
        {
            StaticSettings.isRestart = false;
        }
    }
    private IEnumerator Start()
    {
        starshipMoveStart.isTrigger = false;
        starshipMoveEnd.isTrigger = true;
        starshipMoveEnd.TriggerEvent = EndEvent;

        if (!isRestart)
        {
            playerController.SetLockControl(true);
            playerCamera.SetLockMove(true);
            starshipMoveStart.StartMove(playerController);
            yield return StartCoroutine(ScreenDark.ITransparentEvent());
            GameDialogs.StartDialogEvent(EndDialogEvent);
        }
        else
        {
            StaticSettings.isRestart = false;
            GameGoals.SetActiveGoalEvent(true);
            starshipMoveStart.MoveInEndPosition(playerController);
            yield return StartCoroutine(ScreenDark.ITransparentEvent());
        }
    }

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

    private void EndEvent()
    {
        if (!isRestart && !isEnd)
        {
            StartCoroutine(IEndEvent());
        }
    }
    private IEnumerator IEndEvent()
    {
        isEnd = true;
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        if (nextScene == Scenes.Menu)
        {
            StaticSettings.isCompanyMenu = true;
            StaticSettings.isPart1Complete = true;
        }
        GameAudio.StopAudioEvent();
        SceneController.LoadEquationsAndSlides(nextScene, nextEquationsID, nextSlidesID);
    }
    private void Restart()
    {
        if (!isRestart && !isEnd)
        {
            StartCoroutine(IRestart());
        }
    }
    private IEnumerator IRestart()
    {
        isRestart = true;
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        SceneController.RestartScene();
    }
    private void EndDialogEvent()
    {
        GameGoals.SetActiveGoalEvent(true);
        starshipMoveStart.StopMove();
    }
}
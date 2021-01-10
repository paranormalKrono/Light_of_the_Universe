using System.Collections;
using UnityEngine;

public class Main_End : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private ScenesLocations sceneLocationName;
    [SerializeField] private AudioClip audioClip;

    [SerializeField] private StarshipMover MoverStartPlayer;
    [SerializeField] private StarshipMover MoverStartZ2;
    [SerializeField] private StarshipMover MoverEnd;

    [SerializeField] private PlayerStarshipTrigger EndTrigger;

    [SerializeField] private Starship_AI_Adv Z2StarshipAI;
    [SerializeField] private Starship Z2Starship;

    [SerializeField] private GameObject Z2Image;
    [SerializeField] private GameObject Z5Image;

    [SerializeField] private Transform cameraTarget;

    [SerializeField] private Transform Destination;

    [SerializeField] private PlayerStarshipTrigger playerTrigger;

    [SerializeField] private Animator Z5animator;

    [SerializeField] private Event_CruiserAttack cruiserAttack;
    [SerializeField] private Door ExitDoor;
    [SerializeField] private GameObject FrigateHealth;


    private System_Starships systemStarships;
    private Player_Starship_Controller playerController;
    private Player_Camera_Controller playerCamera;
    private Starship playerStarship;

    private Transform PlayerStarshipTr;

    private bool isRestart;
    private bool isEnd;
    private bool isSayed;
    private bool isBattle;
    private bool isPlayerDead;


    private void Awake()
    {
        GameManager.Initialize();
        GameScreenDark.SetDarkEvent(true);

        SceneController.LoadAdditiveScene(sceneLocationName);
        GameText.DeactivateEvent();
        GameText.SetInGameTextNowEvent(textAsset);

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Starship_Controller>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        playerStarship = playerController.GetComponent<Starship>();
        playerController.GetComponent<Health>().OnDeath += OnPlayerDeath;
        systemStarships = GetComponent<System_Starships>();

        PlayerStarshipTr = playerController.transform;

        EndTrigger.OnPlayerStarshipEnter += PlayerStarshipEnterEndTrigger;
    }

    private IEnumerator Start()
    {
        systemStarships.InitializeStarshipsTeams(GetComponent<StarshipsSpawnMover>().MoveStarshipsOnSpawns());

        if (!StaticSettings.isRestart)
        {
            GameAudio.StartAudioEvent(audioClip, 0.25f, true);

            playerController.SetLockControl(true);
            playerCamera.SetLockMove(false);
            systemStarships.SetStarshipsLock(true);

            MoverStartPlayer.ToStartPosition(playerStarship);

            playerCamera.SetPositionWithOffset(PlayerStarshipTr.position);

            playerController.SetActiveCanvas(false);

            Z5animator.SetFloat("Part(speed)", 1);

            StartCoroutine(GameScreenDark.ITransparentEvent());

            MoverStartZ2.Move(Z2Starship);
            yield return MoverStartPlayer.IStarshipMove(playerStarship);

            cruiserAttack.StartAim();

            Z5Image.SetActive(true);
            Z2Image.SetActive(true);


            playerCamera.EnableTargetMove(cameraTarget);

            GameDialogs.StartDialogEvent(OnStartDialogEnd);
            GameDialogs.OnNextDialog += OnNextDialog;
        }
        else
        {
            GameGoals.SetActiveGoalEvent(true);

            StaticSettings.isRestart = false;

            systemStarships.SetStarshipsLock(false);

            MoverStartPlayer.ToEndPosition(playerStarship);
            MoverStartZ2.ToEndPosition(Z2Starship);

            Z2StarshipAI.SetTargetToFollowWithMaxDistance(PlayerStarshipTr);

            systemStarships.SetStarshipsFollowEnemy(true);

            StartCoroutine(GameScreenDark.ITransparentEvent());

            cruiserAttack.StartAim();

            FrigateHealth.SetActive(true);

            cruiserAttack.StartAttack(OnBattleEnd);
            isBattle = true;
        }

    }

    private void FixedUpdate()
    {
        if (!isEnd)
        {
            if (Input.GetKey(KeyCode.N))
            {
                StartCoroutine(IEndEvent());
            }
            if (!isPlayerDead)
            {
                if (isBattle && !isSayed && Vector3.Distance(PlayerStarshipTr.position, Z2StarshipAI.transform.position) < 5f)
                {
                    isSayed = true;
                    GameDialogs.ShowInGameDialogEvent(1);
                }
            }
        }
    }

    private int dialogProgress;
    private void OnNextDialog(int dialogNow)
    {
        if (dialogNow > dialogProgress)
        {
            dialogProgress = dialogNow;
            if (dialogProgress == 8)
            {
                Z5Image.SetActive(false);
            }
            if (dialogProgress == 14)
            {
                FrigateHealth.SetActive(true);
            }
        }
    }

    private void OnStartDialogEnd()
    {
        Z2Image.SetActive(false);
        Z5Image.SetActive(false);
        FrigateHealth.SetActive(true);
        playerController.SetActiveCanvas(true);
        playerCamera.DisableTargetMove();
        Z2StarshipAI.SetTargetToFollowWithMaxDistance(PlayerStarshipTr);
        GameGoals.SetActiveGoalEvent(true);
        playerController.SetLockControl(false);
        systemStarships.SetStarshipsLock(false);
        systemStarships.SetStarshipsFollowEnemy(true);

        cruiserAttack.StartAttack(OnBattleEnd);
        isBattle = true;
    }

    private void OnBattleEnd() => StartCoroutine(IOnBattleEnd());
    private IEnumerator IOnBattleEnd()
    {
        isBattle = false;
        playerTrigger.OnPlayerStarshipEnter += EndDialog;
        ExitDoor.Move();
        yield return GameDialogs.IShowInGameDialogEvent(0);
        Z2StarshipAI.SetTargetAndDestinationToShowPath(PlayerStarshipTr, Destination);
    }

    private void EndDialog() => GameDialogs.ShowInGameDialogEvent(1);



    private void PlayerStarshipEnterEndTrigger()
    {
        playerCamera.SetLockMove(true);
        playerController.SetLockControl(true);
        MoverEnd.MoveLine(playerStarship);
        EndGame();
    }

    private void OnPlayerDeath()
    {
        isPlayerDead = true;
        if (!isRestart && !isEnd)
        {
            StartCoroutine(IRestart());
        }
    }

    private IEnumerator IRestart()
    {
        isRestart = true;
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        SceneController.RestartScene();
    }

    private void EndGame()
    {
        if (!isRestart && !isEnd)
        {
            StartCoroutine(IEndEvent());
        }
    }
    private IEnumerator IEndEvent()
    {
        isEnd = true;
        GameAudio.StopAudioEvent();
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        SceneController.LoadNextStoryScene();
    }
}

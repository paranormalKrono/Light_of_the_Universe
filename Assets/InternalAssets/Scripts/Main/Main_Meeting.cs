using System.Collections;
using UnityEngine;

[RequireComponent(typeof(System_Starships), typeof(StarshipsSpawnMover))]
public class Main_Meeting : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private StarshipMover MoverStart;
    [SerializeField] private StarshipMover MoverEnd;
    [SerializeField] private PlayerStarshipTrigger EndTrigger;
    [SerializeField] private Starship_AI_Adv Z2StarshipAI;
    [SerializeField] private ScenesLocations sceneLocationName;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private GameObject Z2Image;
    [SerializeField] private GameObject Z5Image;
    [SerializeField] private GameObject Border;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform Destination;
    [SerializeField] private PlayerStarshipTrigger playerTrigger;
    [SerializeField] private float waitTime1 = 30f;
    [SerializeField] private float waitTime2 = 20f;
    [SerializeField] private float waitTime3 = 10f;
    [SerializeField] private Animator Z5animator;

    [SerializeField] private GameObject Z2_Scene;
    [SerializeField] private GameObject Z5_Scene;

    private System_Starships systemStarships;
    private Player_Starship_Controller playerController;
    private Player_Camera_Controller playerCamera;
    private Starship playerStarship;

    private Transform PlayerStarshipTr;

    private bool isRestart;
    private bool isEnd;
    private bool isWaited1;
    private bool isWaited2;
    private bool isWaited3;
    private bool isWaited;
    private bool isWaitingToAnswer;
    private bool isSayed;
    private bool isWaitedSayed;
    private bool isBattle;
    private bool isPlayerDead;


    private void Awake()
    {
        GameManager.Initialize();

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
        playerTrigger.OnPlayerStarshipEnter += ShowDialog;
    }

    private IEnumerator Start()
    {
        systemStarships.InitializeStarshipsTeams(GetComponent<StarshipsSpawnMover>().MoveStarshipsOnSpawns());

        systemStarships.OnOneTeamLeft += OnOneTeamLeft;

        GameAudio.StartAudioEvent(audioClip, 0.4f, true);
        if (!StaticSettings.isRestart)
        {

            playerController.SetLockControl(true);
            playerCamera.SetLockMove(false);
            systemStarships.SetStarshipsLock(true);

            MoverStart.ToStartPosition(playerStarship);

            playerCamera.SetPositionWithOffset(PlayerStarshipTr.position);

            playerController.SetActiveCanvas(false);

            yield return MoverStart.IStarshipMove(playerStarship);


            playerCamera.EnableTargetMove(cameraTarget);

            GameDialogs.StartDialogEvent(OnStartDialogEnd);
            GameDialogs.OnNextDialog += OnNextDialog;
        }
        else
        {
            GameGoals.SetActiveGoalEvent(true);

            StaticSettings.isRestart = false;

            systemStarships.SetStarshipsLock(false);

            MoverStart.ToEndPosition(playerStarship);

            Z2StarshipAI.SetTargetToFollowWithMaxDistance(PlayerStarshipTr);

            systemStarships.StarshipsTeams[1].SetFollowTarget(PlayerStarshipTr);

            systemStarships.SetStarshipsFollowEnemy(true);

            isBattle = true;
        }

    }

    private void FixedUpdate()
    {
        if (!isEnd)
        {
            if (Input.GetKey(KeyCode.N))
            {
                EndGame();
            }
            if (!isPlayerDead)
            {
                if (isBattle && !isSayed && Vector3.Distance(PlayerStarshipTr.position, Z2StarshipAI.transform.position) < 5f)
                {
                    isSayed = true;
                    GameDialogs.ShowInGameDialogEvent(3);
                }
                if (isWaited3 && !isWaited && Vector3.Distance(PlayerStarshipTr.position, Z2StarshipAI.transform.position) < 40)
                {
                    StartCoroutine(IWaited());
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
            switch (dialogNow)
            {
                case 1:
                    systemStarships.SetStarshipsFollowEnemy(false);
                    systemStarships.StarshipsTeams[1].SetStarshipsLock(false);
                    break;
                case 5:
                    Z5_Scene.SetActive(true);
                    Z5Image.SetActive(true);
                    break;
                case 7:
                    Z5animator.SetFloat("Part(speed)", 1);
                    break;
                case 8:
                    Z2_Scene.SetActive(true);
                    Z2Image.SetActive(true);
                    break;
                case 25:
                    Z5Image.SetActive(false);
                    Z5_Scene.SetActive(false);
                    break;
            }
        }
    }

    private void OnStartDialogEnd()
    {
        Z2Image.SetActive(false);
        Z5Image.SetActive(false);
        Z2_Scene.SetActive(false);
        Z5_Scene.SetActive(false);
        playerController.SetActiveCanvas(true);
        playerCamera.DisableTargetMove();
        Z2StarshipAI.SetTargetToFollowWithMaxDistance(PlayerStarshipTr);
        GameGoals.SetActiveGoalEvent(true);
        playerController.SetLockControl(false);
        systemStarships.StarshipsTeams[1].SetFollowTarget(PlayerStarshipTr);
        systemStarships.SetStarshipsLock(false);
        systemStarships.SetStarshipsFollowEnemy(true);
        isBattle = true;
    }

    private void OnOneTeamLeft() => StartCoroutine(IOnOneTeamLeft());
    private IEnumerator IOnOneTeamLeft()
    {
        isBattle = false;
        Border.SetActive(false);
        Z2StarshipAI.SetTargetAndDestinationToShowPath(PlayerStarshipTr, Destination);
        yield return GameDialogs.IShowInGameDialogEvent(0);
        Wait = IWait1();
        StartCoroutine(Wait);
    }

    private void ShowDialog()
    {
        playerTrigger.OnPlayerStarshipEnter -= ShowDialog;

        StartCoroutine(IShowDialog());
    }
    private IEnumerator IShowDialog()
    {
        if (Wait != null)
        {
            StopCoroutine(Wait);
        }
        if (!isWaited)
        {
            if (!isWaited1)
            {
                yield return GameDialogs.IShowInGameDialogEvent(1);
                Wait = IWait1();
            }
            else if (!isWaited2)
            {
                yield return GameDialogs.IShowInGameDialogEvent(1);
                Wait = IWait2();
            }
            else if (!isWaited3)
            {
                isWaitingToAnswer = true;
                Wait = IWait3();
            }
            StartCoroutine(Wait);
        }
        else
        {
            if (isWaitedSayed)
            {
                GameDialogs.ShowInGameDialogEvent(1);
            }
            else
            {
                isWaitingToAnswer = true;
            }
        }
    }

    private IEnumerator Wait;
    private IEnumerator IWait1()
    {
        yield return new WaitForSeconds(waitTime1);
        isWaited1 = true;
        yield return GameDialogs.IShowInGameDialogEvent(2);
        Wait = IWait2();
        StartCoroutine(Wait);
    }
    private IEnumerator IWait2()
    {
        yield return new WaitForSeconds(waitTime2);
        isWaited2 = true;
        yield return GameDialogs.IShowInGameDialogEvent(4);
        Wait = IWait3();
        StartCoroutine(Wait);
    }
    private IEnumerator IWait3()
    {
        Z2StarshipAI.UnshowPath();
        Z2StarshipAI.SetTargetToFollow(Destination);
        yield return new WaitForSeconds(waitTime3);
        isWaited3 = true;
        Z2StarshipAI.SetTargetToFollow(PlayerStarshipTr);
        Z2StarshipAI.SetTargetAndDestinationToShowPath(PlayerStarshipTr, Destination);
    }
    private IEnumerator IWaited()
    {
        isWaited = true;
        yield return GameDialogs.IShowInGameDialogEvent(5);
        isWaitedSayed = true;
        if (isWaitingToAnswer)
        {
            GameDialogs.ShowInGameDialogEvent(1);
        }
    }


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
            isRestart = true;
            SceneController.RestartScene();
        }
    }

    private void EndGame()
    {
        if (!isRestart && !isEnd)
        {
            isEnd = true;
            SceneController.LoadNextStoryScene();
        }
    }
}
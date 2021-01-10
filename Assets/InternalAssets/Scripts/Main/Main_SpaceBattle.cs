using System.Collections;
using UnityEngine;

[RequireComponent(typeof(System_Starships), typeof(StarshipsSpawnMover))]
public class Main_SpaceBattle : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private StarshipMover MoverEnd;
    [SerializeField] private PlayerStarshipTrigger EndTrigger;
    [SerializeField] private Transform BattleTr;
    [SerializeField] private Starship C07Starship;
    [SerializeField] private Health C07Health;
    [SerializeField] private ScenesLocations sceneLocationName;
    [SerializeField] private float distanceDisqualification = 500;
    [SerializeField] private float distanceBattle = 250;
    [SerializeField] private float timeDisqualification = 8;
    [SerializeField] private AudioClip audioClip;

    private System_Starships systemStarships;
    private Player_Starship_Controller playerController;
    private Player_Camera_Controller playerCamera;
    private Starship PlayerStarship;

    private Transform PlayerStarshipTr;

    private float timeOut;
    private bool isBattle;
    private bool isDisqualified;
    private bool isShowedText;
    private bool isRestart;
    private bool isEnd;
    private bool isPlayerDead;
    private bool isShowedDialog2;


    private void Awake()
    {
        GameManager.Initialize();
        GameScreenDark.SetDarkEvent(true);

        SceneController.LoadAdditiveScene(sceneLocationName);
        GameText.DeactivateEvent();
        GameText.SetInGameTextNowEvent(textAsset);

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Starship_Controller>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        playerController.GetComponent<Health>().OnDeath += OnPlayerDeath;
        systemStarships = GetComponent<System_Starships>();

        PlayerStarshipTr = playerController.transform;
        PlayerStarship = PlayerStarshipTr.GetComponent<Starship>();

        EndTrigger.OnPlayerStarshipEnter += PlayerStarshipEnterEndTrigger;

        C07Health.OnDeath += C07Health_OnDeath;
    }

    private void Start()
    {
        systemStarships.InitializeStarshipsTeams(GetComponent<StarshipsSpawnMover>().MoveStarshipsOnSpawns());

        if (!StaticSettings.isRestart)
        {
            GameAudio.StartAudioEvent(audioClip, 0.4f, true);
            SetGameStop(true);
            GameDialogs.StartDialogEvent(StartGame);
        }
        else
        {
            GameGoals.SetActiveGoalEvent(true);
            StaticSettings.isRestart = false;
            systemStarships.SetStarshipsLock(false);
            systemStarships.SetStarshipsLock(1, true);
            GameDialogs.ShowInGameDialogEvent(0);
        }
        systemStarships.SetStarshipsActive(2, false);
        systemStarships.SetStarshipsLock(2, true);
        systemStarships.SetStarshipsActive(3, false);
        systemStarships.SetStarshipsLock(3, true);
        systemStarships.StarshipsTeams[1].OnTeamDevastated += FirstWaveDead;
        systemStarships.StarshipsTeams[2].OnTeamDevastated += SecondWaveDead;

        StartCoroutine(GameScreenDark.ITransparentEvent());
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
                if (!isBattle)
                {
                    if (systemStarships.GetMinDistanceTeamToPoint(0, BattleTr.position) < distanceBattle)
                    {
                        isBattle = true;
                        systemStarships.SetStarshipsLock(1, false);
                        systemStarships.SetStarshipsFollowEnemy(0, true);
                        GameGoals.ShowGoalEvent(1);
                        GameDialogs.ShowInGameDialogEvent(1);
                    }
                }
                else if (!isDisqualified)
                {
                    if (Vector3.Distance(PlayerStarshipTr.position, BattleTr.position) > distanceDisqualification)
                    {
                        timeOut += Time.fixedDeltaTime;
                        if (!isShowedText)
                        {
                            isShowedText = true;
                            GameDialogs.ShowInGameDialogEvent(4);
                        }
                        if (timeOut > timeDisqualification)
                        {
                            StartCoroutine(IDisqualificate());
                        }
                    }
                    else
                    {
                        isShowedText = false;
                        timeOut = 0;
                    }
                }
            }
        }
    }

    private void StartGame()
    {
        GameGoals.SetActiveGoalEvent(true);
        GameDialogs.ShowInGameDialogEvent(0);
        systemStarships.SetStarshipsLock(1, true);
        SetGameStop(false);
    }


    private void PlayerStarshipEnterEndTrigger()
    {
        playerCamera.SetLockMove(true);
        playerController.SetLockControl(true);
        MoverEnd.MoveLine(playerController.GetComponent<Starship>());
        EndGame();
    }


    private void FirstWaveDead(System_Starships.StarshipsTeam starshipsTeam) => StartCoroutine(IRush());
    private IEnumerator IRush()
    {
        if (!isDisqualified)
        {
            yield return GameDialogs.IShowInGameDialogEvent(2);
            isShowedDialog2 = true;
        }
        systemStarships.SetStarshipsActive(2, true);
        systemStarships.SetStarshipsActive(3, true);
        systemStarships.CombineTeams(3, 0);
    }

    private void SecondWaveDead(System_Starships.StarshipsTeam starshipsTeam) 
    {
        if (!isDisqualified) 
        { 
            StartCoroutine(IDisqualificate()); 
        }
    }

    private IEnumerator IDisqualificate()
    {
        isDisqualified = true;
        yield return GameDialogs.IShowInGameDialogEvent(5);
        int team = systemStarships.StarshipChangeTeamToNew(C07Starship, 0);
        systemStarships.StarshipChangeTeam(PlayerStarship, 0, team);
        C07Health.SetInvincible(false);
        C07Starship.SetFollowTarget(PlayerStarshipTr);
        systemStarships.StarshipsTeams[0].SetFollowTarget(PlayerStarshipTr);
        if (isShowedDialog2)
        {
            GameDialogs.ShowInGameDialogEvent(3);
        }
        else
        {
            GameDialogs.ShowInGameDialogEvent(7);
        }
    }



    private void C07Health_OnDeath()
    {
        GameDialogs.ShowInGameDialogEvent(6);
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

    private void SetGameStop(bool t)
    {
        playerController.SetLockControl(t);
        playerCamera.SetLockMove(t);
        systemStarships.SetStarshipsLock(t);
    }
}
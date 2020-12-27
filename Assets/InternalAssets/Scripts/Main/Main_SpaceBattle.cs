using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(System_Starships), typeof(StarshipsSpawnMover))]
public class Main_SpaceBattle : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private PlayerStarshipMover MoverEnd;
    [SerializeField] private PlayerStarshipTrigger EndTrigger;
    [SerializeField] private Text DisqualificationText;
    [SerializeField] private Transform BattleTr;
    [SerializeField] private Starship C07Starship;
    [SerializeField] private Health C07Health;
    [SerializeField] private ScenesLocations sceneLocationName;
    [SerializeField] private float distanceDisqualification = 300;
    [SerializeField] private float distanceBattle = 250;
    [SerializeField] private float timeDisqualification = 8;
    [SerializeField] private float showTextTime = 3;
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
    private bool isShowText;
    private bool isRestart;
    private bool isEnd;
    private bool isPlayerDead;
    private bool isC07Dead;


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
        C07Health.OnDeath += C07Death;
    }

    private void Start()
    {
        systemStarships.InitializeStarshipsTeams(GetComponent<StarshipsSpawnMover>().MoveStarshipsOnSpawns()); 

        if (!StaticSettings.isRestart)
        {
            GameAudio.StartAudioEvent(audioClip, true);
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
                if (!isDisqualified)
                {
                    if (Vector3.Distance(PlayerStarshipTr.position, BattleTr.position) > distanceDisqualification && systemStarships.GetMinDistanceTeamToPoint(0, PlayerStarshipTr.position) > distanceDisqualification)
                    {
                        timeOut += Time.fixedDeltaTime;
                        if (!isShowedText && !isShowText)
                        {
                            isShowedText = true;
                            StartCoroutine(IShowText(DisqualificationText));
                        }
                        if (timeOut > timeDisqualification)
                        {
                            isDisqualified = true;
                            int team = systemStarships.StarshipChangeTeamToNew(C07Starship, 0);
                            systemStarships.StarshipChangeTeam(PlayerStarship, 0, team);
                            if (!isC07Dead)
                            {
                                GameDialogs.ShowInGameDialogEvent(4);
                            }
                        }
                    }
                    else
                    {
                        isShowedText = false;
                        timeOut = 0;
                    }
                }
                if (!isBattle)
                {
                    if (systemStarships.GetMinDistanceTeamToPoint(0, BattleTr.position) < distanceBattle)
                    {
                        isBattle = true;
                        systemStarships.SetStarshipsLock(1, false);
                        systemStarships.SetStarshipsFollowTarget(0, true);
                        GameGoals.ShowGoalEvent(1);
                        GameDialogs.ShowInGameDialogEvent(1);
                        systemStarships.StarshipsTeams[1].OnTeamDevastated += Rush;
                    }
                }
            }
        }
    }

    private void OnPlayerDeath()
    {
        isPlayerDead = true;
        if (!isRestart && !isEnd)
        {
            StartCoroutine(IRestart());
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
        MoverEnd.MoveLine(playerController);
        EndGame();
    }
    private void EndGame()
    {
        if (!isRestart && !isEnd)
        {
            StartCoroutine(IEndEvent());
        }
    }
    private void Rush(System_Starships.StarshipsTeam starshipsTeam)
    {
        GameDialogs.ShowInGameDialogEventIE(2, RushE);
    }
    private void RushE()
    {
        C07Health.SetInvincible(false);
        systemStarships.SetStarshipsActive(1, true);
        systemStarships.SetStarshipsLock(1, false);
    }
    private void C07Death()
    {
        isC07Dead = true;
        GameDialogs.ShowInGameDialogEvent(3);
    }

    private IEnumerator IRestart()
    {
        isRestart = true;
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        SceneController.RestartScene();
    }
    private IEnumerator IEndEvent()
    {
        isEnd = true;
        GameAudio.StopAudioEvent();
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        SceneController.LoadNextStoryScene();
    }


    private IEnumerator IShowText(Text text)
    {
        isShowText = true;
        text.enabled = true;
        yield return new WaitForSeconds(showTextTime);
        text.enabled = false;
        isShowText = false;
    }

    private void SetGameStop(bool t)
    {
        playerController.SetLockControl(t);
        playerCamera.SetLockMove(t);
        systemStarships.SetStarshipsLock(t);
    }
}
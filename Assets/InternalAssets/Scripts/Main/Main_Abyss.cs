using System.Collections;
using UnityEngine;

public class Main_Abyss : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private ScenesLocations sceneLocation;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Transform[] enemySpawns;
    [SerializeField] private ScreenDark screenDark;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject slide;
    [SerializeField] private float wavesTimeInterval = 20;
    [SerializeField] private float timeBetweenCreatingEnemies = 0.2f;
    [SerializeField] private float timeToSupportInSeconds = 540;
    [SerializeField] private float timeToIngameDialog1 = 200;
    [SerializeField] private float timeToIngameDialog2 = 200;

    private System_Starships systemStarships;
    private Player_Starship_Controller playerController;
    private Player_Camera_Controller playerCamera;
    private Starship playerStarship;

    private float time = 0;
    private float timeToWave;
    private int enemiesCount;
    private bool isPlayerDead;
    private bool isCreatingEnemies;
    private bool isBattle;
    private bool isShowDialog1;
    private bool isShowDialog2;
    private bool isShowedSlide;

    private void Awake()
    {
        GameManager.Initialize();

        SceneController.LoadAdditiveScene(sceneLocation);
        GameText.DeactivateEvent();
        GameText.SetInGameTextNowEvent(textAsset);

        systemStarships = GetComponent<System_Starships>();

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Starship_Controller>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        playerStarship = playerController.GetComponent<Starship>();
        playerController.GetComponent<Health>().OnDeath += OnPlayerDeath;

        systemStarships.StarshipsTeams.Add(new System_Starships.StarshipsTeam(playerStarship.GetComponent<Starship>()));
        systemStarships.StarshipsTeams.Add(new System_Starships.StarshipsTeam());

        playerCamera.SetPositionWithOffset(playerController.transform.position);
        screenDark.SetDark(false);
        playerController.SetLockControl(true);
    }

    private void Update()
    {
        if (!isShowedSlide)
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.RightShift))
            {
                isShowedSlide = true;
                StartCoroutine(ISlidesEnd());
            }
        }
    }
    private void FixedUpdate()
    {
        if (isShowedSlide &&isBattle && !isPlayerDead)
        {
            time += Time.fixedDeltaTime;
            if (!isShowDialog1 && time > timeToIngameDialog1)
            {
                isShowDialog1 = true;
                GameDialogs.ShowInGameDialogEvent(0);
            }
            if (!isShowDialog2 && time > timeToIngameDialog2)
            {
                isShowDialog2 = true;
                GameDialogs.ShowInGameDialogEvent(1);
            }
            timeToWave += Time.fixedDeltaTime;
            if (timeToWave > wavesTimeInterval)
            {
                wavesTimeInterval *= 1.02f;
                timeToWave = 0;
                StopAllCoroutines();
                StartCoroutine(IWave());
            }
        }
    }
    private IEnumerator ISlidesEnd()
    {
        yield return StartCoroutine(screenDark.IDark());
        slide.SetActive(false);
        yield return StartCoroutine(screenDark.ITransparent());
        GameDialogs.StartDialogEvent(OnStartDialogEnd);
        GameDialogs.OnNextDialog += OnNextDialog;

    }

    private int dialogProgress;
    private void OnNextDialog(int dialogNow)
    {
        if (dialogNow > dialogProgress)
        {
            dialogProgress = dialogNow;
            switch (dialogNow)
            {
                case 5:
                    GameTimer.ShowTimerEvent(timeToSupportInSeconds);
                    break;
            }
        }
    }

    private void OnStartDialogEnd()
    {
        GameTimer.StartDecreasingTimerEvent(timeToSupportInSeconds, OnTimerEnd);
        GameAudio.StartAudiosEvent(audioClips, 0.12f, false);
        playerController.SetLockControl(false);
        isBattle = true;
    }

    private void OnTimerEnd()
    {
        StartCoroutine(ITimerEnd());
    }
    private IEnumerator ITimerEnd()
    {
        yield return GameDialogs.IShowInGameDialogEvent(2);
        GameTimer.DeactivateEvent();
    }

    private IEnumerator IWave()
    {
        float waveStartTime = time;
        isCreatingEnemies = true;
        while (enemiesCount < waveStartTime / 10)
        {
            enemiesCount += 1;
            CreateEnemy(enemySpawns[Random.Range(0, enemySpawns.Length)]);
            yield return new WaitForSeconds(timeBetweenCreatingEnemies);
        }
        isCreatingEnemies = false;
    }

    private void CreateEnemy(Transform spawnTr)
    {
        GameObject g = Instantiate(enemyPrefab);
        Starship starship = g.GetComponent<Starship>();

        g.transform.position = spawnTr.position;
        starship.RotationPoint.rotation = spawnTr.rotation;

        systemStarships.StarshipsTeams[1].AddStarship(starship);
        starship.SetFollowTarget(playerStarship.transform);
        starship.SetAttack(true);
        starship.SetFollowEnemy(true);
        starship.DeathEvent += OnEnemyDeath;
        starship.SetLockControl(false);
    }

    private void OnEnemyDeath(Transform transform)
    {
        if (enemiesCount == 0 && !isCreatingEnemies)
        {
            time += wavesTimeInterval;
            timeToWave = wavesTimeInterval;
        }
        enemiesCount -= 1;
    }

    private void OnPlayerDeath()
    {
        StopAllCoroutines();
        isPlayerDead = true;
        SceneController.LoadSceneWithTransition(Scenes.Menu);
    }
}

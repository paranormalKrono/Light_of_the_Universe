using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Main_Base : MonoBehaviour
{
    [SerializeField] private Scenes nextRankMission;
    [SerializeField] private Scenes nextBarMission;
    [SerializeField] private Player_Character_Controller Player;
    [SerializeField] private Character_Camera_Controller PlayerCamera;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private GameObject Base1;
    [SerializeField] private GameObject Base2;
    [SerializeField] private GameObject PlayerStarshipPrefab1;
    [SerializeField] private GameObject PlayerStarshipPrefab2;
    [SerializeField] private GameObject PlayerStarship1;
    [SerializeField] private GameObject PlayerStarship2;
    [SerializeField] private ModificationPanel UpgradeMenu1;
    [SerializeField] private ModificationPanel UpgradeMenu2;
    [SerializeField] private System_Goals systemGoals;
    [SerializeField] private System_News NewsTextLoader;
    [SerializeField] private AudioClip audioClip;


    [SerializeField] private GameObject News;
    [SerializeField] private GameObject NextRankButton;
    [SerializeField] private GameObject NextBarButton;
    [SerializeField] private Text nextRankText;
    [SerializeField] private Text nextBarText;

    [SerializeField] private Color Base1Color;
    [SerializeField] private Color Base2Color;

    [SerializeField] private int nextRankSlidesID;

    private bool isMissionStarted;

    private Scenes GameProgressNow;

    void Awake()
    {
        GameManager.Initialize();

        PlayerCamera.SetSensitivity((float)Settings.Sensitivity / 100);
        Settings.OnSensitivityChanged += OnSensitivityChanged;

        if (StaticSettings.GameProgress != 0)
        {
            GameProgressNow = (Scenes)StaticSettings.GameProgress;
        }
        else
        {
            GameProgressNow = SpaceMissions.FirstSpaceMission;
        }
        Base1.SetActive(false);
        Base2.SetActive(false);
        if (GameProgressNow <= nextRankMission && !StaticSettings.isCompleteSomething)
        {
            Base1.SetActive(true);
            PlayerStarship1.SetActive(true);
        }
        else
        {
            Base2.SetActive(true);
            PlayerStarship2.SetActive(true);
        }
        if (GameProgressNow == nextRankMission && !StaticSettings.isCompleteSomething)
        {
            nextRankText.enabled = true;
            NextRankButton.SetActive(true);
        }
        else if (GameProgressNow == nextBarMission && !StaticSettings.isCompleteSomething)
        {
            nextBarText.enabled = true;
            NextBarButton.SetActive(true);
        }
        else
        {
            if (GameProgressNow >= nextRankMission)
            {
                NewsTextLoader.Initialize(StaticSettings.MissionStagesProgress, Base2Color);
            }
            else
            {
                NewsTextLoader.Initialize(StaticSettings.MissionStagesProgress, Base1Color);
            }
            Scenes[] scenes = SceneController.MissionStages[StaticSettings.MissionStagesProgress];
            systemGoals.Initialize(StaticSettings.MissionStagesProgress, (int)scenes[0] - (int)SpaceMissions.FirstSpaceMission, scenes.Length);
        }
        if (GameProgressNow != nextRankMission || !StaticSettings.isCompleteSomething)
        {
            GameAudio.StartAudioEvent(audioClip, true);
        }
        GameMenu.DisactivateGameMenuEvent();
        ScreenDark.SetDarkEvent(true);
        StartCoroutine(ScreenDark.ITransparentEvent());
    }

    public void StartMission()
    {
        if (!isMissionStarted)
        {
            isMissionStarted = true;
            StartCoroutine(IStartGoal());
        }
    }

    public void NextRank()
    {
        StartCoroutine(INextRank());
    }

    public void NextBar()
    {
        StartCoroutine(INextBar());
    }

    private void Start()
    {
        if (GameProgressNow <= nextRankMission && !StaticSettings.isCompleteSomething)
        {
            UpgradeMenu1.gameObject.SetActive(true);
            UpgradeMenu1.Initialize(PlayerStarshipPrefab1);
        }
        else
        {
            UpgradeMenu2.gameObject.SetActive(true);
            UpgradeMenu2.Initialize(PlayerStarshipPrefab2);
        }
    }

    private IEnumerator INextRank()
    {
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        StaticSettings.isCompleteSomething = true;
        Settings.OnSensitivityChanged -= OnSensitivityChanged;
        SceneController.LoadSlides(Scenes.Space_Base,nextRankSlidesID);
    }

    private IEnumerator INextBar()
    {
        GameAudio.StopAudioEvent();
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        Settings.OnSensitivityChanged -= OnSensitivityChanged;
        SceneController.LoadScene(Scenes.Space_Base_Bar);
    }

    private IEnumerator IStartGoal()
    {
        GameAudio.StopAudioEvent();
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        GameMenu.SetGameCursorLock(false);
        StaticSettings.isCompleteSomething = false;
        Settings.OnSensitivityChanged -= OnSensitivityChanged;
        SceneController.LoadScene(GameProgressNow);
    }

    public void GoInHangar() => StartCoroutine(IGoInHangar());

    private IEnumerator IGoInHangar()
    {
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        Base1.SetActive(false);
        Base2.SetActive(false);
        News.SetActive(false);
        MainCamera.enabled = false;
        Player.gameObject.SetActive(true);
        GameMenu.SetGameCursorLock(true);
        StartCoroutine(ScreenDark.ITransparentEvent());
    }

    private void OnSensitivityChanged(int value) => PlayerCamera.SetSensitivity((float)value / 100);

}
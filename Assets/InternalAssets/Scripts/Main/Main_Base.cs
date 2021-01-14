using System.Collections;
using UnityEngine;

public class Main_Base : MonoBehaviour
{

    [SerializeField] private int nextRankNextMissionStage;
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
    [SerializeField] private ScreenDark screenDark;


    [SerializeField] private GameObject News;
    [SerializeField] private GameObject NextRankButton;
    [SerializeField] private GameObject NextBarButton;
    [SerializeField] private GameObject nextRankText;
    [SerializeField] private GameObject nextBarText;

    [SerializeField] private Color Base1Color;
    [SerializeField] private Color Base2Color;

    private bool isMissionStarted;

    private int nextRankStatus;


    void Awake()
    {
        GameManager.Initialize();
        PlayerCamera.SetSensitivity((float)Settings.Sensitivity / 100);
        Settings.OnSensitivityChanged += OnSensitivityChanged;

        Base1.SetActive(false);
        Base2.SetActive(false);

        if (SceneController.GetNextMissionStage() < nextRankNextMissionStage)
        {
            nextRankStatus = -1;
        }
        else if (SceneController.GetNextMissionStage() == nextRankNextMissionStage)
        {
            nextRankStatus = 0;
        }
        else
        {
            nextRankStatus = 1;
        }

        if (nextRankStatus == -1 || nextRankStatus == 0 && !StaticSettings.isCompleteSomething)
        {
            Base1.SetActive(true);
            PlayerStarship1.SetActive(true);
        }
        else
        {
            Base2.SetActive(true);
            PlayerStarship2.SetActive(true);
        }
        if (nextRankStatus == 0 && !StaticSettings.isCompleteSomething)
        {
            nextRankText.SetActive(true);
            NextRankButton.SetActive(true);
        }
        else if (SceneController.GetNextStoryScene() == Scenes.Space_Base_Bar && !StaticSettings.isCompleteSomething)
        {
            nextBarText.SetActive(true);
            NextBarButton.SetActive(true);
        }
        else
        {
            int MissionStagesProgress = SceneController.GetNextMissionStage();
            if (nextRankStatus == 1 || nextRankStatus == 0 && StaticSettings.isCompleteSomething)
            {
                NewsTextLoader.Initialize(MissionStagesProgress, Base2Color);
            }
            else
            {
                NewsTextLoader.Initialize(MissionStagesProgress, Base1Color);
            }
            Scenes[] scenes = SceneController.MissionStages[MissionStagesProgress];
            systemGoals.Initialize(MissionStagesProgress, (int)scenes[0] - (int)SpaceMissions.FirstSpaceMission, scenes.Length);
        }
        if (nextRankStatus != 0 || !StaticSettings.isCompleteSomething)
        {
            GameAudio.StartAudioEvent(audioClip, 0.2f, true);
        }
        GameMenu.DisactivateGameMenuEvent();
    }

    private void Start()
    {
        if (nextRankStatus == -1 && !StaticSettings.isCompleteSomething)
        {
            UpgradeMenu1.gameObject.SetActive(true);
            UpgradeMenu1.Initialize(PlayerStarshipPrefab1);
        }
        else
        {
            UpgradeMenu2.gameObject.SetActive(true);
            UpgradeMenu2.Initialize(PlayerStarshipPrefab2);
        }
        StaticSettings.isCompleteSomething = false;
    }



    public void StartMission()
    {
        if (!isMissionStarted)
        {
            isMissionStarted = true;
            Settings.OnSensitivityChanged -= OnSensitivityChanged;
            SceneController.LoadNextStoryScene();
        }
    }


    private void NextRank()
    {
        StaticSettings.isCompleteSomething = true;
        Settings.OnSensitivityChanged -= OnSensitivityChanged;
        SceneController.LoadNextStoryScene();
    }


    public void NextBar()
    {
        GameAudio.StopAudioEvent();
        StaticSettings.isCompleteSomething = true;
        Settings.OnSensitivityChanged -= OnSensitivityChanged;
        SceneController.LoadNextStoryScene();
    }

    private void GoInHangar() => StartCoroutine(IGoInHangar());
    private IEnumerator IGoInHangar()
    {
        GameMenu.SetGameCursorLock(true, CursorLockMode.Locked);
        yield return StartCoroutine(screenDark.IDark());
        Base1.SetActive(false);
        Base2.SetActive(false);
        News.SetActive(false);
        MainCamera.enabled = false;
        Player.gameObject.SetActive(true);
        StartCoroutine(screenDark.ITransparent());
    }

    private void OnSensitivityChanged(int value) => PlayerCamera.SetSensitivity((float)value / 100);

}
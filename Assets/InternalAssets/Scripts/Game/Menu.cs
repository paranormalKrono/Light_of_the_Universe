using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//Меню
public class Menu : MonoBehaviour
{
    [SerializeField] private Scenes[] CompanyScenes; // Сцены начал компаний
    [SerializeField] private int[] SlidesProgress; // Прогресс слайдов для сцен начала компаний
    [SerializeField] private GameObject MenuPanel; // Панель меню
    [SerializeField] private GameObject CompanyPanel; // Панель компании
    [SerializeField] private Button CompanyButton2; // Кнопка запуска компании 2
    [SerializeField] private Button ButtonGalacticCoordinates; // Кнопка галактических координат
    [SerializeField] private Button ButtonStart;
    [SerializeField] private AudioClip[] audioClips;
    private bool isLoading; // Загрузка другой сцены

    private void Awake()
    {
        GameManager.Initialize();
        StaticSettings.isMenu = true;
        ScreenDark.SetDarkEvent(true);
        StartCoroutine(ScreenDark.ITransparentEvent());
        GameAudio.StartAudiosEvent(audioClips, true);
        GameMenu.DisactivateGameMenuEvent();
        GameCursor.SetCursorLock(false);

        if (StaticSettings.isCompanyMenu)
        {
            CompanyPanel.SetActive(true);
            MenuPanel.SetActive(false);
        }
        if (StaticSettings.isPart1Complete)
        {
            CompanyButton2.interactable = true;
        }
        if (StaticSettings.isPart2Complete)
        {
            ButtonGalacticCoordinates.interactable = true;
        }
    }

    private void Start()
    {
        StaticSettings.ReInitialize();

        ButtonStart.Select();
        if (StaticSettings.isCreatedGameMenus)
        {
            ScreenDark.SetDarkEvent(true);
            StartCoroutine(ScreenDark.ITransparentEvent());
        }
    }

    public void Exit() // Выход из игры
    {
        Application.Quit();
    }
    public void StartCompany(int ID) // Начало одиночной игры
    {
        if (!isLoading)
        {
            StartCoroutine(IStartCompany(ID));
        }
    }
    private IEnumerator IStartCompany(int ID) // Начало компании
    {
        isLoading = true;
        GameAudio.StopAudioEvent();
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        StaticSettings.isMenu = false;
        SceneController.LoadSlides(CompanyScenes[ID], SlidesProgress[ID]); // Компания
    }
    public void LoadSpaceBase(int id) // Загрузка определённого фрагмента одиночной игры
    {
        if (!isLoading)
        {
            StartCoroutine(ILoadSpaceBase(id + SpaceMissions.FirstSpaceMission));
        }
    }
    private IEnumerator ILoadSpaceBase(Scenes nextMission) // Загрузка компании
    {
        isLoading = true;
        GameAudio.StopAudioEvent();
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        StaticSettings.GameProgress = (int)nextMission;
        StaticSettings.credits += SpaceMissions.GetRewardsTo(nextMission);
        StaticSettings.isMenu = false;
        SceneController.LoadScene(Scenes.Space_Base); // Фрагмент
    }
    public void LoadMission(int id) // Загрузка определённой миссии одиночной игры
    {
        if (!isLoading)
        {
            StartCoroutine(ILoadMission(id + SpaceMissions.FirstSpaceMission));
        }
    }
    private IEnumerator ILoadMission(Scenes mission) // Загрузка миссии
    {
        isLoading = true;
        GameAudio.StopAudioEvent();
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        StaticSettings.isMenu = false;
        SceneController.LoadScene(mission); // Фрагмент
    }

    public void OpenSettings() => GameSettingsMenu.OpenMenuEvent(MenuPanel);
}

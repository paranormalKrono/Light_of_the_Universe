using System.Collections;
using UnityEngine;

public class GameMenu : MonoBehaviour
{

    [SerializeField] private GameObject mainGameMenu;

    private static bool isMenuOpened;
    private static bool isGameCursorLock;

    private bool isExit;

    public delegate void Event();
    public static Event DisactivateGameMenuEvent;
    public static event Event OnMenuClose;
    public static event Event OnMenuOpen;


    private void Awake()
    {
        DisactivateGameMenuEvent = CloseGameMenu;
    }

    private void Update()
    {
        if (!StaticSettings.isMenu && !isExit)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isMenuOpened)
                {
                    CloseMenu();
                }
                else
                {
                    OpenMenu();
                }
            }
        }
    }


    public void CloseMenu()
    {
        SetTimeScale(1);
        GameSettingsMenu.CloseMenuEvent();
        mainGameMenu.SetActive(false);
        if (isGameCursorLock)
        {
            GameCursor.SetCursorLock(true);
        }
        isMenuOpened = false;
        OnMenuClose?.Invoke();
    }
    private void OpenMenu()
    {
        SetTimeScale(0);
        isMenuOpened = true;
        mainGameMenu.SetActive(true);
        GameCursor.SetCursorLock(false);
        OnMenuOpen?.Invoke();
    }
    private void SetTimeScale(float f)
    {
        Time.timeScale = f;
    }


    private void CloseGameMenu()
    {
        GameText.DeactivateEvent();
    }


    public void GameExit()
    {
        StartCoroutine(IGameExit());
    }

    private IEnumerator IGameExit()
    {
        if (!isExit)
        {
            isExit = true;
            GameAudio.StopAudioEvent();
            StaticSettings.isCompanyMenu = true;
            CloseMenu();
            yield return StartCoroutine(ScreenDark.IDarkEvent());
            GameTimer.DeactivateEvent();
            isExit = false;
            SceneController.LoadScene(Scenes.Menu);
        }
    }


    internal static void SetGameCursorLock(bool t)
    {
        if (!isMenuOpened)
        {
            GameCursor.SetCursorLock(t);
        }
        isGameCursorLock = t;
    }

    public void OpenSettings() => GameSettingsMenu.OpenMenuEvent(mainGameMenu);
}
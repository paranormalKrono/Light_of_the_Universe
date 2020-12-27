using System.Collections;
using UnityEngine;

public class GameMenu : MonoBehaviour
{

    [SerializeField] private GameObject mainGameMenu;

    private static bool isMenuOpened;
    private static bool isGameCursorLock;

    private bool isDoSomething;

    public delegate void Event();
    public static Event DisactivateGameMenuEvent;
    public static Event CloseMenuEvent;
    public static event Event OnMenuClose;
    public static event Event OnMenuOpen;


    private void Awake()
    {
        DisactivateGameMenuEvent = CloseGameMenu;
        CloseMenuEvent = CloseMenu;
    }

    private void Update()
    {
        if (!SceneController.IsMenu && !isDoSomething)
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
        MenusOpener.ClosesMenusEvent();
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

    public void GameLevelRestart()
    {
        if (!isDoSomething)
        {
            isDoSomething = true;
            StartCoroutine(IGameLevelRestart());
        }
    }

    public void GameLevelCheckpointRestart()
    {
        if (!isDoSomething)
        {
            isDoSomething = true;
            StartCoroutine(IGameLevelCheckpointRestart());
        }
    }

    public void GameExit()
    {
        if (!isDoSomething)
        {
            isDoSomething = true;
            StartCoroutine(IGameExit());
        }
    }

    private IEnumerator IGameLevelRestart()
    {
        CloseMenu();
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        GameTimer.DeactivateEvent();
        isDoSomething = false;
        StaticSettings.checkpointID = 0;
        SceneController.RestartScene();
    }

    private IEnumerator IGameLevelCheckpointRestart()
    {
        CloseMenu();
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        GameTimer.DeactivateEvent();
        isDoSomething = false;
        SceneController.RestartScene();
    }

    private IEnumerator IGameExit()
    {
        GameAudio.StopAudioEvent();
        CloseMenu();
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        GameTimer.DeactivateEvent();
        isDoSomething = false;
        SceneController.SceneTransitionTo(Scenes.Menu);
    }


    internal static void SetGameCursorLock(bool t)
    {
        if (!isMenuOpened)
        {
            GameCursor.SetCursorLock(t);
        }
        isGameCursorLock = t;
    }

    public void OpenSettings() => MenusOpener.OpenSettingsMenuEvent(mainGameMenu);
    public void OpenSaves() => MenusOpener.OpenSavesMenuEvent(mainGameMenu);
}
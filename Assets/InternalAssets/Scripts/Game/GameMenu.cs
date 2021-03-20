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
        GameAudio.UnPauseAudioEvent();
        MenusOpener.ClosesMenusEvent();
        mainGameMenu.SetActive(false);
        if (isGameCursorLock)
        {
            GameCursor.SetCursorLock(true, CursorLockMode.Locked);
        }
        isMenuOpened = false;
        OnMenuClose?.Invoke();
    }
    private void OpenMenu()
    {
        SetTimeScale(0);
        GameAudio.PauseAudioEvent();
        isMenuOpened = true;
        mainGameMenu.SetActive(true);
        GameCursor.SetCursorLock(false, CursorLockMode.None);
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
            CloseMenu();
            isDoSomething = false;
            StaticSettings.checkpointID = 0;
            SceneController.RestartScene();
        }
    }

    public void GameLevelCheckpointRestart()
    {
        if (!isDoSomething)
        {
            isDoSomething = true;
            CloseMenu();
            isDoSomething = false;
            SceneController.RestartScene();
        }
    }

    public void GameExit()
    {
        if (!isDoSomething)
        {
            isDoSomething = true;
            CloseMenu();
            isDoSomething = false;
            SceneController.SceneTransitionTo(Scenes.Menu, false);
        }
    }


    internal static void SetGameCursorLock(bool t, CursorLockMode cursorLockMode)
    {
        if (!isMenuOpened)
        {
            GameCursor.SetCursorLock(t, cursorLockMode);
        }
        isGameCursorLock = t;
    }

    public void OpenSettings() => MenusOpener.OpenSettingsMenuEvent(mainGameMenu);
    public void OpenSaves() => MenusOpener.OpenSavesMenuEvent(mainGameMenu);
}
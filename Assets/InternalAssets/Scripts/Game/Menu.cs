using System;
using UnityEngine;
using UnityEngine.UI;
//Меню
public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject MenuPanel; // Панель меню
    [SerializeField] private Button ButtonGalacticCoordinates; // Кнопка галактических координат
    [SerializeField] private Button ButtonContinue;
    [SerializeField] private Button ButtonStart;
    [SerializeField] private AudioClip[] audioClips;
    private bool isLoading; // Загрузка другой сцены

    private void Awake()
    {
        GameManager.Initialize();
        GameAudio.StartAudiosEvent(audioClips, true);
        GameMenu.DisactivateGameMenuEvent();

        ButtonStart.Select();
        if (PlayerPrefs.HasKey("GameComplete"))
        {
            StaticSettings.isGameComplete = Convert.ToBoolean(PlayerPrefs.GetInt("GameCompete"));
        }
        if (StaticSettings.isGameComplete)
        {
            ButtonGalacticCoordinates.interactable = true;
        }

        if (Saves.GetAutosaveExists())
        {
            ButtonContinue.interactable = true;
        }
    }

    
    public void StartCompany() // Начало одиночной игры
    {
        if (!isLoading)
        {
            isLoading = true;
            SceneController.LoadStory(); // Компания
        }
    }

    public void LoadAutosave()
    {
        isLoading = true;
        Saves.LoadAutosave();
        SceneController.LoadSave();
    }

    public void OpenSettings() => MenusOpener.OpenSettingsMenuEvent(MenuPanel);
    public void OpenSaves() => MenusOpener.OpenSavesMenuEvent(MenuPanel);

    public void Exit() => Application.Quit();
}
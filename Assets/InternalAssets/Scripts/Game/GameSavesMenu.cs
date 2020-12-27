using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSavesMenu : MonoBehaviour
{

    [SerializeField] private RectTransform saveMenuPrefab;
    [SerializeField] private RectTransform savesContent;

    [SerializeField] private Button CreateSaveButton;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button DeleteSaveButton;
    [SerializeField] private Button LoadSaveButton;

    [SerializeField] private float timeToDeselect = 0.2f;

    private DateTime selectedSaveDateTime;
    private SaveCell selectedSaveCell;
    private bool isSelected;


    private void Awake()
	{
		InitialiseSavesMenus();
        MenusOpener.OnSavesMenuOpen += OnMenuOpen;
        MenusOpener.OnSavesMenuClose += OnMenuClose;
    }


    private void OnMenuOpen()
    {
        CreateSaveButton.interactable = !SceneController.IsMenu;
    }

    private void OnMenuClose()
    {
        DeselectSaveCell();
    }


	private void InitialiseSavesMenus()
    {
		Saves.SaveData[] saves = Saves.GetSaveDatas();
        for (int i = 0; i < saves.Length; ++i)
        {
            InitialiseSaveCell(AddContent(saveMenuPrefab, savesContent).GetComponent<SaveCell>(), saves[i]);
        }
    }



    public void Save()
    {
        Saves.SaveData saveData = Saves.GetCurrentDataToSave();
        Saves.DeleteSaveFile(selectedSaveDateTime);
        Saves.CreateSaveFile(saveData);
        InitialiseSaveCell(selectedSaveCell, saveData);
        selectedSaveDateTime = saveData.saveDateTime;
    }
    public void CreateSave()
    {
        Saves.SaveData saveData = Saves.GetCurrentDataToSave();
        GameObject g = AddContent(saveMenuPrefab, savesContent);
        g.transform.SetAsFirstSibling();
        InitialiseSaveCell(g.GetComponent<SaveCell>(), saveData);
        Saves.CreateSaveFile(saveData);
    }
    public void DeleteSave()
    {
        Saves.DeleteSaveFile(selectedSaveDateTime);
        Destroy(selectedSaveCell.gameObject);
        SetSaveDataSelected(false);
    }
    public void LoadSave()
    {
        StaticSettings.ReInitialize();
        Saves.LoadSaveFile(selectedSaveDateTime);
        MenusOpener.ClosesMenusEvent();
        GameMenu.CloseMenuEvent();
        SceneController.LoadSave();
    }

    private void InitialiseSaveCell(SaveCell saveCell, Saves.SaveData saveData) => saveCell.Initialise(saveData, SelectSaveCell);


    private void DeselectSaveCell()
    {
        if (isSelected)
        {
            isSelected = false;
            selectedSaveCell.SetButtonInteractable(true);
            SetSaveDataSelected(false);
        }
    }

    private void SelectSaveCell(DateTime dateTime, SaveCell saveCell)
    {
        if (isSelected)
        {
            selectedSaveCell.SetButtonInteractable(true);
        }
        else
        {
            SetSaveDataSelected(false);
        }
        selectedSaveDateTime = dateTime;
        selectedSaveCell = saveCell;
        SetSaveDataSelected(true);
        selectedSaveCell.SetButtonInteractable(false);
    }

    private void SetSaveDataSelected(bool b)
    {
        isSelected = b;
        if (!SceneController.IsMenu)
        {
            SaveButton.interactable = b;
        }
        LoadSaveButton.interactable = b;
        DeleteSaveButton.interactable = b;
    }



    private GameObject AddContent(RectTransform prefab, RectTransform content)
    {
        GameObject instance = Instantiate(prefab.gameObject);
        instance.transform.SetParent(content, false);
        return instance;
    }

}
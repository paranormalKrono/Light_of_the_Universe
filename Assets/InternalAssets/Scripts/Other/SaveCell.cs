using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveCell : MonoBehaviour
{
    [SerializeField] private Text DataText;
    [SerializeField] private Text ProgressText;
    [SerializeField] private Text CreditsText;
    [SerializeField] private Button button;

    public delegate void EventHandler(DateTime dateTime, SaveCell saveCell);

    public void Initialise(Saves.SaveData saveData, EventHandler @event)
    {
        DataText.text = saveData.saveDateTime.ToString();
        ProgressText.text = (int)(100 / (float)SceneController.StoryScenesSequence.Length * saveData.GameProgress) + "%";
        CreditsText.text = saveData.credits.ToString();
        button.onClick.AddListener(() => @event(saveData.saveDateTime, this));
    }

    public void SetButtonInteractable(bool b)
    {
        button.interactable = b;
    }
}
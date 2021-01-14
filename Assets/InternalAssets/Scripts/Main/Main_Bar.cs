using System.Collections;
using UnityEngine;

public class Main_Bar : MonoBehaviour
{
    [SerializeField] private System_Dialogs system_dialogs;

    void Awake()
    {
        GameManager.Initialize();

        system_dialogs.Initialise(GameText.GetBarDialogEvent(), GameText.GetNamesEvent());
        system_dialogs.EndEvent = End;
    }

    private void End()
    {
        StaticSettings.isCompleteSomething = true;
        SceneController.LoadNextStoryScene();
    }
}

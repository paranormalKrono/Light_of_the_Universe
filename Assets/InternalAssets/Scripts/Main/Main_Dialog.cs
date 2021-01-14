using UnityEngine;

public class Main_Dialog : MonoBehaviour
{
    [SerializeField] protected System_Dialogs system_dialogs;
    [SerializeField] private TextAsset dialogText;

    private void Awake()
    {
        GameManager.Initialize();
        GameMenu.DisactivateGameMenuEvent();

        system_dialogs.Initialise(GameText.GetDialogEvent(dialogText), GameText.GetNamesEvent());
        system_dialogs.EndEvent = End;

        bAwake();
    }

    protected virtual void bAwake() { }

    private void End()
    {
        SceneController.LoadNextStoryScene();
    }

}

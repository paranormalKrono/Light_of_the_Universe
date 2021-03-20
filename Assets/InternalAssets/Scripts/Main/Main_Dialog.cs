using UnityEngine;

public class Main_Dialog : MonoBehaviour
{
    [SerializeField] protected System_Dialogs system_dialogs;
    [SerializeField] protected DialogCanvas dialogCanvas;
    [SerializeField] private TextAsset dialogText;

    private bool isEnd;

    private void Awake()
    {
        GameManager.Initialize();
        GameMenu.DisactivateGameMenuEvent();

        system_dialogs.Initialise(GameText.GetDialogEvent(dialogText), GameText.GetNamesEvent());
        system_dialogs.EndEvent = End;

        dialogCanvas.InstantlyClose();

        bAwake();
    }

    private void Update()
    {
        if (!isEnd)
        {
            if (Input.GetKey(KeyCode.N))
            {
                End();
            }
        }
    }

    protected virtual void bAwake() { }

    private void End()
    {
        if (!isEnd)
        {
            isEnd = true;
            SceneController.LoadNextStoryScene();
        }
    }

}

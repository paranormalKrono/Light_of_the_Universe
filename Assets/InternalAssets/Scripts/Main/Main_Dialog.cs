using System.Collections;
using UnityEngine;

public class Main_Dialog : MonoBehaviour
{
    [SerializeField] protected System_Dialogs system_dialogs;
    [SerializeField] private TextAsset dialogText;

    private void Awake()
    {
        GameManager.Initialize();
        GameScreenDark.SetDarkEvent(true);
        GameMenu.DisactivateGameMenuEvent();

        system_dialogs.Initialise(GameText.GetDialogEvent(dialogText), GameText.GetNamesEvent());
        system_dialogs.EndEvent = End;

        bAwake();

        StartCoroutine(GameScreenDark.ITransparentEvent());
    }

    protected virtual void bAwake() { }

    private void End()
    {
        StartCoroutine(IEnd());
    }

    private IEnumerator IEnd()
    {
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        SceneController.LoadNextStoryScene();
    }
}

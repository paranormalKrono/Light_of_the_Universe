using System.Collections;
using UnityEngine;

public class Main_Bar : MonoBehaviour
{
    [SerializeField] private System_Dialogs system_dialogs;

    void Awake()
    {
        GameManager.Initialize();
        GameScreenDark.SetDarkEvent(true);

        system_dialogs.Initialise(GameText.GetBarDialogEvent(), GameText.GetNamesEvent());
        system_dialogs.EndEvent = End;

        StartCoroutine(GameScreenDark.ITransparentEvent());
    }

    private void End()
    {
        StartCoroutine(IEnd());
    }
    private IEnumerator IEnd()
    {
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        StaticSettings.isCompleteSomething = true;
        SceneController.LoadNextStoryScene();
    }
}

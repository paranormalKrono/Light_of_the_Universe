using System.Collections;
using UnityEngine;

public class Main_Bar : MonoBehaviour
{
    [SerializeField] private System_Dialogs system_dialogs;
    [SerializeField] private int nextBar;
    void Awake()
    {
        GameManager.Initialize();
        system_dialogs.EndEvent = End;
        ScreenDark.SetDarkEvent(true);
        StartCoroutine(ScreenDark.ITransparentEvent());
    }

    private void End()
    {
        StartCoroutine(IEnd());
    }
    private IEnumerator IEnd()
    {
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        StaticSettings.isCompleteSomething = true;
        StaticSettings.GameProgress = nextBar;
        SceneController.LoadScene(Scenes.Space_Base);
    }
}

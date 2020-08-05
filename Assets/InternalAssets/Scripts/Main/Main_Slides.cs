using System.Collections;
using UnityEngine;

public class Main_Slides : MonoBehaviour
{
    private System_Slides[] systemSlides;
    private void Awake()
    {
        GameManager.Initialize();
        systemSlides = GetComponentsInChildren<System_Slides>();
        foreach (System_Slides SS in systemSlides)
        {
            SS.gameObject.SetActive(false);
        }
        systemSlides[StaticSettings.SlidesID].gameObject.SetActive(true);
        systemSlides[StaticSettings.SlidesID].EndEvent = EndEvent;

        GameMenu.DisactivateGameMenuEvent();
        ScreenDark.SetDarkEvent(true);
        StartCoroutine(ScreenDark.ITransparentEvent());
    }

    private void EndEvent()
    {
        StartCoroutine(IEnd());
    }
    private IEnumerator IEnd()
    {
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        SceneController.LoadScene(StaticSettings.nextSceneToLoad);
    }
}

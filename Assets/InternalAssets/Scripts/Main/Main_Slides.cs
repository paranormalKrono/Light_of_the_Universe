using System.Collections;
using UnityEngine;

public class Main_Slides : MonoBehaviour
{

    private System_Slides[] systemSlides;

    private void Awake()
    {
        GameManager.Initialize();
        GameScreenDark.SetDarkEvent(true);
        systemSlides = GetComponentsInChildren<System_Slides>();
        foreach (System_Slides SS in systemSlides)
        {
            SS.gameObject.SetActive(false);
        }
        int id = SceneController.SlidesID;
        systemSlides[id].gameObject.SetActive(true);
        systemSlides[id].EndEvent = EndEvent;

        GameMenu.DisactivateGameMenuEvent();
        StartCoroutine(GameScreenDark.ITransparentEvent());
    }

    private void EndEvent()
    {
        StartCoroutine(IEnd());
    }

    private IEnumerator IEnd()
    {
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        SceneController.LoadNextStoryScene();
    }
}

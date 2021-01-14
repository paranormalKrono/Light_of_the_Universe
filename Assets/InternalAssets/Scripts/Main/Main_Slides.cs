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
        int id = SceneController.SlidesID;
        systemSlides[id].Initialise();
        systemSlides[id].gameObject.SetActive(true);
        systemSlides[id].EndEvent = EndEvent;

        GameMenu.DisactivateGameMenuEvent();
    }

    private void EndEvent()
    {
        SceneController.LoadNextStoryScene();
    }
}

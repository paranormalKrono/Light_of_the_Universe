using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Main_Equations : MonoBehaviour
{
    [SerializeField] private Button EndButton;
    [SerializeField] private System_Equation[] systemEquations;
    private bool isEnd;
    private void Awake()
    {
        GameManager.Initialize();
        foreach (System_Equation SS in systemEquations)
        {
            SS.gameObject.SetActive(false);
        }
        systemEquations[StaticSettings.EquationsID].gameObject.SetActive(true);
        systemEquations[StaticSettings.EquationsID].RightEvent = EquationsRight;

        GameMenu.DisactivateGameMenuEvent();
        ScreenDark.SetDarkEvent(true);
        StartCoroutine(ScreenDark.ITransparentEvent());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(IEnd());
        }
    }
    private void EquationsRight()
    {
        EndButton.interactable = true;
    }
    public void UIEnd()
    {
        StartCoroutine(IEnd());
    }
    private IEnumerator IEnd()
    {
        if (!isEnd)
        {
            isEnd = true;
            yield return StartCoroutine(ScreenDark.IDarkEvent());
            if (StaticSettings.isNextSlides)
            {
                StaticSettings.isNextSlides = false;
                SceneController.LoadSlides();
            }
            else
            {
                SceneController.LoadScene(StaticSettings.nextSceneToLoad);
            }
        }
    }
}

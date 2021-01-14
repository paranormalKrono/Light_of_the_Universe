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
        int id = SceneController.EquationsID;
        systemEquations[id].gameObject.SetActive(true);
        systemEquations[id].RightEvent = EquationsRight;

        GameMenu.DisactivateGameMenuEvent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            UIEnd();
        }
    }
    private void EquationsRight()
    {
        EndButton.interactable = true;
    }
    public void UIEnd()
    {
        if (!isEnd)
        {
            isEnd = true;
            SceneController.LoadNextStoryScene();
        }
    }
}

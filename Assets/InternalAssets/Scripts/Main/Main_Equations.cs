using UnityEngine;
using UnityEngine.UI;



public class Main_Equations : MonoBehaviour
{
    [SerializeField] private Button EndButton;
    [SerializeField] private Button EquationsButton;
    [SerializeField] private GameObject TheoryPanel;
    [SerializeField] private GameObject TaskPanel;
    [SerializeField] private GameObject[] TheoryPanels;
    [SerializeField] private System_Equation[] systemEquations;

    private bool isEnd;
    private bool isTheory;
    private int id;
    private void Awake()
    {
        GameManager.Initialize();

        id = SceneController.EquationsID;
        if (id == 0 || id == 1 || id == 5 || id == 11)
        {
            isTheory = true;
            TheoryPanel.SetActive(true);
            if (id == 0)
            {
                TheoryPanels[0].SetActive(true);
            }
            else if (id == 1)
            {
                TheoryPanels[1].SetActive(true);
            }
            else if (id == 5)
            {
                TheoryPanels[2].SetActive(true);
            }
            else if (id == 11)
            {
                TheoryPanels[3].SetActive(true);
            }
            EndButton.interactable = true;
        }
        else
        {
            TaskPanel.SetActive(true);
            if (id > 5)
            {
                if (id > 11)
                {
                    id -= 1;
                }
                id -= 1;
            }
            id -= 2;
            systemEquations[id].gameObject.SetActive(true);
        }

        GameMenu.DisactivateGameMenuEvent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            UIEnd();
        }
    }
    public void UIEnd()
    {
        if (!isEnd)
        {
            isEnd = true;
            if (!isTheory)
            {
                systemEquations[id].CheckRight();
            }
            SceneController.LoadNextStoryScene();
        }
    }
}

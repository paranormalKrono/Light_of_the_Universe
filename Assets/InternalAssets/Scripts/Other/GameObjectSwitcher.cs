using UnityEngine;
using UnityEngine.UI;

public class GameObjectSwitcher : MonoBehaviour
{

    [SerializeField] private GameObject[] gameObjects;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    private int currentId = 0;

    private void Start()
    {
        prevButton.interactable = false;
        if (gameObjects.Length == 1)
        {
            nextButton.interactable = false;
        }
        gameObjects[currentId].SetActive(true);

        prevButton.onClick.AddListener(() => Previous());
        nextButton.onClick.AddListener(() => Next());
    }

    public void Next()
    {
        gameObjects[currentId].SetActive(false);
        if (currentId == 0)
        {
            prevButton.interactable = true;
        }
        currentId += 1;
        if (currentId == gameObjects.Length - 1)
        {
            nextButton.interactable = false;
        }
        gameObjects[currentId].SetActive(true);
    }

    public void Previous()
    {
        gameObjects[currentId].SetActive(false);
        if (currentId == gameObjects.Length - 1)
        {
            nextButton.interactable = true;
        }
        currentId -= 1;
        if (currentId == 0)
        {
            prevButton.interactable = false;
        }
        gameObjects[currentId].SetActive(true);
    }

}

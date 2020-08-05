using UnityEngine;
using UnityEngine.UI;

public class System_News : MonoBehaviour
{
    [SerializeField] private GameObject GameObjectText;
    [SerializeField] private Text NewsText;
    [SerializeField] private Button NextNewsButton;
    [SerializeField] private Button PreviousNewsButton;

    private string[] News;

    private int lastNewsID;
    private int currentNewsID;


    internal void Initialize(int lastNewsID, Color color)
    {
        GameObjectText.SetActive(true);
        News = GameText.GetBaseNewsEvent();
        this.lastNewsID = lastNewsID;
        NewsText.color = color;
        if (lastNewsID == 0)
        {
            PreviousNewsButton.interactable = false;
        }
        NextNewsButton.interactable = false;
        currentNewsID = lastNewsID;
        LoadNews(currentNewsID);
    }
    private void LoadNews(int newsID)
    {
        NewsText.text = News[newsID];
    }

    public void NextNews()
    {
        if (currentNewsID == 0)
        {
            PreviousNewsButton.interactable = true;
        }
        currentNewsID += 1;
        if (currentNewsID == lastNewsID)
        {
            NextNewsButton.interactable = false;
        }
        LoadNews(currentNewsID);
    }

    public void PreviousNews()
    {
        if (currentNewsID == lastNewsID)
        {
            NextNewsButton.interactable = true;
        }
        currentNewsID -= 1;
        if (currentNewsID == 0)
        {
            PreviousNewsButton.interactable = false;
        }
        LoadNews(currentNewsID);
    }
}
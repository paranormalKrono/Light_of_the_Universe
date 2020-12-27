using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class System_Dialogs : MonoBehaviour
{
    [SerializeField] private ScrollRect dialogScrollRect;
    [SerializeField] private RectTransform artistPrefab;
    [SerializeField] private RectTransform descriptionPrefab;
    [SerializeField] private RectTransform answerPrefab;

    [SerializeField] private RectTransform dialogContent;
    [SerializeField] private RectTransform answersContent;

    [SerializeField] private int playerNameID;

    private List<Button> answerButtons;

    private TextDialog dialog;
    private TextNames names;

    private Transform CanvasTr;

    private TextDialog.Node currentNode;
    private TextNames.Name currentName;
    private int currentNodeID;

    internal delegate void Event();
    internal event Event OnNextPage;
    internal Event EndEvent;

    private void Awake()
    {
        CanvasTr = GetComponentInParent<Canvas>().transform;
    }

    public void Initialise(TextDialog dialog, TextNames names)
    {
        this.dialog = dialog;
        this.names = names;
        NextPage();
    }

    private void NextPage()
    {
        currentNode = dialog.nodes[currentNodeID];
        currentName = names.names[currentNode.nameid];
        UpdateDialog();
        UpdateAnswers();
        OnNextPage?.Invoke();
    }

    private void UpdateDialog()
    {
        if (currentNode.isDescription)
        {
            AddDescription(descriptionPrefab, dialogContent, currentNode.text);
        }
        else
        {
            AddArtistDialog(artistPrefab, dialogContent, currentName.name, currentNode.text, currentName.namecolor, currentName.textcolor);
        }
        StartCoroutine(MoveDialogDown());
    }

    private IEnumerator MoveDialogDown()
    {
        UIX.UpdateLayout(CanvasTr);
        yield return null;
        dialogScrollRect.verticalNormalizedPosition = 0f;
    }

    private void UpdateAnswers()
    {
        ClearContent(answersContent);
        answerButtons = new List<Button>();
        for (int i = 0; i < currentNode.answers.Length; ++i)
        {
            GameObject prefab = AddContent(answerPrefab, answersContent);
            prefab.GetComponentInChildren<Text>().text = currentNode.answers[i].text;
            int b = new int();
            b = i;
            answerButtons.Add(prefab.GetComponentInChildren<Button>());
            answerButtons[i].onClick.AddListener(() => Answer(b));
        }
    }

    public void Answer(int answerID)
    {
        if (!currentNode.answers[answerID].isend)
        {
            if (currentNode.isAnswer)
            {
                AddArtistDialog(artistPrefab, dialogContent, names.names[playerNameID].name, currentNode.answers[answerID].text, names.names[playerNameID].namecolor, names.names[playerNameID].textcolor);
            }
            currentNodeID = dialog.nodes[currentNodeID].answers[answerID].nextNode;
            NextPage();
        }
        else
        {
            EndEvent();
        }
    }


    private void AddArtistDialog(RectTransform prefab, RectTransform content, string name, string text, string nameColorS, string textColorS)
    {
        ColorUtility.TryParseHtmlString("#" + nameColorS, out Color nameColor);
        ColorUtility.TryParseHtmlString("#" + textColorS, out Color textColor);
        GameObject g = AddContent(prefab, content);
        Text[] t = g.GetComponentsInChildren<Text>();
        t[0].text = name;
        t[1].text = text;
        t[0].color = nameColor;
        t[1].color = textColor;
    }
    private void AddDescription(RectTransform prefab, RectTransform content, string text) => AddContent(prefab, content).GetComponent<Text>().text = text;



    private GameObject AddContent(RectTransform prefab, RectTransform content)
    {
        GameObject instance = Instantiate(prefab.gameObject);
        instance.transform.SetParent(content, false);
        return instance;
    }
    private void ClearContent(RectTransform content)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetAnswerButtonsInteractable(bool t)
    {
        if (answerButtons != null)
        {
            for (int i = 0; i < answerButtons.Count; ++i)
            {
                answerButtons[i].interactable = t;
            }
        }
    }
}
using System.Collections;
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

    private TextDialog dialog;
    private TextNames names;

    private Transform CanvasTr;

    private TextDialog.Node currentNode;
    private TextNames.Name currentName;
    private int currentNodeID;

    internal delegate void EndDelegate();
    internal EndDelegate EndEvent;

    private void Awake()
    {
        CanvasTr = GetComponentInParent<Canvas>().transform;
    }

    private void Start()
    {
        GameText.GetBarDialogEvent(out dialog, out names);
        NextPage();
    }


    private void NextPage()
    {
        currentNode = dialog.nodes[currentNodeID];
        currentName = names.names[currentNode.nameid];
        UpdateDialog();
        UpdateAnswers();
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
        StartCoroutine(enumerator());
    }

    private IEnumerator enumerator()
    {
        UIX.UpdateLayout(CanvasTr);
        yield return null;
        dialogScrollRect.verticalNormalizedPosition = 0f;
    }

    private void UpdateAnswers()
    {
        ClearContent(answersContent);
        for (int i = 0; i < currentNode.answers.Length; ++i)
        {
            GameObject prefab = AddContent(answerPrefab, answersContent);
            prefab.GetComponentInChildren<Text>().text = currentNode.answers[i].text;
            int b = new int();
            b = i;
            prefab.GetComponentInChildren<Button>().onClick.AddListener(() => OnClickAnswer(b));
        }
    }

    private void OnClickAnswer(int answerID)
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
        GameObject instance = Instantiate(prefab.gameObject) as GameObject;
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
}

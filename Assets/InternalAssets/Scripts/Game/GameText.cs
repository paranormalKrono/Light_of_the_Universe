using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class GameText : MonoBehaviour
{
    [SerializeField] private GameGoals GameGoals;
    [SerializeField] private GameDialogs GameDialogs;

    [SerializeField] private TextAsset[] InGameTextAssets;
    [SerializeField] private TextAsset[] DialogsTextAssets;
    [SerializeField] private TextAsset NamesTextAsset;
    [SerializeField] private TextAsset BarDialogTextAsset;
    [SerializeField] private TextAsset BaseNewsTextAsset;
    [SerializeField] private TextAsset BaseGoalsTextAsset;


    private TextInGame[] InGameTexts;
    private TextNames Names;
    private TextDialog BarDialog;
    private TextDialog[] Dialogs;

    private string[] News;
    private string[] Goals;
    private string goalName;
    private string goalcollectionName;


    public delegate void Event();
    public static Event DeactivateEvent;

    public delegate void EventT(TextAsset textAsset);
    public static EventT SetInGameTextNowEvent;

    public delegate TextDialog EventBarDialog();
    public static EventBarDialog GetBarDialogEvent;

    public delegate TextNames EventGetNames();
    public static EventGetNames GetNamesEvent;

    public delegate string[] EventGetNews();
    public static EventGetNews GetBaseNewsEvent;
    public delegate string[] EventGetGoals(out string goalName, out string collectionName);
    public static EventGetGoals GetBaseGoalsEvent;

    public delegate TextDialog GetDialogDelegate(TextAsset textAsset);
    public static GetDialogDelegate GetDialogEvent;

    public static event Event OnDeactivate;

    private void Awake()
    {
        IDeactivated[] deactivateds = transform.parent.GetComponentsInChildren<IDeactivated>();
        for (int i = 0; i < deactivateds.Length; ++i)
        {
            OnDeactivate += deactivateds[i].Deactivate;
        }
        DeactivateEvent = OnDeactivate;

        SetInGameTextNowEvent = SetInGameTextNow;
        GetBarDialogEvent = GetBarDialog;
        GetBaseNewsEvent = GetBaseNews;
        GetBaseGoalsEvent = GetBaseGoals;
        GetNamesEvent = GetNames;

        InGameTexts = new TextInGame[InGameTextAssets.Length];
        for (int i = 0; i < InGameTextAssets.Length; ++i)
        {
            LoadXML(InGameTextAssets[i], out InGameTexts[i]);
        }
        Dialogs = new TextDialog[DialogsTextAssets.Length];
        for (int i = 0; i < DialogsTextAssets.Length; ++i)
        {
            LoadXML(DialogsTextAssets[i], out Dialogs[i]);
        }
        LoadXML(NamesTextAsset, out Names);
        LoadXML(BarDialogTextAsset, out BarDialog);

        TextBaseNews BaseNews;
        LoadXML(BaseNewsTextAsset, out BaseNews);
        News = UnboxNews(BaseNews);

        TextBaseGoals BaseGoals;
        LoadXML(BaseGoalsTextAsset, out BaseGoals);
        Goals = UnboxGoals(BaseGoals);

        GetDialogEvent = GetDialog;

        goalName = BaseGoals.goalname;
        goalcollectionName = BaseGoals.collectionname;

        GameDialogs.Names = Names;
    }



    private void SetInGameTextNow(TextAsset textAsset)
    {
        for (int i = 0; i < InGameTextAssets.Length; ++i)
        {
            if (InGameTextAssets[i] == textAsset)
            {
                GameDialogs.SetDialogTextAsset(InGameTexts[i]);
                GameGoals.SetGoals(InGameTexts[i]);
                break;
            }
        }
    }

    private TextDialog GetBarDialog() => BarDialog;

    private TextNames GetNames() => Names;

    private string[] GetBaseNews() => News;
    private string[] GetBaseGoals(out string goalName, out string collectionName)
    {
        goalName = this.goalName;
        collectionName = goalcollectionName;
        return Goals;
    }

    private TextDialog GetDialog(TextAsset textAsset)
    {
        for (int i = 0; i < DialogsTextAssets.Length; ++i)
        {
            if (textAsset == DialogsTextAssets[i])
            {
                return Dialogs[i];
            }
        }
        return Dialogs[0];
    }

    private string[] UnboxNews(TextBaseNews textBaseNews)
    {
        string[] News = new string[textBaseNews.news.Length];
        for (int w = 0; w < News.Length; ++w)
        {
            TextBaseNews.News news = textBaseNews.news[w];
            string s = "";
            TextBaseNews.News.Item item;
            for (int i = 0; i < news.items.Length; ++i)
            {
                item = news.items[i];
                s += item.header + "\n";
                if (item.texts != null)
                {
                    for (int j = 0; j < item.texts.Length; ++j)
                    {
                        s += "\t" + item.texts[j] + "\n";
                    }
                }
                s += "\n";
            }
            if (news.texts != null)
            {
                for (int i = 0; i < news.texts.Length; ++i)
                {
                    s += news.texts[i] + "\n";
                }
            }
            News[w] = s;
        }
        return News;
    }

    private string[] UnboxGoals(TextBaseGoals TextBaseGoals)
    {
        string[] Goals = new string[TextBaseGoals.goals.Length];
        for (int w = 0; w < Goals.Length; ++w)
        {
            TextBaseGoals.Goal goal = TextBaseGoals.goals[w];
            string s = string.Format("{0} №{1}\n\n{2}\n",
                TextBaseGoals.missionnames[goal.missionnameid],
                goal.goalid,
                goal.header);
            for (int i = 0; i < goal.texts.Length; ++i)
            {
                s += goal.texts[i] + "\n";
            }
            s += string.Format("\n{0}\n{1}\n\n{2}\n\t+{3} {4}",
                TextBaseGoals.fieldname,
                TextBaseGoals.fielddescriptions[goal.fielddescriptionid],
                TextBaseGoals.reward,
                SpaceMissions.GetSpaceMissionData(w).reward,
                TextBaseGoals.rewardtext);
            Goals[w] = s;
        }
        return Goals;
    }

    private static void LoadXML<T>(TextAsset _xml, out T textClass) where T : class
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringReader reader = new StringReader(_xml.text);
        textClass = serializer.Deserialize(reader) as T;
    }
}
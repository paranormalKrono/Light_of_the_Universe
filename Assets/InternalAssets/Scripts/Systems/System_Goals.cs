using UnityEngine;
using UnityEngine.UI;

public class System_Goals : MonoBehaviour
{
    [SerializeField] private Text GoalCollectionText;
    [SerializeField] private Text GoalNowText;
    [SerializeField] private Text GoalText;
    [SerializeField] private GameObject EntityNextMission;
    [SerializeField] private GameObject EntityPreviousMission;
    [SerializeField] private Button NextMissionImage;
    [SerializeField] private Button PreviousMissionImage;

    private string[] Goals;
    private string goalName;

    private int firstMission;
    private int endMission;
    private int goalNow;

    internal void Initialize(int missionCollectionID, int firstMissionID, int count)
    {
        GoalText.enabled = true;

        firstMission = firstMissionID;
        endMission = firstMission + count - 1;
        goalNow = firstMission;

        EntityPreviousMission.SetActive(false);
        PreviousMissionImage.interactable = false;

        if (count == 1)
        {
            EntityNextMission.SetActive(false);
            NextMissionImage.interactable = false;
        }
        string collectionName;
        Goals = GameText.GetBaseGoalsEvent(out goalName, out collectionName);
        GoalCollectionText.text = collectionName + " " + (missionCollectionID + 1);
        GoalNowText.text = goalName + " №1"; 

        LoadGoal(goalNow);
    }

    internal void LoadGoal(int id) => GoalText.text = Goals[id];

    public void NextMissionText()
    {
        if (goalNow == firstMission)
        {
            EntityPreviousMission.SetActive(true);
            PreviousMissionImage.interactable = true;
        }
        goalNow += 1;
        if (goalNow == endMission)
        {
            EntityNextMission.SetActive(false);
            NextMissionImage.interactable = false;
        }
        LoadGoal(goalNow);
        GoalNowText.text = goalName + " №" + (goalNow - firstMission + 1).ToString();
    }

    public void PreviousMissionText()
    {
        if (goalNow == endMission)
        {
            EntityNextMission.SetActive(true);
            NextMissionImage.interactable = true;
        }
        goalNow -= 1;
        if (goalNow == firstMission)
        {
            EntityPreviousMission.SetActive(false);
            PreviousMissionImage.interactable = false;
        }
        LoadGoal(goalNow);
        GoalNowText.text = goalName + " №" + (goalNow - firstMission + 1).ToString();
    }
}
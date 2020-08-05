using UnityEngine;
using UnityEngine.UI;

public class GameGoals : MonoBehaviour, IDeactivated
{
    [SerializeField] private Text Goal; // Цель
    [SerializeField] private Text GoalText; // Текст цели

    private string[] Goals;
    private int goalNow;

    public delegate void Event();
    public static Event NextGoalEvent;

    public delegate void EventB(bool b);
    public static EventB SetActiveGoalEvent;

    private void Awake()
    {
        NextGoalEvent = NextGoal;
        SetActiveGoalEvent = SetActiveGoal;
    }

    private void NextGoal()
    {
        goalNow += 1;
        UpdateGoal();
    }

    private void SetActiveGoal(bool t)
    {
        GoalText.enabled = t;
        Goal.enabled = t;
        if (t)
        {
            UpdateGoal();
        }
    }

    private void UpdateGoal() => GoalText.text = Goals[goalNow];

    internal void SetGoals(TextInGame textInGame) => Goals = textInGame.goals;

    public void Deactivate()
    {
        SetActiveGoal(false);
        goalNow = 0;
    }
}
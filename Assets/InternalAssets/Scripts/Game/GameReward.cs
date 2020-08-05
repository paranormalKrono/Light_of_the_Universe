using UnityEngine;
using UnityEngine.UI;

public class GameReward : MonoBehaviour, IDeactivated
{
    [SerializeField] private Text RewardText;
    [SerializeField] private Text PostRewardText;

    public delegate void EventR(int reward);
    public static EventR ShowRewardEvent;

    public delegate void Event();
    public static Event HideRewardEvent;

    private void Awake()
    {
        ShowRewardEvent = ShowReward;
        HideRewardEvent = Deactivate;
    }

    private void ShowReward(int reward)
    {
        PostRewardText.enabled = true;
        RewardText.text = "+" + reward;
        RewardText.enabled = true;
    }

    public void Deactivate()
    {
        PostRewardText.enabled = false;
        RewardText.enabled = false;
    }
}

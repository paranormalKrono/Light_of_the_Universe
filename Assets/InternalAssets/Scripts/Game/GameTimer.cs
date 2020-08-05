using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour, IDeactivated
{
    [SerializeField] private GameObject Timer;
    [SerializeField] private Text TimerText;

    private string timeText;

    public delegate void Event();
    private static Event OnTimerEnd;
    public static Event DeactivateEvent;
    public static Event StopTimerEvent;
    public static Event StartIncreasingTimerEvent;

    public delegate void EventT(float time);
    public static EventT ShowTimerEvent;

    public delegate void EventFE(float time, Event OnTimerEndEvent);
    public static EventFE StartDecreasingTimerEvent;


    private void Awake()
    {
        StopTimerEvent = StopTimer;
        ShowTimerEvent = ShowTimer;
        StartDecreasingTimerEvent = StartDecreasingTimer;
        StartIncreasingTimerEvent = StartIncreasingTimer;
        DeactivateEvent = Deactivate;
    }

    private void ShowTimer(float time)
    {
        SetTimer(time);
        SetTimerEnabled(true);
    }

    private void StartDecreasingTimer(float time, Event OnTimerEndEvent)
    {
        StopTimer();
        OnTimerEnd = OnTimerEndEvent;
        StartCoroutine(TimerDecreasing(time));
    }
    private void StartIncreasingTimer()
    {
        StopTimer();
        StartCoroutine(TimerIncreasing());
    }



    private IEnumerator TimerDecreasing(float time)
    {
        SetTimer(time);
        SetTimerEnabled(true);
            while (time > 0)
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    SetTimer(0);
                    break;
                }
                else
                {
                    SetTimer(time);
                }
                yield return null;
            }
        OnTimerEnd();
    }

    private IEnumerator TimerIncreasing()
    {
        SetTimer(0);
        SetTimerEnabled(true);
        float time = 0;
        while (time < 3600)
        {
            time += Time.deltaTime;
            if (time >= 3600)
            {
                SetTimer(3599);
                break;
            }
            else
            {
                SetTimer(time);
            }
            yield return null;
        }
    }


    private void SetTimer(float time)
    {
        timeText = ((time - time % 60) / 60).ToString("00") + ":" + (time % 60 - time % 1).ToString("00");
        TimerText.text = timeText;
    }



    private void StopTimer()
    {
        StopAllCoroutines();
    }
    public void Deactivate()
    {
        StopAllCoroutines();
        SetTimerEnabled(false);
    }
    private void SetTimerEnabled(bool t) => Timer.SetActive(t);
}

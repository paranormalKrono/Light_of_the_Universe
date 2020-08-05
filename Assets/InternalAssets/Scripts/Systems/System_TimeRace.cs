using UnityEngine;

public class System_TimeRace : MonoBehaviour
{
    [SerializeField] private Transform PointsTr;
    [SerializeField] private float time = 180;
    [SerializeField] private bool isReverse;

    private RacePoint[] RacePoints;
    private int PointsPassed = 0;

    public delegate void Event();
    private Event OnEnd;


    private void Awake()
    {
        RacePoints = PointsTr.GetComponentsInChildren<RacePoint>();
    }

    public void ShowTimer() => GameTimer.ShowTimerEvent(time);


    public void Activate(GameTimer.Event OnTimeOutEvent, Event OnEndEvent)
    {
        OnEnd = OnEndEvent;
        if (isReverse)
        {
            PointsPassed = RacePoints.Length - 1;
        }
        RacePoints[PointsPassed].ActivateTimeRace(PointPass);
        GameTimer.StartDecreasingTimerEvent(time, OnTimeOutEvent);
    }

    private void PointPass()
    {
        RacePoints[PointsPassed].DisActivateTimeRace();
        if (isReverse && PointsPassed > 0 || !isReverse && PointsPassed < RacePoints.Length - 1)
        {
            if (isReverse)
            {
                PointsPassed -= 1;
            }
            else
            {
                PointsPassed += 1;
            }
            RacePoints[PointsPassed].ActivateTimeRace(PointPass);
        }
        else
        {
            OnEnd();
        }
    }
}

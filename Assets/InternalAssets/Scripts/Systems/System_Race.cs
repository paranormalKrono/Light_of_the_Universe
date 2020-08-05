using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class System_Race : MonoBehaviour
{
    [SerializeField] private Transform PointsTr;
    [SerializeField] private bool isReverse;

    private List<Transform> Racers;
    private List<RacePoint> RacePoints;
    private List<int> PointsPassed = new List<int>();

    public delegate void Event(Transform RacerTr);
    private Event OnEnd;


    private void Awake()
    {
        RacePoints = PointsTr.GetComponentsInChildren<RacePoint>().ToList();
        for (int i = 0; i < RacePoints.Count; ++i)
        {
            PointsPassed.Add(new int());
            RacePoints[i].ActivateRace(PointPass,i);
            if (isReverse)
            {
                PointsPassed[i] = RacePoints.Count - 1;
            }
        }
    }

    public void Initialize(List<Transform> racers)
    {
        Racers = racers;
    }

    public void StartRace(Event OnEndEvent)
    {
        OnEnd = OnEndEvent;
        if (isReverse)
        {
            RacePoints[RacePoints.Count - 1].SetMarkerActive(true);
        }
        else
        {
            RacePoints[0].SetMarkerActive(true);
        }
        GameTimer.StartIncreasingTimerEvent();
    }

    private int racerID;
    private void PointPass(Transform RacerTr, int PointID)
    {
        racerID = Racers.IndexOf(RacerTr);
        if (racerID > -1) // Объект является гонщиком
        {
            if (PointID == PointsPassed[racerID]) // Если это следующий вейпоинт для гонщика
            {
                if (isReverse && PointsPassed[racerID] > 0 || !isReverse && PointsPassed[racerID] < RacePoints.Count - 1)
                {
                    if (RacerTr.GetComponent<Player_Starship_Controller>() != null)
                    {
                        RacePoints[PointsPassed[racerID]].SetMarkerActive(false);
                        if (isReverse)
                        {
                            RacePoints[PointsPassed[racerID] - 1].SetMarkerActive(true);
                        }
                        else
                        {
                            RacePoints[PointsPassed[racerID] + 1].SetMarkerActive(true);
                        }
                    }
                    if (isReverse)
                    {
                        PointsPassed[racerID] -= 1;
                    }
                    else
                    {
                        PointsPassed[racerID] += 1;
                    }
                }

                else
                {
                    OnEnd(RacerTr);
                }
            }
        }
    }

    public void ShowTimer() => GameTimer.ShowTimerEvent(0);
}
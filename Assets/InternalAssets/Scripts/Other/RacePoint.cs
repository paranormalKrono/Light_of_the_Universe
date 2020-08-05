using UnityEngine;

public class RacePoint : MonoBehaviour
{
    [SerializeField] private GameObject MinimapMarker;
    private int ID;

    private bool isActiveRace;
    private bool isActiveTimeRace;

    internal delegate void EventRacer(Transform RacerTr, int PointID);
    private EventRacer OnRacerEnter;
    internal delegate void EventPlayer();
    private EventPlayer OnPlayerEnter;


    private void OnTriggerEnter(Collider other)
    {
        if (isActiveRace)
        {
            OnRacerEnter(other.transform.root, ID);
        }
        if (isActiveTimeRace)
        {
            if (other.transform.root.GetComponent<Player_Starship_Controller>() != null)
            {
                OnPlayerEnter();
            }
        }
    }

    internal void ActivateRace(EventRacer @event, int ID)
    {
        this.ID = ID;
        OnRacerEnter = @event;
        isActiveRace = true;
    }
    internal void ActivateTimeRace(EventPlayer @event)
    {
        OnPlayerEnter = @event;
        MinimapMarker.SetActive(true);
        isActiveTimeRace = true;
    }
    internal void DisActivateTimeRace()
    {
        MinimapMarker.SetActive(false);
        isActiveTimeRace = false;
    }

    internal void SetMarkerActive(bool t)
    {
        MinimapMarker.SetActive(t);
    }
}

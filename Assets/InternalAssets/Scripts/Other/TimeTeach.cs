using System.Collections;
using UnityEngine;

public class TimeTeach : MonoBehaviour
{
    [SerializeField] private float teachTime = 10;
    [SerializeField] private float timeToEndTeach = 1;
    [SerializeField] private Color emissionColor = Color.green;
    [SerializeField] private PlayerStarshipTrigger TriggerStart;
    [SerializeField] private PlayerStarshipTrigger TriggerEnd;
    [SerializeField] private EmissionColorChanger EmissionChanger;
    [SerializeField] private Door DoorTeach;
    [SerializeField] private AudioSource audioSource;

    internal delegate void Method();
    internal event Method OnLoseTeach;

    internal void Activate()
    {
        TriggerStart.OnPlayerStarshipEnter += StartTeach;
    }
    internal void InstantlyTeach()
    {
        DoorTeach.InstantlyMove();
        EmissionChanger.ChangeEmissionColor(emissionColor);
    }

    private void StartTeach()
    {
        TriggerStart.OnPlayerStarshipEnter -= StartTeach;
        TriggerEnd.OnPlayerStarshipEnter += EndTeach;
        GameTimer.StartDecreasingTimerEvent(teachTime, TeachLose);
    }
    private void EndTeach()
    {
        StartCoroutine(IEndTeach());
    }

    private IEnumerator IEndTeach()
    {
        TriggerEnd.OnPlayerStarshipEnter -= EndTeach;
        GameTimer.StopTimerEvent();
        EmissionChanger.ChangeEmissionColor(emissionColor);
        audioSource.Play();
        yield return new WaitForSeconds(timeToEndTeach);
        GameTimer.DeactivateEvent();
        DoorTeach.Move();
    }


    private void TeachLose()
    {
        OnLoseTeach?.Invoke();
    }
}

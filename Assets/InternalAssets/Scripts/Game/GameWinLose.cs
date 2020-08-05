using UnityEngine;

public class GameWinLose : MonoBehaviour, IDeactivated
{
    [SerializeField] private GameObject WinText;
    [SerializeField] private GameObject LoseText;

    public delegate void Event();
    public static Event ActivateWinTextEvent;
    public static Event ActivateLoseTextEvent;
    public static Event DisactivateTextEvent;

    private void Awake()
    {
        ActivateWinTextEvent = ActivateWinText;
        ActivateLoseTextEvent = ActivateLoseText;
        DisactivateTextEvent = Deactivate;
    }

    private void ActivateWinText()
    {
        LoseText.SetActive(false);
        WinText.SetActive(true);
    }

    private void ActivateLoseText()
    {
        WinText.SetActive(false);
        LoseText.SetActive(true);
    }

    public void Deactivate()
    {
        WinText.SetActive(false);
        LoseText.SetActive(false);
    }
}

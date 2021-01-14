using System.Collections;
using UnityEngine;

public class GameScreenDark : MonoBehaviour
{
    [SerializeField] private ScreenDark screenDark;

    public delegate void EventHandler();
    public delegate void EventHandlerE(EventHandler @event);
    static public EventHandlerE DarkEvent;
    static public EventHandlerE TransparentEvent;

    private void Awake()
    {
        DarkEvent = Dark;
        TransparentEvent = Transparent;
    }



    private void Dark(EventHandler @event)
    {
        StopAllCoroutines();
        StartCoroutine(IDark(@event));
    }
    private IEnumerator IDark(EventHandler @event)
    {
        yield return StartCoroutine(screenDark.IDark());
        @event.Invoke();
    }


    private void Transparent(EventHandler @event)
    {
        StopAllCoroutines();
        StartCoroutine(ITransparent(@event));
    }
    private IEnumerator ITransparent(EventHandler @event)
    {
        yield return StartCoroutine(screenDark.ITransparent());
        @event.Invoke();
    }

}

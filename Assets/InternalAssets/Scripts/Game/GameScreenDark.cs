using System.Collections;
using UnityEngine;

public class GameScreenDark : MonoBehaviour
{
    [SerializeField] private ScreenDark screenDark;

    public delegate void EventHandlerB(bool b);
    public static EventHandlerB SetDarkEvent;
    public delegate IEnumerator IEnumeratorHandler();
    public static IEnumeratorHandler IDarkEvent;
    public static IEnumeratorHandler ITransparentEvent;

    private void Awake()
    {
        SetDarkEvent = screenDark.SetDark;
        IDarkEvent = screenDark.IDark;
        ITransparentEvent = screenDark.ITransparent;
    }
}

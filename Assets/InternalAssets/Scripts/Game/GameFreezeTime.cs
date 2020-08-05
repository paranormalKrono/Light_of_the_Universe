using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameFreezeTime : MonoBehaviour, IDeactivated
{
    [SerializeField] private Text FreezeTimeText;

    public delegate void Event();
    public delegate IEnumerator EventFreezeTime();
    public static EventFreezeTime IFreezeTimeEvent;

    private void Awake()
    {
        IFreezeTimeEvent = IFreezeTime;
    }

    private IEnumerator IFreezeTime()
    {
        FreezeTimeText.enabled = true;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                break;
            }
            yield return null;
        }
        FreezeTimeText.enabled = false;
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        FreezeTimeText.enabled = false;
    }
}

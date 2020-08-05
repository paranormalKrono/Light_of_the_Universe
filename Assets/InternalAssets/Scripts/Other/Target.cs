using UnityEngine;

public class Target : MonoBehaviour
{
    private Light li;
    private bool isActived;
    internal delegate void TargetDelegate();
    internal TargetDelegate TargetEvent;

    private void Awake()
    {
        li = GetComponentInChildren<Light>();
        li.color = Color.red;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isActived && collision.gameObject.CompareTag("Bullet"))
        {
            isActived = true;
            li.color = Color.green;
            TargetEvent();
        }
    }
}

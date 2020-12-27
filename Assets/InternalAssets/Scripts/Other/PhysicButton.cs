using UnityEngine;

public class PhysicButton : MonoBehaviour
{
    public delegate void ButtonPush();
    public event ButtonPush OnButtonPushed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Button"))
        {
            OnButtonPushed?.Invoke();
        }
    }
}

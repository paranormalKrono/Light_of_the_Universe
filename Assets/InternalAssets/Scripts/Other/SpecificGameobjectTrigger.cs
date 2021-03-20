using UnityEngine;

public class SpecificGameobjectTrigger : MonoBehaviour
{
    [SerializeField] private GameObject SpecificObject;

    public delegate void Delegate();
    public event Delegate OnSpecificObjectEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == SpecificObject)
        {
            OnSpecificObjectEnter?.Invoke();
        }
    }
}

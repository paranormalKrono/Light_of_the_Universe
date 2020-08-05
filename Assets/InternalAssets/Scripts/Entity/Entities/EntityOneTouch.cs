using UnityEngine;
using UnityEngine.Events;

public class EntityOneTouch : MonoBehaviour, IEntity
{
    [SerializeField] private string description;
    [SerializeField] private UnityEvent Event;

    public string Description { get => description; }

    public void Interact() => Event.Invoke();
}

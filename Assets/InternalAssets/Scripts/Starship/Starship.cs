using UnityEngine;

[RequireComponent(typeof(Health))]
public class Starship : MonoBehaviour
{
    public delegate void EventBoolHandler(bool b);
    public EventBoolHandler SetFollowTarget;
    public EventBoolHandler SetLockControl;
    public EventBoolHandler SetAttack;

    public delegate void EventTransformHandler(Transform Tr);
    public event EventTransformHandler DeathEvent;
    public EventTransformHandler SetEnemyTarget;


    private void Start() => GetComponent<Health>().OnDeath += Death;


    private void Death() => DeathEvent?.Invoke(transform); // Смерть

}

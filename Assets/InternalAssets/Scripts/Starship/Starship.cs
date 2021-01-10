using UnityEngine;

[RequireComponent(typeof(Health))]
public class Starship : MonoBehaviour
{
    [SerializeField] public Transform RotationPoint;

    public delegate void EventBoolHandler(bool b);
    public EventBoolHandler SetFollowEnemy;
    public EventBoolHandler SetLockControl;
    public EventBoolHandler SetAttack;

    public delegate void EventTransformHandler(Transform Tr);
    public event EventTransformHandler DeathEvent;
    public EventTransformHandler SetEnemyTarget;
    public EventTransformHandler SetFollowTarget;


    private void Start() => GetComponent<Health>().OnDeath += Death;


    private void Death() => DeathEvent?.Invoke(transform); // Смерть

}

using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private bool isHasHealth = false;
    [SerializeField] private bool isAutoFindingRigidbodies = true;
    [SerializeField] private Rigidbody[] rigidbodies;
    internal Rigidbody[] Rigidbodies { get { return rigidbodies; } }
    private Health Health;

    private void Awake()
    {
        if (isHasHealth)
        {
            Health = GetComponent<Health>();
        }
        if (isAutoFindingRigidbodies)
        {
            rigidbodies = GetComponentsInChildren<Rigidbody>();
        }
    }

    internal void TakeDamage(float Damage)
    {
        if (isHasHealth)
        {
            Health.TakeDamage(Damage);
        }
    }

}

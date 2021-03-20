using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private bool isHasHealth = false;
    [SerializeField] private bool isAutoFindingRigidbodies = true;
    [SerializeField] private Rigidbody[] rigidbodies;
    internal Rigidbody[] Rigidbodies { get => rigidbodies;  }
    private Health Health;

    public delegate void Method(float force, bool isHit);
    public event Method OnForce;

    public bool isDisabled;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDisabled)
        {
            OnForce?.Invoke((collision.impulse / Time.fixedDeltaTime).magnitude, true);
        }
    }


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

    public void SetRigidbodies(Rigidbody[] rigidbodies) => this.rigidbodies = rigidbodies;

    public void Force(float force, bool isHit) => OnForce?.Invoke(force, isHit);

}

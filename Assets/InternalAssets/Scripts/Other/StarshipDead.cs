using UnityEngine;

[RequireComponent(typeof(Health))]
public class StarshipDead : MonoBehaviour
{
    [SerializeField] private GameObject Starship;
    [SerializeField] private Rigidbody starshipPositionRb;
    [SerializeField] private Rigidbody starshipRotationRb;
    [SerializeField] private RigidbodyConstraints rigidbodyConstraints;
    [SerializeField] private Prop starshipProp;

    private void Start()
    {
        starshipProp.isDisabled = true;
        GetComponent<Health>().OnDeath += Dead;
    }
    private void Dead()
    {
        Starship.transform.parent = null;
        Rigidbody Rb = Starship.AddComponent<Rigidbody>();
        Rb.useGravity = false;
        Rb.velocity = starshipPositionRb.velocity;
        Rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        Rb.mass = starshipRotationRb.mass;
        Rb.angularVelocity = starshipRotationRb.angularVelocity;
        Rb.constraints = rigidbodyConstraints;
        Rigidbody[] rb = new Rigidbody[1];
        rb[0] = Rb;
        starshipProp.SetRigidbodies(rb);
        starshipProp.isDisabled = false;
    }
}

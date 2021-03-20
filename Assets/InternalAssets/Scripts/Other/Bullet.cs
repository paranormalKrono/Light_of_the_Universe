using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        Enemy,
        Player
    }

    [SerializeField] private float DestroyTime = 4;
    [SerializeField] private GameObject Boom;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private BulletType bulletType;
    private float ImpulseToDestroy;
    private float damageImpulse;
    private float damage;
    private float time;

    internal void Initialize(float Damage, Vector3 velocity, float force, Vector3 direction, RigidbodyConstraints rigidbodyConstraints)
    {
        damage = Damage;
        rb.constraints = rigidbodyConstraints;
        rb.velocity = velocity;
        rb.AddForce(force * direction, ForceMode.Impulse);
        damageImpulse = force;
        ImpulseToDestroy = damageImpulse / 1.1f;
        StartCoroutine(IDestroy());
    }

    private IEnumerator IDestroy()
    {
        while (time < DestroyTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Health health = collision.gameObject.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage * (collision.impulse.magnitude / damageImpulse));
        }
        ImpulseToDestroy -= collision.impulse.magnitude;
        if (ImpulseToDestroy <= 0)
        {
            Instantiate(Boom, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    public BulletType GetBulletType() => bulletType;
}
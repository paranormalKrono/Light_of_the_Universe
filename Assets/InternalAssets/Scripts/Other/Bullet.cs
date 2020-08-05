using System.Collections;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float DestroyTime = 4;
    [SerializeField] private GameObject Boom;
    [SerializeField] private Rigidbody rb;
    private float newDamage;

    private float time;

    internal void Initialize(float Damage, Vector3 velocity, Vector3 force, RigidbodyConstraints rigidbodyConstraints)
    {
        newDamage = Damage;
        rb.constraints = rigidbodyConstraints;
        rb.velocity = velocity;
        rb.AddForce(force, ForceMode.Impulse);
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
            health.TakeDamage(newDamage);
        }
        Instantiate(Boom, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
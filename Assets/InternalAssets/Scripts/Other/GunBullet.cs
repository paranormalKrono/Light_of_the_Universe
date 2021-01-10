using UnityEngine;

public class GunBullet : MonoBehaviour, IGun
{
    [SerializeField] private float Damage = 10;
    [SerializeField] private float shootForce = 20;
    [SerializeField] private GameObject prefab;
    [SerializeField] private AudioSource audioSource;

    private Collider[] colliders;
    private RigidbodyConstraints rigidbodyConstraints;

    private Vector3 forward = Vector3.forward;

    private void Shoot(Vector3 velocity, Transform Target, out Vector3 shootReverseForce)
    {
        GameObject g = Instantiate(prefab, transform.position, transform.rotation);
        Collider collider = g.GetComponentInChildren<Collider>();
        for (int i = 0; i < colliders.Length; ++i)
        {
            Physics.IgnoreCollision(colliders[i], collider);
        }
        shootReverseForce = -transform.TransformDirection(forward) * shootForce;

        Bullet Bullet = g.GetComponent<Bullet>();
        Bullet.Initialize(Damage, velocity, transform.TransformDirection(forward) * shootForce, rigidbodyConstraints);

        audioSource.Play();
    }

    public void Initialise(Collider[] ChildrenColliders, RigidbodyConstraints rigidbodyConstraints, out Guns.ShootDelegate shoot)
    {
        colliders = ChildrenColliders;
        this.rigidbodyConstraints = rigidbodyConstraints;
        shoot = Shoot;
    }
}

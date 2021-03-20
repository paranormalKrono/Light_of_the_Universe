using UnityEngine;

public class GunRocket : MonoBehaviour, IGun
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private AudioSource audioSource;

    private Transform target;

    private Collider[] colliders;

    private Vector3 vector3Zero = Vector3.zero;

    private void Shoot(Vector3 velocity, out Vector3 shootReverseForce)
    {
        GameObject g = Instantiate(prefab, transform.position, transform.rotation);
        Collider collider = g.GetComponentInChildren<Collider>();
        for (int i = 0; i < colliders.Length; ++i)
        {
            Physics.IgnoreCollision(colliders[i], collider);
        }

        shootReverseForce = vector3Zero;

        g.GetComponent<Rocket>().Initialise(target);

        audioSource.Play();
    }

    public void Initialise(Collider[] ChildrenColliders, RigidbodyConstraints rigidbodyConstraints, out Guns.ShootDelegate shoot)
    {
        colliders = ChildrenColliders;
        shoot = Shoot;
    }

    public void SetTarget(Transform Target) => target = Target;
}

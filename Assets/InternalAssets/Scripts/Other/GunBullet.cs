using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunBullet : MonoBehaviour
{
    [SerializeField] private float Damage = 10;
    [SerializeField] private float shootForce = 20;
    [SerializeField] private GameObject BulletGameobject;

    private Collider[] colliders;
    private AudioSource gunAudioSource;

    private Vector3 forward = Vector3.forward;

    private void Awake()
    {
        GetComponentInParent<Guns>().InitializeGun(Shoot, shootForce / BulletGameobject.GetComponent<Rigidbody>().mass, out colliders);
        gunAudioSource = GetComponent<AudioSource>();
    }

    private void Shoot(RigidbodyConstraints rigidbodyConstraints, Vector3 velocity, out Vector3 shootReverseForce)
    {
        GameObject bulletGameobject = Instantiate(BulletGameobject, transform.position, transform.rotation);
        Collider bulletCollider = bulletGameobject.GetComponentInChildren<Collider>();
        for (int i = 0; i < colliders.Length; ++i)
        {
            Physics.IgnoreCollision(colliders[i], bulletCollider);
        }
        Bullet Bullet = bulletGameobject.GetComponent<Bullet>();
        shootReverseForce = -transform.TransformDirection(forward) * shootForce;
        Bullet.Initialize(Damage, velocity, transform.TransformDirection(forward) * shootForce, rigidbodyConstraints);
        gunAudioSource.Play();
    }
}

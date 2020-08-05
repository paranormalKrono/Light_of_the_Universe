using UnityEngine;

public class Starship_Velocity : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 75;
    private Transform starshipTr;
    private Rigidbody starshipRb;
    private Vector3 vector3Zero = Vector3.zero;
    private Vector3 localUp = Vector3.up;
    private Quaternion StartQuaternion;
    private Vector3 V3;

    private void Awake()
    {
        localUp = transform.TransformDirection(localUp);
        starshipTr = GameObject.FindGameObjectWithTag("Player").transform;
        starshipTr.GetComponent<Health>().DeathEvent += PlayerDead;
        transform.position = starshipTr.position;
        starshipRb = starshipTr.GetComponent<Rigidbody>();
        StartQuaternion = Quaternion.FromToRotation(localUp, starshipTr.TransformDirection(Vector3.forward));
    }

    private void PlayerDead()
    {
        Destroy(gameObject);
    }
    void Update()
    {
        transform.position = starshipTr.position;
        V3 = starshipRb.velocity;
        if (V3 != vector3Zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(localUp, V3), Time.deltaTime * rotSpeed);
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, StartQuaternion, Time.deltaTime * rotSpeed);
        }
    }
}
using UnityEngine;

public class SpaceBorder : MonoBehaviour
{
    [SerializeField] private float dropMoveForce = 600;
    private Rigidbody PlayerStarshipRb;
    private bool isActive;

    private void Start()
    {
        PlayerStarshipRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player_Starship_Controller>() != null)
        {
            isActive = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Player_Starship_Controller>() != null)
        {
            isActive = false;
        }
    }
    private void FixedUpdate()
    {
        if (isActive)
        {
            PlayerStarshipRb.AddForce(transform.forward * dropMoveForce * Time.fixedDeltaTime, ForceMode.Impulse);
        }
    }
}

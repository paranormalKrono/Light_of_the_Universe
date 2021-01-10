using UnityEngine;

public class CameraTriggerController : MonoBehaviour
{
    [SerializeField] private Transform TargetCameraTransform;

    private Player_Camera_Controller player_Camera_Controller;

    private void Start()
    {
        player_Camera_Controller = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            player_Camera_Controller.EnableTargetMove(TargetCameraTransform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            player_Camera_Controller.DisableTargetMove();
        }
    }
}

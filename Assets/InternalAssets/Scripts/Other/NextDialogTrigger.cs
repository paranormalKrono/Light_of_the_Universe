using UnityEngine;

public class NextDialogTrigger : MonoBehaviour
{
    private bool isActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated)
        {
            if (other.gameObject.GetComponentInParent<Player_Starship_Controller>() != null)
            {
                GameDialogs.NextInGameDialogEvent();
                isActivated = true;
            }
        }
    }
}

using UnityEngine;

public class Player_Character_Trigger : MonoBehaviour
{
    public delegate void PlayerEnter();
    public event PlayerEnter OnPlayerEnter;

    private bool isActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.GetComponentInParent<Player_Character_Controller>() != null)
        {
            isActivated = true;
            OnPlayerEnter?.Invoke();
        }
    }
}

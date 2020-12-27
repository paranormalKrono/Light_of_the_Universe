using UnityEngine;

public class PlayerStarshipTrigger : MonoBehaviour
{
    public delegate void StarshipEnter();
    public event StarshipEnter OnPlayerStarshipEnter;

    private bool isActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.GetComponentInParent<Player_Starship_Controller>() != null)
        {
            isActivated = true;
            OnPlayerStarshipEnter?.Invoke();
        }
    }
}

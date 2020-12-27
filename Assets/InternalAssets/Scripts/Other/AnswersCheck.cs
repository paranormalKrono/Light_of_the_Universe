using UnityEngine;
using UnityEngine.UI;

public class AnswersCheck : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private int PlanetsCount;
    private void Awake()
    {
        foreach (PlanetCoordinates PC in GetComponentsInChildren<PlanetCoordinates>())
        {
            PC.PlanetFinded = PlanetFinded;
        }
    }

    private void PlanetFinded()
    {
        PlanetsCount -= 1;
        if (PlanetsCount < 1)
        {
            button.interactable = true;
        }
    }
}

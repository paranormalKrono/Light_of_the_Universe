using UnityEngine;


[RequireComponent(typeof(Health)), RequireComponent(typeof(Guns)), RequireComponent(typeof(Player_Starship_Controller))]
public class PlayerStarshipModifications : MonoBehaviour
{

    [SerializeField] private StarshipType starship;
    public StarshipType Starship { get => starship; }

    private void Awake()
    {
        Starship_Engine starship_Engine = GetComponent<Starship_Engine>();
        Starship_RotationEngine starship_RotationEngine = GetComponent<Starship_RotationEngine>();

        StarshipData data = StarshipsModificatonsData.GetStarshipData(starship);
        float value = data.GetModificationData(ModificationName.Health).CurGradeModifier;
        if (value != 0)
        {
            GetComponent<Health>().maxHp += value;
        }
        value = data.GetModificationData(ModificationName.EnginePower).CurGradeModifier;
        if (value != 0)
        {
            starship_Engine.force += value;
        }
        value = data.GetModificationData(ModificationName.GyroPower).CurGradeModifier;
        if (value != 0)
        {
            starship_RotationEngine.force += value;
        }
        value = data.GetModificationData(ModificationName.ShootTime).CurGradeModifier;
        if (value != 0)
        {
            Guns Guns = GetComponent<Guns>();
            Guns.ShootTime = 1 / ((1 / Guns.ShootTime) + value);
        }
    }
}

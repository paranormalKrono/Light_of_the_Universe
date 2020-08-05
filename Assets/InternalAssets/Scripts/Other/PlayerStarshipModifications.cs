using UnityEngine;


[RequireComponent(typeof(Health)), RequireComponent(typeof(Guns)), RequireComponent(typeof(Player_Starship_Controller))]
public class PlayerStarshipModifications : MonoBehaviour
{

    [SerializeField] private StarshipType starship;
    public StarshipType Starship { get => starship; }

    private void Awake()
    {
        Player_Starship_Controller PSC = GetComponent<Player_Starship_Controller>();

        StarshipData data = StarshipModificatonsData.GetStarshipData(starship);
        float value = data.GetModificationData(ModificationName.Health).CurGradeModifier;
        if (value != 0)
        {
            GetComponent<Health>().maxHp += value;
        }
        value = data.GetModificationData(ModificationName.EnginePower).CurGradeModifier;
        if (value != 0)
        {
            PSC.MoveForce += value;
        }
        value = data.GetModificationData(ModificationName.GyroPower).CurGradeModifier;
        if (value != 0)
        {
            PSC.RotateForce += value;
        }
        value = data.GetModificationData(ModificationName.ShootTime).CurGradeModifier;
        if (value != 0)
        {
            Guns Guns = GetComponent<Guns>();
            Guns.ShootTime = 1 / ((1 / Guns.ShootTime) + value);
        }
    }
}

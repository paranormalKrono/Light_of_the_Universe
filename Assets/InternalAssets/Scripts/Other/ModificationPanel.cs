using UnityEngine;
using UnityEngine.UI;

public class ModificationPanel : MonoBehaviour
{

    [SerializeField] private Text TextCredits;
    [SerializeField] private Color CostEnabledColor = new Color(255, 255, 0);
    [SerializeField] private Color CostDisabledColor = new Color(10, 10, 10);

    private ModificationMenu[] ModificationMenus = new ModificationMenu[4];

    private void Awake()
    {
        ModificationMenus = GetComponentsInChildren<ModificationMenu>();
        OnCreditsChange();
    }

    public void Initialize(GameObject StarshipPrefab)
    {

        Player_Starship_Controller PSC = StarshipPrefab.GetComponent<Player_Starship_Controller>();
        Health health = StarshipPrefab.GetComponent<Health>();
        Guns guns = StarshipPrefab.GetComponent<Guns>();

        StarshipData starshipData = StarshipModificatonsData.GetStarshipData(StarshipPrefab.GetComponent<PlayerStarshipModifications>().Starship);
        InitializeModificationMenu(ModificationMenus[0], health.maxHp, starshipData, ModificationName.Health);
        InitializeModificationMenu(ModificationMenus[1], PSC.MoveForce, starshipData, ModificationName.EnginePower);
        InitializeModificationMenu(ModificationMenus[2], PSC.RotateForce, starshipData, ModificationName.GyroPower);
        InitializeModificationMenu(ModificationMenus[3], 1 / guns.ShootTime, starshipData, ModificationName.ShootTime);

    }
    private void InitializeModificationMenu(ModificationMenu ModificationMenu, float value, StarshipData StarshipData, ModificationName ModificationName)
    {
        ModificationMenu.Initialize(value, StarshipData.GetModificationData(ModificationName), TryUpgrade, CostEnabledColor, CostDisabledColor);
    }

    private bool TryUpgrade(int cost)
    {
        if (cost <= StaticSettings.credits)
        {
            StaticSettings.credits -= cost;
            OnCreditsChange();
            return true;
        }
        else
        {
            return false;
        }
    }
    private void OnCreditsChange()
    {
        TextCredits.text = StaticSettings.credits.ToString();
    }
}
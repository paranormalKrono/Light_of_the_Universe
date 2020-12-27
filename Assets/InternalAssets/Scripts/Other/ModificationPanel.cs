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
        Health health = StarshipPrefab.GetComponent<Health>();
        Starship_Engine starship_Engine = StarshipPrefab.GetComponent<Starship_Engine>();
        Starship_RotationEngine starship_RotationEngine = StarshipPrefab.GetComponent<Starship_RotationEngine>();
        Guns guns = StarshipPrefab.GetComponent<Guns>();

        StarshipData starshipData = StarshipsModificatonsData.GetStarshipData(StarshipPrefab.GetComponent<PlayerStarshipModifications>().Starship);
        InitializeModificationMenu(ModificationMenus[0], health.maxHp, starshipData, ModificationName.Health);
        InitializeModificationMenu(ModificationMenus[1], starship_Engine.force, starshipData, ModificationName.EnginePower);
        InitializeModificationMenu(ModificationMenus[2], starship_RotationEngine.force, starshipData, ModificationName.GyroPower);
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
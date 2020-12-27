using UnityEngine;
using UnityEngine.UI;

/// <summary>Интерфейс для улучшения корабля.</summary>
internal class ModificationMenu : MonoBehaviour
{
    [SerializeField] internal string FormatValue;
    [SerializeField] internal Text TextValue;
    [SerializeField] internal Image ImageFiller;
    [SerializeField] internal Image ImageCurGrade;
    [SerializeField] internal Button ButtonUpgrade;
    private UpgradeMenu[] UpgradeMenus;

    private Color CostEnabledColor;
    private Color CostDisabledColor;

    private ModificationData ModificationData;
    private float value;
    private float maxValue;
    private float modificatedValue;

    internal delegate bool EventHandlerB(int cost);
    internal EventHandlerB TryUpgrade;

    internal void Initialize(float value, ModificationData ModificationData, EventHandlerB tryUpgrade, Color CostEnabledColor, Color CostDisabledColor)
    {
        UpgradeMenus = GetComponentsInChildren<UpgradeMenu>();

        this.CostEnabledColor = CostEnabledColor;
        this.CostDisabledColor = CostDisabledColor;

        this.value = value;

        this.ModificationData = ModificationData; // Дата о модификации

        TryUpgrade = tryUpgrade; // При попытке улучшения


        for (int i = 0; i < UpgradeMenus.Length; ++i)
        {
            UpgradeMenus[i].Initialize(FormatValue, ModificationData.UpgradeDatas[i]); // Инициализируем улучшения
        }

        ButtonUpgrade.onClick.AddListener(() => Upgrade());

        maxValue = value + ModificationData.maxGradeModifier;

        UpgradeToGrade(ModificationData.curGrade); // Обновляемся до определённой модификации
    }



    internal void Upgrade()
    {
        ModificationData.UpgradeData upgradeData = ModificationData.GetCurrentUpgradeData(); // Ссылка на данные об улучшении
        if (TryUpgrade(upgradeData.Cost)) // Если можно обновиться
        {
            ModificationData.Upgrade(); // Улучшаемся


            ImageCurGrade.sprite = UpgradeMenus[ModificationData.curGrade - 1].GetSprite();
            UpgradeMenus[ModificationData.curGrade - 1].DisableCost(CostDisabledColor);
            UpgradeUpdate();
        }
    }

    internal void UpgradeToGrade(int newGrade)
    {
        if (newGrade > 0)
        {
            ModificationData.UpgradeToGrade(newGrade);
            ImageCurGrade.sprite = UpgradeMenus[newGrade-1].GetSprite();
            for (int i = 0; i < newGrade; ++i)
            {
                UpgradeMenus[i].DisableCost(CostDisabledColor);
            }
        }
        UpgradeUpdate();
    }


    private void UpgradeUpdate()
    {

        if (ModificationData.curGrade == UpgradeMenus.Length)
        {
            ButtonUpgrade.interactable = false;
        }
        else
        {
            UpgradeMenus[ModificationData.curGrade].EnableCost(CostEnabledColor);
        }

        modificatedValue = value + ModificationData.CurGradeModifier;
        TextValue.text = string.Format(FormatValue, modificatedValue)+"/"+ string.Format(FormatValue, maxValue);

        ImageFiller.fillAmount = modificatedValue / maxValue;

    }
}

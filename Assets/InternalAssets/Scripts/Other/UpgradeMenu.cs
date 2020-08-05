using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] private Text TextCost;
    [SerializeField] private Text TextValue;
    [SerializeField] private Image ImageGrade;

    internal void Initialize(string valueFormat, ModificationData.UpgradeData upgradeData)
    {
        // Здесь устанавливается текст цены, текст улучшения и действие при нажатии на кнопку
        TextValue.text = "+" + string.Format(valueFormat, upgradeData.UpgradeValue);
        TextCost.text = upgradeData.Cost.ToString();

    }
    internal void DisableCost(Color disableColor)
    {
        TextCost.color = disableColor;
    }
    internal void EnableCost(Color enableColor)
    {
        TextCost.color = enableColor;
    }
    internal Sprite GetSprite() => ImageGrade.sprite;
}

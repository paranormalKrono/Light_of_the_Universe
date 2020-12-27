using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBar;

    void Awake()
    {
        GetComponent<Health>().OnHealthChange += ChangeBarValue;
        healthBar.fillAmount = 1;
    }

    private void ChangeBarValue(float nowHp, float maxHp)
    {
        healthBar.fillAmount = nowHp / maxHp;
    }
}

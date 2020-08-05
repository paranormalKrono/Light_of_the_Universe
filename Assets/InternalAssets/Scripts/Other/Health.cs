using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHp = 100;
    [SerializeField] private bool isInvincible;
    [SerializeField] private Image healthBar;

    private bool isDead;
    private float nowHp;

    internal delegate void DeadDelegate();
    internal event DeadDelegate DeathEvent;

    void Awake()
    {
        nowHp = maxHp;
        healthBar.fillAmount = 1;
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible && !isDead)
        {
            if (nowHp > damage)
            {
                SetHealth(nowHp - damage);
            }
            else
            {
                SetHealth(0);
                Dead();
            }
        }
    }

    private void SetHealth(float value)
    {
        nowHp = value;
        healthBar.fillAmount = nowHp / maxHp;
    }

    private void Dead()
    {
        isDead = true;
        DeathEvent?.Invoke();
        Destroy(gameObject);
    }

    public void SetInvincible(bool t)
    {
        isInvincible = t;
    }
}
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHp = 100;
    [SerializeField] private bool isInvincible;

    private bool isDead;
    private float nowHp;

    internal delegate void Method();
    internal event Method OnDeath;
    internal delegate void HealthChange(float nowHp, float maxHp);
    internal event HealthChange OnHealthChange;

    void Awake()
    {
        nowHp = maxHp;
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
        OnHealthChange?.Invoke(nowHp, maxHp);
    }

    public void Kill()
    {
        if (!isDead)
        {
            SetHealth(0);
            Dead();
        }
    }

    private void Dead()
    {
        isDead = true;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public float GetHealthNow() => nowHp;

    public void SetInvincible(bool t)
    {
        isInvincible = t;
    }
}
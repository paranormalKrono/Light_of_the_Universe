using System.Collections;
using UnityEngine;

public class HealthBarHider : MonoBehaviour
{

    [SerializeField] private Health health;
    [SerializeField] private GameObject HealthUI;
    [SerializeField] private float ActivateTime = 10;

    private void Awake()
    {
        HealthUI.SetActive(false);
        health.OnHealthChange += Health_OnHealthChange;
    }

    private void Health_OnHealthChange(float nowHp, float maxHp)
    {
        StopAllCoroutines();
        StartCoroutine(IHealthTimeActivator());
    }

    private IEnumerator IHealthTimeActivator()
    {
        HealthUI.SetActive(true);
        yield return new WaitForSeconds(ActivateTime);
        HealthUI.SetActive(false);
    }
}

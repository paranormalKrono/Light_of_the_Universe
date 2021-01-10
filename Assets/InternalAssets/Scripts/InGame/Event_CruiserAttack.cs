using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Event_CruiserAttack : MonoBehaviour
{

    [SerializeField] private System_EnemyWavesAttack enemyWavesAttack;
    [SerializeField] private Transform[] Targets;
    [SerializeField] private Turret[] turrets;
    [SerializeField] private float timeToNextShoot = 15;
    [SerializeField] private Health health1;
    [SerializeField] private Health health2;
    [SerializeField] private int waveLimit1 = 3;
    [SerializeField] private int waveLimit2 = 7;
    [SerializeField] private int waveLimit3 = 13;
    [SerializeField] private float hpToNextWave = 5000;

    private int waveNow = 0;
    private int turretsAttackCount = 3;

    private bool isOneDeath;

    public delegate void EventHandler();
    private EventHandler OnEndAttack;

    private void Awake()
    {
        health1.OnDeath += OnOneDeath;
        health2.OnDeath += OnOneDeath;
        health1.OnHealthChange += OnHealthChange;
        health2.OnHealthChange += OnHealthChange;
    }

    public void StartAim()
    {
        for (int i = 0; i < turrets.Length; ++i)
        {
            turrets[i].StartAim(Targets);
        }
    }

    public void StartAttack(EventHandler @event)
    {
        OnEndAttack = @event;

        enemyWavesAttack.Initialize();
        Attack();
        NextWave();
    }



    private void OnHealthChange(float nowHp, float maxHp)
    {
        if (health1.GetHealthNow() + health2.GetHealthNow() < hpToNextWave)
        {
            health1.OnHealthChange -= OnHealthChange;
            health2.OnHealthChange -= OnHealthChange;
            StopAllCoroutines();

            waveNow = waveLimit1 + 1;
            NextWave();

            turretsAttackCount = 4;
            timeToNextShoot /= 1.1f;
            Attack();
        }
    }

    private void OnOneDeath()
    {
        if (isOneDeath)
        {
            StopAllCoroutines();
            for (int i = 0; i < turrets.Length; ++i)
            {
                turrets[i].StopAim();
            }
            enemyWavesAttack.ResetWave();
            EndAttack();
        }
        else
        {
            StopAllCoroutines();

            waveNow = waveLimit2 + 1;
            NextWave();

            turretsAttackCount = 6;
            timeToNextShoot /= 1.2f;
            Attack();

            isOneDeath = true;
        }
    }


    private void Attack() => StartCoroutine(IAttack());
    private IEnumerator IAttack()
    {
        List<Turret> turs = turrets.ToList();
        int r;
        for (int i = 0; i < turretsAttackCount; ++i)
        {
            r = Random.Range(0, turs.Count);
            turs[r].Attack();
            turs.Remove(turs[r]);
        }
        yield return new WaitForSeconds(timeToNextShoot);
        Attack();
    }

    private void NextWave() => StartCoroutine(INextWave());
    private IEnumerator INextWave()
    {
        enemyWavesAttack.ResetWave();
        yield return enemyWavesAttack.IStartWave(waveNow, NextWave);
        if (waveNow != waveLimit1 && waveNow != waveLimit2 && waveNow != waveLimit3)
        {
            waveNow += 1;
        }
    }


    private void EndAttack() => OnEndAttack();
}
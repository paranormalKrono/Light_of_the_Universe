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
    [SerializeField] private float hpTo1Level = 5000;
    [SerializeField] private float hpTo2Level = 10000;
    [SerializeField] private float hpTo3Level = 10000;
    [SerializeField] private float WaitingBeforeAttack = 5;

    private int waveNow = 0;
    private int turretsAttackCount = 3;

    public delegate void EventHandler();
    private EventHandler OnEndAttack;

    private void Awake()
    {
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
        AttackWithWait();
        NextWave();
    }



    private void OnHealthChange(float nowHp, float maxHp)
    {
        maxHp = 0;
        if (health1 != null)
        {
            maxHp += health1.GetHealthNow();
        }
        if (health2 != null)
        {
            maxHp += health2.GetHealthNow();
        }

        if (maxHp < hpTo3Level)
        {
            hpTo3Level = 0;
            StopAllCoroutines();

            waveNow = waveLimit1 + 1;
            NextWave();

            turretsAttackCount = 4;
            timeToNextShoot /= 1.05f;
            AttackWithWait();
        }
        else if (maxHp < hpTo2Level)
        {
            hpTo2Level = 0;
            StopAllCoroutines();

            waveNow = waveLimit2 + 1;
            NextWave();

            turretsAttackCount = 5;
            timeToNextShoot /= 1.1f;
            AttackWithWait();
        }
        else if (maxHp < hpTo1Level)
        {
            hpTo1Level = 0;
            StopAllCoroutines();

            waveNow = waveLimit3 + 1;
            NextWave();

            turretsAttackCount = 6;
            timeToNextShoot /= 1.15f;
            AttackWithWait();
        }
        else if (maxHp == 0)
        {
            StopAllCoroutines();
            enemyWavesAttack.End();
            for (int i = 0; i < turrets.Length; ++i)
            {
                turrets[i].StopAim();
            }
            EndAttack();
        }
    }

    private void AttackWithWait() => StartCoroutine(IAttackWithWait());
    private IEnumerator IAttackWithWait()
    {
        yield return new WaitForSeconds(WaitingBeforeAttack);
        Attack();
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
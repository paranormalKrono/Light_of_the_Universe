using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Arena : MonoBehaviour
{
    [SerializeField] private float enemyMoveSpeed = 25;
    [SerializeField] private Transform Spawn;
    [SerializeField] private Transform SpawnEnd;
    [SerializeField] private Door EnterDoor;
    [SerializeField] private Door ExitDoor;
    [SerializeField] private PlayerStarshipTrigger playerTrigger;
    [SerializeField] private Wave[] Waves;
    [SerializeField] private GameObject[] EnemiesPrefabs;

    public delegate void Method();
    public event Method OnStart;
    public event Method OnEnd;

    private int waveNow;

    private Stack<Starship_AI> Enemies = new Stack<Starship_AI>();

    private Vector3 enemiesStartPosition;
    private Vector3 v3;

    [System.Serializable]
    internal class Wave
    {
        [SerializeField] internal int EnemyType;
        [SerializeField] internal Transform[] Spawns = new Transform[1];
        internal int enemiesCount;
        internal void Initialise()
        {
            enemiesCount = Spawns.Length;
        }
    }

    private void Awake()
    {
        playerTrigger.OnPlayerStarshipEnter += Activate;

        enemiesStartPosition = Spawn.position;
        for (int i = 0; i < Waves.Length; ++i)
        {
            Waves[i].Initialise();
        }
    }

    private void Activate()
    {
        playerTrigger.OnPlayerStarshipEnter -= Activate;
        StartCoroutine(ActivateEvent());
    }

    private IEnumerator ActivateEvent()
    {
        OnStart?.Invoke();
        yield return StartCoroutine(EnterDoor.IMove());
        StartCoroutine(ActivateFight());
    }
    private IEnumerator ActivateFight()
    {
        if (waveNow < Waves.Length)
        {
            Wave W = Waves[waveNow];
            Starship_AI E;
            for (int i = 0; i < W.enemiesCount; ++i)
            {
                Spawn.position = enemiesStartPosition;
                v3 = Vector3.Scale(Vector3.Scale(SpawnEnd.up, SpawnEnd.up), enemiesStartPosition) + W.Spawns[i].position - Vector3.Scale(Vector3.Scale(SpawnEnd.up, SpawnEnd.up), W.Spawns[i].position);
                E = Instantiate(EnemiesPrefabs[W.EnemyType], v3, W.Spawns[i].rotation).GetComponent<Starship_AI>();
                Enemies.Push(E);
                E.transform.parent = Spawn;
                E.SetPlayerHunt(false);
                E.SetControlLock(true);
                E.GetComponent<Health>().OnDeath += EnemyDeath;
            }
            StartCoroutine(IMoveEnemies());
        }
        else
        {
            OnEnd?.Invoke();
            yield return StartCoroutine(ExitDoor.IMove());
            Destroy(this);
        }
    }
    private void EnemiesFight()
    {
        Starship_AI E;
        while (Enemies.Count > 0)
        {
            E = Enemies.Pop();
            E.transform.parent = null;
            E.SetPlayerHunt(true);
            E.SetControlLock(false);
        }
        Spawn.position = enemiesStartPosition;
    }
    private void EnemyDeath()
    {
        Waves[waveNow].enemiesCount -= 1;
        if (Waves[waveNow].enemiesCount == 0)
        {
            StartCoroutine(DisactivateFight());
        }
    }

    private IEnumerator DisactivateFight()
    {
        yield return new WaitForSeconds(3);
        waveNow += 1;
        StartCoroutine(ActivateFight());
    }
    private IEnumerator IMoveEnemies()
    {
        while (Vector3.Distance(Spawn.position, SpawnEnd.position) > 0.005f)
        {
            Spawn.position = Vector3.MoveTowards(Spawn.position, SpawnEnd.position, Time.deltaTime * enemyMoveSpeed);
            yield return null;
        }
        Spawn.position = SpawnEnd.position;
        EnemiesFight();
    }
}
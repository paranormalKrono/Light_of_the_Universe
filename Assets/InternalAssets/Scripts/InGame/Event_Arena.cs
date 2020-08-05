using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Arena : MonoBehaviour
{
    [SerializeField] private Transform EnterDoor;
    [SerializeField] private Transform EnterDoorEnd;
    [SerializeField] private Transform ExitDoor;
    [SerializeField] private Transform ExitDoorEnd;
    [SerializeField] private Transform Spawn;
    [SerializeField] private Transform SpawnEnd;
    [SerializeField] private float doorSpeed = 10;
    [SerializeField] private float enemyMoveSpeed = 25;
    [SerializeField] private Wave[] Waves;
    [SerializeField] private EnemyResource[] EnemyResources;

    private int waveNow;

    private bool isActivated;
    private bool isCloseEnterDoor;
    private bool isOpenExitDoor;
    private bool isEnemiesSpawned;

    private Stack<Starship_AI> Enemies = new Stack<Starship_AI>();

    private Vector3 enemiesStartPosition;
    private Vector3 v3;

    [System.Serializable]
    internal class EnemyResource
    {
        [SerializeField] internal string path;
        internal GameObject EnemyGameobject;
        internal void Initialise()
        {
            EnemyGameobject = Resources.Load<GameObject>(path);
        }
    }

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

    private void Start()
    {
        enemiesStartPosition = Spawn.position;
        foreach (Wave W in Waves)
        {
            W.Initialise();
        }

        foreach (EnemyResource ER in EnemyResources)
        {
            ER.Initialise();
        }
    }

    private void Update()
    {
        if (isActivated)
        {
            if (isCloseEnterDoor)
            {
                EnterDoor.position = Vector3.MoveTowards(EnterDoor.position, EnterDoorEnd.position, Time.deltaTime * doorSpeed);
            }
            if (isOpenExitDoor)
            {
                ExitDoor.position = Vector3.MoveTowards(ExitDoor.position, ExitDoorEnd.position, Time.deltaTime * doorSpeed);
            }
            if (isEnemiesSpawned)
            {
                Spawn.position = Vector3.MoveTowards(Spawn.position, SpawnEnd.position, Time.deltaTime * enemyMoveSpeed);
                if (Spawn.position == SpawnEnd.position)
                {
                    EnemiesFight();
                    isEnemiesSpawned = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated)
        {
            if (other.gameObject.GetComponentInParent<Player_Starship_Controller>() != null)
            {
                isActivated = true;
                StartCoroutine(ActivateEvent());
            }
        }
    }
    private IEnumerator ActivateEvent()
    {
        NextStage();
        isCloseEnterDoor = true;
        yield return new WaitForSeconds(5);
        isCloseEnterDoor = false;
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
                E = Instantiate(EnemyResources[W.EnemyType].EnemyGameobject, v3, W.Spawns[i].rotation).GetComponent<Starship_AI>();
                Enemies.Push(E);
                E.transform.parent = Spawn;
                E.SetPlayerHunt(false);
                E.SetControlLock(true);
                E.GetComponent<Health>().DeathEvent += EnemyDeath;
            }
            isEnemiesSpawned = true;
        }
        else
        {
            isOpenExitDoor = true;
            NextStage();
            yield return new WaitForSeconds(Vector3.Distance(ExitDoor.position, ExitDoorEnd.position) + 0.5f);
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
    private IEnumerator DisactivateFight()
    {
        yield return new WaitForSeconds(3);
        waveNow += 1;
        StartCoroutine(ActivateFight());
    }
    private void EnemyDeath()
    {
        Waves[waveNow].enemiesCount -= 1;
        if (Waves[waveNow].enemiesCount == 0)
        {
            StartCoroutine(DisactivateFight());
        }
    }
    private void NextStage()
    {
        GameDialogs.NextInGameDialogEvent();
        GameGoals.NextGoalEvent();
    }
}
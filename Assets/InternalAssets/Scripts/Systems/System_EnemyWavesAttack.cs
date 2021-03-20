using System.Collections;
using UnityEngine;

public class System_EnemyWavesAttack : MonoBehaviour
{
    [SerializeField] private System_Starships systemStarships;
    [SerializeField] private Transform[] enemySpawns;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private int enemyTeamID = 1;
    [SerializeField] private int[] WavesEnemiesCount;

    [SerializeField] private GameObject[] enemiesPrefabs;

    [SerializeField] private float timeBetweenCreatingEnemies = 2;

    private int enemiesToCreate = 0;
    private int EnemiesCreated = 0;

    public delegate void EventHandler();
    public EventHandler OnWaveEnd;

    private bool isEnded;

    public void Initialize()
    {
        if (systemStarships.StarshipsTeams.Count - 1 < enemyTeamID)
        {
            systemStarships.StarshipsTeams.Add(new System_Starships.StarshipsTeam());
            enemyTeamID = systemStarships.StarshipsTeams.Count - 1;
        }
    }

    public void ResetWave()
    {
        StopAllCoroutines();
    }
    public void End()
    {
        isEnded = true;
        StopAllCoroutines();
    }
    public IEnumerator IStartWave(int waveID, EventHandler OnWaveEnd)
    {
        this.OnWaveEnd = OnWaveEnd;
        enemiesToCreate = WavesEnemiesCount[waveID];
        while (enemiesToCreate > 0)
        {
            enemiesToCreate -= 1;
            EnemiesCreated += 1;
            CreateEnemy(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)], enemySpawns[Random.Range(0, enemySpawns.Length)]);
            yield return new WaitForSeconds(timeBetweenCreatingEnemies);
        }
    }


    private void CreateEnemy(GameObject enemyPrefab, Transform spawnTr)
    {
        GameObject g = Instantiate(enemyPrefab);
        Starship starship = g.GetComponent<Starship>();

        g.transform.position = spawnTr.position;
        starship.RotationPoint.rotation = spawnTr.rotation;

        systemStarships.StarshipsTeams[enemyTeamID].AddStarship(starship);
        starship.SetFollowTarget(playerTransform);
        starship.SetAttack(true);
        starship.SetFollowEnemy(true);
        starship.DeathEvent += OnEnemyDeath;
        starship.SetLockControl(false);
    }

    private void OnEnemyDeath(Transform Tr)
    {
        if (!isEnded)
        {
            EnemiesCreated -= 1;
            if (EnemiesCreated == 0)
            {
                OnWaveEnd();
            }
        }
    }
}

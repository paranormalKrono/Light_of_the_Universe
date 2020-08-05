using System.Collections.Generic;
using UnityEngine;

public class StarshipsSpawnMover : MonoBehaviour
{

    [SerializeField] private StarshipTeam[] starshipTeams;

    private static SpawnSection[] SpawnerSections;


    public static void SetSpawnSections(SpawnSection[] spawnSections)
    {
        SpawnerSections = spawnSections;
    }

    public List<List<Starship>> MoveStarshipsOnSpawns()
    {
        List<Transform> spawns;
        StarshipTeam.StarshipSection starshipSection;
        Transform Point;
        List<List<Starship>> starships = new List<List<Starship>>();

        for (int i = 0; i < starshipTeams.Length; ++i)
        {
            starships.Add(new List<Starship>());
            for (int j = 0; j < starshipTeams[i].StarshipSections.Length; ++j)
            {
                starshipSection = starshipTeams[i].StarshipSections[j];
                spawns = SpawnerSections[starshipSection.SpawnsSection].GetSpawns();
                if (starshipSection.isChaoticSpawn)
                {
                    for (int t = 0; t < starshipSection.Starships.Length; ++t)
                    {
                        starships[i].Add(starshipSection.Starships[t]);
                        Point = spawns[Random.Range(0, spawns.Count)];
                        spawns.Remove(Point);
                        starshipSection.Starships[t].transform.SetPositionAndRotation(Point.position, Point.rotation);
                    }
                }
                else
                {
                    if (starshipSection.isRandomlySpawn)
                    {
                        for (int t = 0; t < starshipSection.Starships.Length; ++t)
                        {
                            starships[i].Add(starshipSection.Starships[t]);
                            Point = spawns[Random.Range(0, starshipSection.Starships.Length - t)];
                            spawns.Remove(Point);
                            starshipSection.Starships[t].transform.SetPositionAndRotation(Point.position, Point.rotation);
                        }
                    }
                    else
                    {
                        for (int t = 0; t < starshipSection.Starships.Length; ++t)
                        {
                            starships[i].Add(starshipSection.Starships[t]);
                            Point = spawns[t];
                            starshipSection.Starships[t].transform.SetPositionAndRotation(Point.position, Point.rotation);
                        }
                    }
                }
            }
        }
        return starships;
    }
    [System.Serializable]
    public class StarshipTeam
    {
        [SerializeField] public StarshipSection[] StarshipSections;

        [System.Serializable]
        public class StarshipSection
        {
            [SerializeField] public int SpawnsSection;
            [SerializeField] public bool isChaoticSpawn;
            [SerializeField] public bool isRandomlySpawn;
            [SerializeField] public Starship[] Starships;
        }
    }
}
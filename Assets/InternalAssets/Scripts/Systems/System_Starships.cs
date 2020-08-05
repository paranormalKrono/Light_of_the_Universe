using System.Collections.Generic;
using UnityEngine;

public class System_Starships : MonoBehaviour
{
    internal class StarshipsTeam
    {
        internal List<Starship> Starships { get; private set; }
        internal List<Transform> Transforms { get; private set; }
        internal bool isActive = true;

        internal delegate void EventTeamDelegate(StarshipsTeam starhipsTeam);
        internal event EventTeamDelegate OnTeamDevastated;

        internal void AddStarship(Starship starship)
        {
            Transforms.Add(starship.transform);
            starship.DeathEvent += OnStarshipDeath;
            Starships.Add(starship);
        }
        internal void AddStarships(List<Starship> starships)
        {
            for (int i = 0; i < starships.Count; ++i)
            {
                Transforms.Add(starships[i].transform);
                starships[i].DeathEvent += OnStarshipDeath;
            }
            Starships.AddRange(starships);
        }
        internal void RemoveStarship(Starship starship)
        {
            int index = Starships.IndexOf(starship);
            if (index > -1)
            {
                Transforms.RemoveAt(index);
                Starships.RemoveAt(index);
            }
        }

        internal void SetActive(bool t)
        {
            isActive = t;
            foreach (Transform Tr in Transforms)
            {
                Tr.gameObject.SetActive(t);
            }
        }
        internal void SetStarshipsLock(bool t)
        {
            for (int i = 0; i < Starships.Count; ++i)
            {
                Starships[i].SetLockControl?.Invoke(t);
            }
        }
        internal void SetStarshipsFollowTarget(bool t)
        {
            for (int i = 0; i < Starships.Count; ++i)
            {
                Starships[i].SetFollowTarget?.Invoke(t);
            }
        }

        internal StarshipsTeam(Starship starship)
        {
            Initialize();
            AddStarship(starship);
        }
        internal StarshipsTeam(List<Starship> starships)
        {
            Initialize();
            AddStarships(starships);
        }

        private void Initialize()
        {
            Starships = new List<Starship>();
            Transforms = new List<Transform>();
        }
        private void OnStarshipDeath(Transform Tr)
        {
            if (Transforms.Contains(Tr))
            {
                int g = Transforms.IndexOf(Tr);
                Starships.RemoveAt(g);
                Transforms.RemoveAt(g);

                if (Starships.Count == 0)
                {
                    OnTeamDevastated?.Invoke(this);
                }
            }
        }

    }
    internal List<StarshipsTeam> StarshipsTeams = new List<StarshipsTeam>();

    private bool isEveryoneDead;

    private int id1, id2;
    private float d;
    private float d2;
    Transform tr;

    internal delegate void EventDelegate();
    internal event EventDelegate OnOneTeamLeft;

    private void FixedUpdate() => TargetUpdate();


    private void TargetUpdate()
    {
        if (!isEveryoneDead)
        {
            for (int i1 = 0; i1 < StarshipsTeams.Count; ++i1) // Проходим по коллекциям AI
            {
                if (StarshipsTeams[i1].isActive)
                {
                    for (int j1 = 0; j1 < StarshipsTeams[i1].Transforms.Count; ++j1) // Проходим по кораблям 1-ой коллекции
                    {
                        tr = StarshipsTeams[i1].Transforms[j1]; // Корабль из коллекции i1, номер j1
                        id1 = -1; // Выставляем номер второй коллекции
                        id2 = -1; // Выставляем номер корабля во 2-ой коллекции
                        d = int.MaxValue; // Максимальная дистанция поиска
                        for (int i2 = 0; i2 < StarshipsTeams.Count; ++i2) // Проходим по остальным коллекциям, кроме i1
                        {
                            if (i1 != i2)
                            {
                                for (int j2 = 0; j2 < StarshipsTeams[i2].Transforms.Count; ++j2) // Проходим по кораблям из второй коллекции
                                {
                                    d2 = Vector3.Distance(tr.position, StarshipsTeams[i2].Transforms[j2].position);// Новая дистанция между кораблём из первой коллекции и j2 кораблём из второй
                                    if (d2 < d) // Если новая дистанция меньше натоящей
                                    {
                                        d = d2; // Записываем новое расстояние
                                        id1 = i2; // Запоминаем номер второй коллекции
                                        id2 = j2; // запоминаем номер корабля из второй коллекции
                                    }
                                }
                            }
                        }
                        if (id1 > -1 && id2 > -1)
                        {
                            StarshipsTeams[i1].Starships[j1].SetEnemyTarget?.Invoke(StarshipsTeams[id1].Transforms[id2]); // Отправляем корабль из второй коллекции, как новую цель
                        }
                        else
                        {
                            isEveryoneDead = true; // Все умерли
                        }
                    }
                }
            }
        }
    }

    private void OnTeamDevastated(StarshipsTeam starshipsTeam)
    {
        StarshipsTeams.Remove(starshipsTeam);
        if (StarshipsTeams.Count < 2)
        {
            if (StarshipsTeams.Count == 1)
            {
                OnOneTeamLeft?.Invoke();
            }
            SetStarshipsFollowTarget(false);
            isEveryoneDead = true;
        }
    }


    internal void SetStarshipsLock(bool t)
    {
        for (int i = 0; i < StarshipsTeams.Count; ++i)
        {
            SetStarshipsLock(i, t);
        }
    }
    internal void SetStarshipsLock(int team, bool t) => StarshipsTeams[team].SetStarshipsLock(t);

    internal void SetStarshipsActive(int Team, bool t) => StarshipsTeams[Team].SetActive(t);

    internal void SetStarshipsFollowTarget(bool t)
    {
        for (int i = 0; i < StarshipsTeams.Count; ++i)
        {
            SetStarshipsFollowTarget(i, t);
        }
    }
    internal void SetStarshipsFollowTarget(int team, bool t) => StarshipsTeams[team].SetStarshipsFollowTarget(t);


    internal void InitializeStarshipsTeams(List<List<Starship>> starships)
    {
        StarshipsTeam starshipsTeam;
        for (int i = 0; i < starships.Count; ++i)
        {
            starshipsTeam = new StarshipsTeam(starships[i]);
            starshipsTeam.OnTeamDevastated += OnTeamDevastated;
            StarshipsTeams.Add(starshipsTeam);
        }
    }
    internal void StarshipChangeTeam(Starship starship, int From, int To)
    {
        StarshipsTeams[From].RemoveStarship(starship);
        StarshipsTeams[To].AddStarship(starship);
    }
    internal int StarshipChangeTeamToNew(Starship starship, int From)
    {
        StarshipsTeams[From].RemoveStarship(starship);
        StarshipsTeams.Add(new StarshipsTeam(starship));
        return StarshipsTeams.Count - 1;
    }


    internal float GetMinDistanceTeamToPoint(int Team, Vector3 vector3)
    {
        float f;
        float min = float.MaxValue;
        foreach (Transform Tr in StarshipsTeams[Team].Transforms)
        {
            f = Vector3.Distance(Tr.position, vector3);
            if (f < min)
            {
                min = f;
            }
        }
        return min;
    }
}
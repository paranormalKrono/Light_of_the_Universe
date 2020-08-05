using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnSection : MonoBehaviour
{
    public List<Transform> GetSpawns()
    {
        List<Transform> spawns = GetComponentsInChildren<Transform>().ToList();
        spawns.RemoveAt(0);
        return spawns;
    }
}

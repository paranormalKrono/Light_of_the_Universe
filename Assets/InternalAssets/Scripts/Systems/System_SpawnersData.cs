using UnityEngine;

public class System_SpawnersData : MonoBehaviour
{
    // XXXXX НЕ ТРОГАТЬ!!!! XXXXX
    [SerializeField] public SpawnSection[] SpawnSections;
    // XXXXXXXXXXXXXXXXXXXXXXXXXXX

    public void Awake() => StarshipsSpawnMover.SetSpawnSections(SpawnSections);

}

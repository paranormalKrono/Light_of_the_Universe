using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class System_WaypointsData: MonoBehaviour
{
    // НЕ ТРОГАТЬ!!!!!
    //xxxxxxxxxxxxxxxxxxxxxx
    [SerializeField] private Transform PointsFolder;
    [System.Serializable]
    public class MovePoint
    {
        [SerializeField] public List<int> Move = new List<int>();
    }
    [SerializeField] public List<MovePoint> MovePoints = new List<MovePoint>();
    public List<Transform> PointsTr;
    //xxxxxxxxxxxxxxxxxxxxxxxx

    private void Awake() => System_Waypoints.SetMovePoints(MovePoints, PointsTr);

    public void Initialise()
    {
        PointsTr = PointsFolder.GetComponentsInChildren<Transform>().ToList();
        PointsTr.RemoveAt(0);
    }
}
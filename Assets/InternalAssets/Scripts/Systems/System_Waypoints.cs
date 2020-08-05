using System.Collections.Generic;
using UnityEngine;

public static class System_Waypoints
{

    public static System_WaypointsData.MovePoint[] MovePoints;
    public static Transform[] Points;

    private static int curPointID, prePointID, prePointMoveID;

    public static void SetMovePoints(List<System_WaypointsData.MovePoint> movePoints, List<Transform> points)
    {
        MovePoints = movePoints.ToArray();
        Points = points.ToArray();
    }

    private static Transform curPoint;
    private static float d;
    private static float minDistance;

    internal static Transform GetClosePoint(Transform V3)
    {
        curPoint = V3;
        minDistance = float.MaxValue;
        for (int i = 0; i < Points.Length; ++i)
        {
            d = Vector3.Distance(V3.position, Points[i].position);
            if (d < minDistance)
            {
                curPoint = Points[i];
                minDistance = d;
            }
        }
        return curPoint;
    }

    internal static Transform GetNextPoint(Transform currentPoint, Transform previousPoint)
    {
        for (curPointID = 0; curPointID < Points.Length; ++curPointID) // Ищем ID текущей точки
        {
            if (Points[curPointID] == currentPoint)
            {
                break;
            }
        }
        for (prePointID = 0; prePointID < Points.Length; ++prePointID) // Ищем ID прошлой точки
        {
            if (Points[prePointID] == previousPoint)
            {
                break;
            }
        }
        for (prePointMoveID = 0; prePointMoveID < MovePoints[curPointID].Move.Count; ++prePointMoveID) // Находим в текущей точке номер путя к прошлой точке
        {
            if (MovePoints[curPointID].Move[prePointMoveID] == prePointID)
            {
                break;
            }
        }
        if (prePointMoveID < MovePoints[curPointID].Move.Count) // Если прошлая точка найдена в списке путей, то область массива допустимых путей уменьшается на 1
        {
            prePointID = Random.Range(0, MovePoints[curPointID].Move.Count - 1);
        }
        else
        {
            prePointID = Random.Range(0, MovePoints[curPointID].Move.Count);
        }
        if (prePointID >= prePointMoveID && MovePoints[curPointID].Move.Count > 1) // Если путь ведёт к предыдущей точке и путь не один, пропускаем путь к прошлой точке
        {
            prePointID += 1;
        }
        return Points[MovePoints[curPointID].Move[prePointID]]; // Отправляем новую точку по новому пути текущей точки
    }
}
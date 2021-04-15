using System.Collections.Generic;
using System.Linq;
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

    private static float pointToTargetDistance;
    private static Transform curPoint;
    private static Transform curClosePoint;
    private static float distance;
    private static float closeDistance;

    internal static Transform GetBetterPointInDistance(Transform V3, float minDis)
    {
        curPoint = V3;
        closeDistance = 0;
        distance = float.MaxValue;
        for (int i = 0; i < Points.Length; ++i)
        {
            pointToTargetDistance = Vector3.Distance(V3.position, Points[i].position);
            if (pointToTargetDistance < distance && pointToTargetDistance > minDis)
            {
                curPoint = Points[i];
                distance = pointToTargetDistance;
            }
            if (pointToTargetDistance > closeDistance)
            {
                curClosePoint = Points[i];
                closeDistance = pointToTargetDistance;
            }
        }
        if (curPoint == null)
        {
            return curClosePoint;
        }
        else
        {
            return curPoint;
        }
    }

    private static SortedDictionary<float, Transform> pointsDictionary = new SortedDictionary<float, Transform>();
    internal static KeyValuePair<float, Transform>[] GetPointsClose(Transform V3) => GetPointsClose(V3.position);
    internal static KeyValuePair<float, Transform>[] GetPointsClose(Vector3 Target)
    {
        pointsDictionary.Clear();
        for (int i = 0; i < Points.Length; ++i)
        {
            distance = Vector3.Distance(Target, Points[i].position);
            if (!pointsDictionary.ContainsKey(distance))
            {
                pointsDictionary.Add(distance, Points[i]);
            }
        }
        return pointsDictionary.ToArray();
    }

    internal static Transform GetClosestPoint(Transform V3)
    {
        curPoint = V3;
        distance = float.MaxValue;
        for (int i = 0; i < Points.Length; ++i)
        {
            pointToTargetDistance = Vector3.Distance(V3.position, Points[i].position);
            if (pointToTargetDistance < distance)
            {
                curPoint = Points[i];
                distance = pointToTargetDistance;
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

        if (MovePoints[curPointID].Move.Count - 1 < prePointID)
        {
            return null;
        }
        else
        {
            return Points[MovePoints[curPointID].Move[prePointID]]; // Отправляем новую точку по новому пути текущей точки
        }
    }
}
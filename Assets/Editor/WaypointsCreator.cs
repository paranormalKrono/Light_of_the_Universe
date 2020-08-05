using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
public class WaypointsCreator : EditorWindow
{
    [MenuItem("Window/Way Points")]
    public static void ShowWindow() {
        GetWindow(typeof(WaypointsCreator));
    }
    private System_WaypointsData systemWaypointsData;
    private List<Transform> waypoints;
    private List<Vector3> Ways1 = new List<Vector3>();
    private List<Vector3> Ways2 = new List<Vector3>();
    private bool isFindSystem;

    private Transform Point1;
    private Transform Point2;

    private void Update()
    {
        if (isFindSystem)
        {
            DrawWays();
        }
    }
    private void OnGUI()
    {
        GUI.skin.label.fontSize = 20;
        GUI.skin.label.normal.textColor = Color.green;
        GUILayout.Label("WayPoints Creator");
        GUI.skin.label.fontSize = 14;
        GUI.skin.label.normal.textColor = Color.black;
        GUILayout.Label("isSelected = " + isFindSystem);
        if (isFindSystem)
        {
            GUILayout.Label("GameobjectName - " + systemWaypointsData.name);
        }
        GUILayout.Space(5);
        if (GUILayout.Button("Choose"))
        {
            Choose();
        }
        if (isFindSystem)
        {
            if (GUILayout.Button("CreateOneWay"))
            {
                if (Selection.gameObjects.Length >= 2)
                {
                    for (int i = 0; i < Selection.gameObjects.Length - 1; ++i)
                    {
                        Point1 = (Selection.objects[i] as GameObject).transform;
                        Point2 = (Selection.objects[i + 1] as GameObject).transform;
                        Debug.Log("Point1 - " + Selection.objects[i].name + " Point2 - " + Selection.objects[i + 1].name);
                        if (waypoints.Contains(Point1) && waypoints.Contains(Point2))
                        {
                            AddOneWay(Point1, Point2);
                        }
                    }
                    Selection.SetActiveObjectWithContext(Selection.activeGameObject, Selection.activeGameObject);
                }
            }
            if (GUILayout.Button("CreateWay"))
            {
                if (Selection.gameObjects.Length >= 2)
                {
                    for (int i = 0; i < Selection.gameObjects.Length - 1; ++i)
                    {
                        Point1 = (Selection.objects[i] as GameObject).transform;
                        Point2 = (Selection.objects[i+1] as GameObject).transform;
                        Debug.Log("Point1 - " + Selection.objects[i].name + " Point2 - " + Selection.objects[i+1].name);
                        if (waypoints.Contains(Point1) && waypoints.Contains(Point2))
                        {
                            AddWay(Point1, Point2);
                        }
                    }
                    Selection.SetActiveObjectWithContext(Selection.activeGameObject, Selection.activeGameObject);
                }
            }
            if (GUILayout.Button("DestroyWay"))
            {
                if (Selection.gameObjects.Length >= 2)
                {
                    for (int i = 0; i < Selection.gameObjects.Length - 1; ++i)
                    {
                        Point1 = (Selection.objects[i] as GameObject).transform;
                        Point2 = (Selection.objects[i + 1] as GameObject).transform;
                        if (waypoints.Contains(Point1) && waypoints.Contains(Point2))
                        {
                            DestroyWay(Point1, Point2);
                        }
                    }
                    Selection.SetActiveObjectWithContext(Selection.activeGameObject, Selection.activeGameObject);
                }
            }
        }
    }
    private void Choose()
    {
        Ways1.Clear();
        Ways2.Clear();
        if (!Selection.activeGameObject.TryGetComponent(out systemWaypointsData))
        {
            isFindSystem = false;
            return;
        }
        isFindSystem = true;
        systemWaypointsData.Initialise();
        waypoints = systemWaypointsData.PointsTr.ToList();
        if (systemWaypointsData.MovePoints.Count != waypoints.Count)
        {
            while (waypoints.Count > systemWaypointsData.MovePoints.Count)
            {
                systemWaypointsData.MovePoints.Add(new System_WaypointsData.MovePoint());
            }
            while (waypoints.Count < systemWaypointsData.MovePoints.Count)
            {
                systemWaypointsData.MovePoints.RemoveAt(systemWaypointsData.MovePoints.Count-1);
            }
        }
        bool b;
        for (int i = 0; i < systemWaypointsData.MovePoints.Count; ++i)
        {
            foreach (int M in systemWaypointsData.MovePoints[i].Move)
            {
                if (i == M)
                {
                    systemWaypointsData.MovePoints[i].Move.Remove(M);
                }
                else
                {
                    b = false;
                    for (int w = 0; w < Ways1.Count; ++w)
                    {
                        if (Ways1[w] == waypoints[i].position && Ways2[w] == waypoints[M].position)
                        {
                            b = true;
                            break;
                        }
                        else if (Ways2[w] == waypoints[i].position && Ways1[w] == waypoints[M].position)
                        {
                            b = true;
                            break;
                        }
                    }
                    if (!b)
                    {
                        //Debug.Log("Ways initialized - " + i + " " + M);
                        Ways1.Add(waypoints[i].position);
                        Ways2.Add(waypoints[M].position);
                    }
                }
            }
        }
        EditorUtility.SetDirty(systemWaypointsData);
    }
    private void AddOneWay(Transform T1, Transform T2)
    {
        for (int i = 0; i < Ways1.Count; ++i)
        {
            if (Ways1[i] == T1.position && Ways2[i] == T2.position)
            {
                return;
            }
        }
        Ways1.Add(T1.position);
        Ways2.Add(T2.position);
        systemWaypointsData.MovePoints[waypoints.IndexOf(T1)].Move.Add(waypoints.IndexOf(T2));

        EditorUtility.SetDirty(systemWaypointsData);
        Debug.Log("OneWay added");
    }
    private void AddWay(Transform T1, Transform T2)
    {
        for (int i = 0; i < Ways1.Count; ++i)
        {
            if (Ways1[i] == T1.position && Ways2[i] == T2.position)
            {
                return;
            }
            else if (Ways1[i] == T2.position && Ways2[i] == T1.position)
            {
                return;
            }
        }
        Ways1.Add(T1.position);
        Ways2.Add(T2.position);
        systemWaypointsData.MovePoints[waypoints.IndexOf(T1)].Move.Add(waypoints.IndexOf(T2));
        systemWaypointsData.MovePoints[waypoints.IndexOf(T2)].Move.Add(waypoints.IndexOf(T1));

        EditorUtility.SetDirty(systemWaypointsData);
        Debug.Log("Way added");
    }
    private void DestroyWay(Transform T1, Transform T2)
    {
        for (int i = 0; i < Ways1.Count; ++i)
        {
            if (Ways1[i] == T1.position && Ways2[i] == T2.position || Ways1[i] == T2.position && Ways2[i] == T1.position)
            {
                Ways1.RemoveAt(i);
                Ways2.RemoveAt(i);
                systemWaypointsData.MovePoints[waypoints.IndexOf(T1)].Move.Remove(waypoints.IndexOf(T2));
                systemWaypointsData.MovePoints[waypoints.IndexOf(T2)].Move.Remove(waypoints.IndexOf(T1));
                EditorUtility.SetDirty(systemWaypointsData);
                Debug.Log("Way destroyed");
            }
        }
    }
    private void DrawWays()
    {
        for (int i = 0; i < Ways1.Count; ++i)
        {
            Debug.DrawLine(Ways1[i],Ways2[i],Color.green);
        }
    }
}
#endif
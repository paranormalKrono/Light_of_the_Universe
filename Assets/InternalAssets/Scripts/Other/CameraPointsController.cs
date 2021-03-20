using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPointsController : MonoBehaviour
{
    [SerializeField] private Transform[] Points;

    public void MoveCameraToPoint(int point)
    {
        transform.position = Points[point].position;
        transform.rotation = Points[point].rotation;
    }
}
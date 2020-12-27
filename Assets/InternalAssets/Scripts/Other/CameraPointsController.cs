using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPointsController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 20;

    public delegate void EventHandler();
    public event EventHandler OnCameraMoved;


    public void MoveCamera(Transform Tr)
    {
        StopAllCoroutines();
        StartCoroutine(IMoveCamera(Tr.position, Tr.rotation));
    }


    public void MoveCameraWithDark(Transform Tr)
    {
        StopAllCoroutines();
        StartCoroutine(IMoveCameraWithDark(Tr));
    }


    private IEnumerator IMoveCameraWithDark(Transform Tr)
    {
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        transform.position = Tr.position;
        transform.rotation = Tr.rotation;
        OnCameraMoved?.Invoke();
        yield return StartCoroutine(GameScreenDark.ITransparentEvent());
    }

    private IEnumerator IMoveCamera(Vector3 TargetPos, Quaternion TargetRot)
    {
        while (Vector3.Distance (transform.position, TargetPos) > 0 || Quaternion.Angle(transform.rotation, TargetRot) > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPos, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRot, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        OnCameraMoved?.Invoke();
    }
}
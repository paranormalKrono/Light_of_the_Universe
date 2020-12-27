using System.Collections;
using UnityEngine;

public class PlayerStarshipMover : MonoBehaviour
{
    [SerializeField] private float starshipMoveSpeed = 10;
    [SerializeField] private float starshipRotateSpeed = 80;
    [SerializeField] private Transform startTr;
    [SerializeField] private Transform endTr;

    internal delegate void Method();
    internal event Method OnStartMove;
    internal event Method OnEndMove;


    public void Move(Player_Starship_Controller player_Starship_Controller)
    {
        Transform pos, rot;
        player_Starship_Controller.GetPositionAndRotationTransforms(out pos, out rot);
        pos.position = startTr.position;
        rot.rotation = endTr.rotation;
        StartCoroutine(IStarshipMove(pos, rot));
    }

    private IEnumerator IStarshipMove(Transform pos, Transform rot)
    {
        OnStartMove?.Invoke();
        while (Vector3.Distance(pos.position, endTr.position) > 0.01f)
        {
            pos.position = Vector3.MoveTowards(pos.position, endTr.position, Time.deltaTime * starshipMoveSpeed);
            rot.rotation = Quaternion.RotateTowards(rot.rotation, endTr.rotation, Time.deltaTime * starshipRotateSpeed);
            yield return null;
        }
        pos.position = endTr.position;
        OnEndMove?.Invoke();
    }

    public void MoveLine(Player_Starship_Controller player_Starship_Controller)
    {
        Transform pos, rot;
        player_Starship_Controller.GetPositionAndRotationTransforms(out pos, out rot);
        StartCoroutine(IStarshipMoveLine(pos, rot));
    }

    private IEnumerator IStarshipMoveLine(Transform pos, Transform rot)
    {
        OnStartMove?.Invoke();
        while (Vector3.Distance(pos.position, endTr.position) > 0.1f)
        {
            pos.position += endTr.forward * starshipMoveSpeed * Time.deltaTime;
            rot.rotation = Quaternion.RotateTowards(rot.rotation, endTr.rotation, starshipRotateSpeed * Time.deltaTime);
            yield return null;
        }
        OnEndMove?.Invoke();
    }

    public void StarshipToEndPosition(Player_Starship_Controller player_Starship_Controller)
    {
        Transform pos, rot;
        player_Starship_Controller.GetPositionAndRotationTransforms(out pos, out rot);
        pos.position = endTr.position;
        rot.rotation = endTr.rotation;
    }

    public void StopMove() => StopAllCoroutines();

    public Vector3 GetEndPosition() => endTr.position;
}
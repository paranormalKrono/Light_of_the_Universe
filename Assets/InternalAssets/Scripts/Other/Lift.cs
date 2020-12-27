using System.Collections;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] private float LiftMoveSpeed = 6;
    [SerializeField] private float starshipMoveSpeed = 10;
    [SerializeField] private float starshipRotateSpeed = 80;
    [SerializeField] private Transform liftTr;
    [SerializeField] private Transform liftEndTr;
    [SerializeField] private Transform starshipTargetTr;

    internal delegate void Method();

    internal void StartLift(Player_Starship_Controller player_Starship_Controller, Method OnLiftArrived) => StartCoroutine(IStartLift(player_Starship_Controller, OnLiftArrived));

    private IEnumerator IStartLift(Player_Starship_Controller player_Starship_Controller, Method OnLiftArrived)
    {
        Transform pos, rot;
        player_Starship_Controller.GetPositionAndRotationTransforms(out pos, out rot);
        while (Quaternion.Angle(starshipTargetTr.rotation, rot.rotation) > 0.2f || Vector3.Distance(starshipTargetTr.position, pos.position) > 0.01f)
        {
            pos.position = Vector3.MoveTowards(pos.position, starshipTargetTr.position, Time.deltaTime * starshipMoveSpeed);
            rot.rotation = Quaternion.RotateTowards(rot.rotation, starshipTargetTr.rotation, Time.deltaTime * starshipRotateSpeed);
            yield return null;
        }

        while (Vector3.Distance(liftTr.position, liftEndTr.position) > 0.01f)
        {
            liftTr.position = Vector3.MoveTowards(liftTr.position, liftEndTr.position, Time.deltaTime * LiftMoveSpeed);
            pos.position = starshipTargetTr.position;
            yield return null;
        }
        liftTr.position = liftEndTr.position;
        OnLiftArrived?.Invoke();
    }
}

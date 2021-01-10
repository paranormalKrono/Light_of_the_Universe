using System.Collections;
using UnityEngine;

public class StarshipMover : MonoBehaviour
{
    [SerializeField] private Transform startTr;
    [SerializeField] private Transform endTr;

    internal delegate void Method();
    internal event Method OnStartMove;
    internal event Method OnEndMove;


    public void Move(Starship starship) => StartCoroutine(IStarshipMove(starship));

    public IEnumerator IStarshipMove(Starship starship)
    {
        Starship_Engine starship_Engine = starship.GetComponent<Starship_Engine>();
        Starship_RotationEngine starship_RotationEngine = starship.GetComponent<Starship_RotationEngine>();
        Rigidbody rigidbody = starship.GetComponent<Rigidbody>();

        ToStartPosition(starship);

        OnStartMove?.Invoke();
        starship_Engine.SetLockMove(false);
        while (Vector3.Distance(starship.transform.position, endTr.position) > 1f)
        {
            starship_RotationEngine.RotateToTargetWithPlaneLimiter(endTr.position + endTr.forward);

            starship_Engine.Move(MoveDirectionConsideringAngle(1, Vector3.Angle((endTr.position - rigidbody.velocity - starship.transform.position).normalized, starship.RotationPoint.forward))); // Двигаемся
            
            yield return new WaitForFixedUpdate();
        }
        starship_Engine.SetLockMove(true);
        OnEndMove?.Invoke();
    }

    public void MoveLine(Starship starship) => StartCoroutine(IStarshipMoveLine(starship));

    public IEnumerator IStarshipMoveLine(Starship starship)
    {

        Starship_Engine starship_Engine = starship.GetComponent<Starship_Engine>();
        Starship_RotationEngine starship_RotationEngine = starship.GetComponent<Starship_RotationEngine>();
        Rigidbody rigidbody = starship.GetComponent<Rigidbody>();

        OnStartMove?.Invoke();
        starship_Engine.SetLockMove(false);
        while (Vector3.Distance(starship.transform.position, endTr.position) > 1f)
        {
            starship_RotationEngine.RotateToTargetWithPlaneLimiter(starship.transform.position + endTr.forward);

            starship_Engine.Move(MoveDirectionConsideringAngle(1, Vector3.Angle((endTr.position - rigidbody.velocity - starship.transform.position).normalized, starship.RotationPoint.forward))); // Двигаемся

            yield return new WaitForFixedUpdate();
        }
        starship_Engine.SetLockMove(true);
        OnEndMove?.Invoke();
    }

    public void ToEndPosition(Starship starship)
    {
        starship.transform.position = endTr.position;
        starship.RotationPoint.rotation = endTr.rotation;
    }
    public void ToStartPosition(Starship starship)
    {
        starship.transform.position = startTr.position;
        starship.RotationPoint.rotation = endTr.rotation;
    }

    private float MoveDirectionConsideringAngle(float moveDirection, float angle)
    {
        if (angle > 10) // Если угол больше
        {
            if (angle > 90) // Если угол больше 90, то меняем направление скорости на противоположное
            {
                if (180 - angle > 10)
                {
                    moveDirection /= (180 - angle) / 10; // Уменьшаем вектор направления для снижения скорости
                }
                moveDirection *= -1;
            }
            else
            {
                moveDirection /= angle / 10; // Уменьшаем вектор направления для снижения скорости
            }
        }
        return moveDirection;
    }


    public void StopMove() => StopAllCoroutines();

    public Vector3 GetEndPosition() => endTr.position;
}
using UnityEngine;

public class Player_Camera_Controller : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] internal Vector3 offset;
    private Transform starshipTr;
    private Transform targetTr;
    private bool isLockMove = false;
    private bool isTargetMove = false;

    private void Awake()
    {
        starshipTr = GameObject.FindGameObjectWithTag("Player").transform;
        starshipTr.GetComponent<Health>().DeathEvent += PlayerDead;
    }

    void Update()
    {
        if (!isLockMove)
        {
            transform.position = Vector3.Lerp(transform.position, starshipTr.position + offset, Time.deltaTime * moveSpeed);
        }
        else if (isTargetMove)
        {
            transform.position = Vector3.Lerp(transform.position, targetTr.position, Time.deltaTime * moveSpeed);
        }
    }
    private void PlayerDead()
    {
        SetLockMove(true);
    }
    internal void SetLockMove(bool t)
    {
        isLockMove = t;
    }
    internal void UpdatePlayerLookPosition() => transform.position = starshipTr.position + offset;
    internal void DisableTargetMove()
    {
        SetLockMove(false);
        isTargetMove = false;
    }
    internal void EnableTargetMove(Transform Tr)
    {
        isTargetMove = true;
        targetTr = Tr;
        SetLockMove(true);
    }
}
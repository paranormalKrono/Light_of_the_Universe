using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float doorSpeed = 10;
    [SerializeField] private Transform DoorTr;
    [SerializeField] private Transform DoorEndTr;
    private bool isOpen;

    public void Move() 
    { 
        if (!isOpen)
        {
            StartCoroutine(IMove());
        } 
    }

    internal IEnumerator IMove()
    {
        isOpen = true;
        while (Vector3.Distance(DoorTr.position, DoorEndTr.position) > 0.005f)
        {
            DoorTr.position = Vector3.MoveTowards(DoorTr.position, DoorEndTr.position, Time.deltaTime * doorSpeed);
            yield return null;
        }
        DoorTr.position = DoorEndTr.position;
    }

    public void InstantlyMove()
    {
        if (!isOpen)
        {
            isOpen = true;
            DoorTr.position = DoorEndTr.position;
        }
    }

    //public void StopOpen() => StopAllCoroutines();
}

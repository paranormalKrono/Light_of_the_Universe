using System.Collections;
using UnityEngine;

public class TimeDestroy : MonoBehaviour
{
    [SerializeField] private float timeToDestroy = 10;

    private IEnumerator Start()
    {
        while (timeToDestroy > 0)
        {
            timeToDestroy -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}

using UnityEngine;

public class Activator : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects;

    public void SetActiveGameObjects(bool t)
    {
        for (int i = 0; i < gameObjects.Length; ++i)
        {
            gameObjects[i].SetActive(t);
        }
    }
}

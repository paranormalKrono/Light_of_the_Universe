using System.Collections;
using UnityEngine;

public class Main_Origin : MonoBehaviour
{
    [SerializeField] private float timeToNextScene;

    private IEnumerator Start()
    {
        GameManager.Initialize();

        yield return new WaitForSeconds(timeToNextScene);

        SceneController.LoadNextStoryScene();

    }
}

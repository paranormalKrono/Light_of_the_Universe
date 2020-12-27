using System.Collections;
using UnityEngine;

public class Main_Origin : MonoBehaviour
{
    [SerializeField] private float timeToNextScene;

    private IEnumerator Start()
    {
        GameManager.Initialize();

        yield return StartCoroutine(GameScreenDark.ITransparentEvent());
        yield return new WaitForSeconds(timeToNextScene);
        yield return StartCoroutine(GameScreenDark.IDarkEvent());

        SceneController.LoadNextStoryScene();

    }
}

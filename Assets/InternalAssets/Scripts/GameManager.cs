using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static bool isCreated;
    private static string GameManagerPath = "GameManager";

    public static void Initialize()
    {
        if (!isCreated)
        {
            isCreated = true;
            Saves.Initialise();
            SceneController.Initialise();
            GameObject G = Instantiate(Resources.Load<GameObject>(GameManagerPath));
            DontDestroyOnLoad(G);
        }
    }
}
using System.Collections;
using UnityEngine;

public class Main_Last_2 : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private AudioClip audioClip;

    [SerializeField] private Starship_AI_Adv Z2StarshipAI;
    [SerializeField] private Health Z2Health;

    [SerializeField] private GameObject Z2Image;
    [SerializeField] private Transform Target;
    [SerializeField] private float timeToEnd;

    private Player_Starship_Controller playerController;
    private Player_Camera_Controller playerCamera;

    private Transform PlayerStarshipTr;

    private bool isRestart;
    private bool isEnd;


    private void Awake()
    {
        GameManager.Initialize();
        GameScreenDark.SetDarkEvent(true);

        GameText.DeactivateEvent();
        GameText.SetInGameTextNowEvent(textAsset);

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Starship_Controller>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();

        PlayerStarshipTr = playerController.transform;
    }

    private void Start()
    {
        GameAudio.StartAudioEvent(audioClip, 0.4f, true);

        playerController.SetLockControl(true);
        playerCamera.SetLockMove(false);
        Z2StarshipAI.SetLockControl(true);

        playerCamera.SetPositionWithOffset(PlayerStarshipTr.position);

        playerCamera.EnableTargetMove(Target);

        playerController.SetActiveCanvas(false);

        Z2Image.SetActive(true);

        GameDialogs.StartDialogEvent(OnStartDialogEnd);

        StartCoroutine(GameScreenDark.ITransparentEvent());
    }

    private void FixedUpdate()
    {
        if (!isEnd)
        {
            if (Input.GetKey(KeyCode.N))
            {
                StartCoroutine(IEndEvent());
            }
        }
    }

    private void OnStartDialogEnd() => EndGame();

    private void EndGame()
    {
        if (!isRestart && !isEnd)
        {
            StartCoroutine(IEndEvent());
        }
    }
    private IEnumerator IEndEvent()
    {
        isEnd = true;
        playerController.SetLockControl(false);
        GameAudio.StopAudioEvent();
        Z2Image.SetActive(false);
        Z2Health.Kill();
        yield return new WaitForSeconds(timeToEnd);
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        SceneController.LoadNextStoryScene();
    }
}

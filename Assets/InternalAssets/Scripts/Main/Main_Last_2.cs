using System.Collections;
using UnityEngine;

public class Main_Last_2 : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private AudioClip audioClip;

    [SerializeField] private Starship_AI_Adv Z2StarshipAI;
    [SerializeField] private Health Z2Health;
    [SerializeField] private Animator Z2Animator;

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

        GameDialogs.OnNextDialog += OnNextDialog;
        GameDialogs.StartDialogEvent(OnStartDialogEnd);
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

    private int dialogProgress;
    private void OnNextDialog(int dialogNow)
    {
        if (dialogNow > dialogProgress)
        {
            dialogProgress = dialogNow;
            switch (dialogNow)
            {
                case 2:
                    Z2Animator.SetInteger("Part", 1);
                    break;
                case 11:
                    Z2Animator.SetInteger("Part", 2);
                    break;
                case 12:
                    Z2Animator.SetInteger("Part", 3);
                    break;
                case 14:
                    Z2Animator.SetInteger("Part", 4);
                    break;
                case 18:
                    Z2Animator.SetInteger("Part", 5);
                    break;
                case 25:
                    Z2Animator.SetInteger("Part", 6);
                    break;
                case 27:
                    Z2Image.SetActive(false);
                    break;
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
        Z2Health.Kill();
        yield return new WaitForSeconds(timeToEnd);
        SceneController.LoadNextStoryScene();
    }
}

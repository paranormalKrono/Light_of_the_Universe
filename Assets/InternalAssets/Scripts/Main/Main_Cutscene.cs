using System.Collections;
using UnityEngine;

public class Main_Cutscene : MonoBehaviour
{
    [SerializeField] private Animator characterZ2Animator;
    [SerializeField] private Animator characterKainAnimator;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private ScreenDark screenDark;
    [SerializeField] private int parameterId = 0;
    [SerializeField] private float KainSleepTime = 2;

    private void Awake()
    {
        GameManager.Initialize();
        GameScreenDark.SetDarkEvent(true);
        GameMenu.DisactivateGameMenuEvent();

        AnimationBehaviour[] animationBehaviours = characterZ2Animator.GetBehaviours<AnimationBehaviour>();
        animationBehaviours[0].OnStateExitEvent += KainSleep;
        animationBehaviours[1].OnStateExitEvent += End;

        characterKainAnimator.GetBehaviour<AnimationBehaviour>().OnStateExitEvent += OnKainShortToDoor;

        screenDark.SetDark(false);
        StartCoroutine(GameScreenDark.ITransparentEvent());
    }

    private void KainSleep()
    {
        StartCoroutine(IKainSleep());
    }
    private IEnumerator IKainSleep()
    {
        yield return StartCoroutine(screenDark.IDark());
        yield return new WaitForSeconds(KainSleepTime);
        yield return StartCoroutine(screenDark.ITransparent());
        characterKainAnimator.SetFloat(characterKainAnimator.GetParameter(parameterId).name, 1);
    }

    private void OnKainShortToDoor()
    {
        doorAnimator.SetFloat(doorAnimator.GetParameter(parameterId).name, 1);
        characterZ2Animator.SetFloat(characterZ2Animator.GetParameter(parameterId).name, 1);
    }

    private void End()
    {
        characterZ2Animator.gameObject.SetActive(false);
        StartCoroutine(IEnd());
    }

    private IEnumerator IEnd()
    {
        yield return StartCoroutine(GameScreenDark.IDarkEvent());
        SceneController.LoadNextStoryScene();
    }
}

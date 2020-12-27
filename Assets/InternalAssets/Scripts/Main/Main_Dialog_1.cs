using System.Collections;
using UnityEngine;

public class Main_Dialog_1 : Main_Dialog
{
    [SerializeField] private Animator characterDictorAnimator;
    [SerializeField] private Animator characterKainAnimator;
    [SerializeField] private GameObject Note;
    [SerializeField] private int parameterId;
    [SerializeField] private int dialogPart1;

    private int dialogPart;

    protected override void bAwake()
    {
        system_dialogs.gameObject.SetActive(false);
        characterKainAnimator.GetBehaviour<AnimationBehaviour>().OnStateEnterEvent += StartDialog;
        characterDictorAnimator.GetBehaviour<AnimationBehaviour>().OnStateExitEvent += GiveACard;
    }

    private void StartDialog()
    {
        StartCoroutine(IStartDialog());
    }

    private void OnNextPage()
    {
        dialogPart += 1;
        if (dialogPart - 1 == dialogPart1)
        {
            system_dialogs.SetAnswerButtonsInteractable(false);
            characterDictorAnimator.SetFloat(characterDictorAnimator.GetParameter(parameterId).name, 2);
        }
    }

    private void GiveACard()
    {
        characterKainAnimator.SetFloat(characterKainAnimator.GetParameter(parameterId).name, 1);
        Note.SetActive(true);
        system_dialogs.SetAnswerButtonsInteractable(false);
        StartCoroutine(Answer());
    }
    private IEnumerator IStartDialog()
    {
        while (Vector3.Distance(characterKainAnimator.transform.position, Vector3.zero) > 0.01f)
        {
            characterKainAnimator.transform.position = Vector3.Lerp(characterKainAnimator.transform.position, Vector3.zero, Time.deltaTime);
            yield return null;
        }
        characterDictorAnimator.SetFloat(characterDictorAnimator.GetParameter(parameterId).name, 1);
        system_dialogs.OnNextPage += OnNextPage;
        system_dialogs.gameObject.SetActive(true);
    }
    private IEnumerator Answer()
    {
        yield return new WaitForSeconds(2);
        system_dialogs.Answer(0);
    }
}

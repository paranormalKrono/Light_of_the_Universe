using UnityEngine;

public class Main_Dialog_3 : Main_Dialog
{
    [SerializeField] private Animator characterDictorAnimator;
    [SerializeField] private Animator characterKainAnimator;
    [SerializeField] private int parameterId;
    [SerializeField] private int dialogPart1;
    [SerializeField] private int dialogPart2;

    private int dialogPart;

    protected override void bAwake()
    {
        system_dialogs.OnNextPage += OnNextPage;
    }

    private void OnNextPage()
    {
        dialogPart += 1;
        if (dialogPart - 1 == dialogPart1)
        {
            characterKainAnimator.SetFloat(characterKainAnimator.GetParameter(parameterId).name, 1);
            characterDictorAnimator.SetFloat(characterDictorAnimator.GetParameter(parameterId).name, 1);
        }
        else if (dialogPart - 1 == dialogPart2)
        {
            characterKainAnimator.SetFloat(characterKainAnimator.GetParameter(parameterId).name, 2);
            characterDictorAnimator.SetFloat(characterDictorAnimator.GetParameter(parameterId).name, 2);
        }
    }
}

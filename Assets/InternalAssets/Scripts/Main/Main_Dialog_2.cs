using UnityEngine;


public class Main_Dialog_2 : Main_Dialog
{
    [SerializeField] private Animator characterZ3Animator;
    [SerializeField] private int parameterId = 0;
    [SerializeField] private GameObject camera0;
    [SerializeField] private GameObject camera1;
    [SerializeField] private GameObject camera2;
    [SerializeField] private int dialogPart1 = 1;
    [SerializeField] private int dialogPart2 = 4;
    [SerializeField] private int dialogPart3 = 13;

    private int dialogPart;

    protected override void bAwake()
    {
        system_dialogs.gameObject.SetActive(false);
    }

    public void StartDialog()
    {
        system_dialogs.OnNextPage += OnNextPage;
        system_dialogs.gameObject.SetActive(true);
    }

    private void OnNextPage()
    {
        if (dialogPart == dialogPart1)
        {
            camera0.SetActive(false);
            camera1.SetActive(true);
        }
        else if (dialogPart == dialogPart2)
        {
            camera1.SetActive(false);
            camera2.SetActive(true);
            characterZ3Animator.SetFloat(characterZ3Animator.GetParameter(parameterId).name, 1);
        }
        else if (dialogPart == dialogPart3)
        {
            characterZ3Animator.SetFloat(characterZ3Animator.GetParameter(parameterId).name, 2);
        }
        dialogPart += 1;
    }
}

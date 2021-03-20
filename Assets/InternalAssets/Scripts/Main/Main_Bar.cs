using System.Collections;
using UnityEngine;

public class Main_Bar : MonoBehaviour
{
    [SerializeField] private System_Dialogs system_dialogs;
    [SerializeField] private CameraPointsController cameraPointsController;
    [SerializeField] private Animator[] characterAnimators;
    [SerializeField] private string animationsName = "Action";
    [SerializeField] private int dialogNode1 = 1;
    [SerializeField] private int dialogNode2_1 = 11;
    [SerializeField] private int dialogNode2_2 = 14;
    [SerializeField] private int dialogNode2_3 = 17;
    [SerializeField] private int dialogNode3 = 19;
    [SerializeField] private ScreenDark screenDark;
    [SerializeField] private GameObject Barracuda;
    [SerializeField] private GameObject Hypersnapsh;
    [SerializeField] private GameObject Micksture;
    [SerializeField] private Transform barmen;
    [SerializeField] private Transform newBarmenTransform;
    [SerializeField] private DialogCanvas DialogCanvas;

    private bool isEnd;

    private void Awake()
    {
        GameManager.Initialize();
        DialogCanvas.InstantlyClose();

        cameraPointsController.MoveCameraToPoint(0);

        for (int i = 0; i < characterAnimators.Length; ++i)
        {
            characterAnimators[i].Play(animationsName + i);
        }

        system_dialogs.Initialise(GameText.GetBarDialogEvent(), GameText.GetNamesEvent());
        system_dialogs.OnNextNode += OnNextNode;
        system_dialogs.EndEvent = End;
    }

    private void OnNextNode(int curNode)
    {
        if (curNode == dialogNode1)
        {
            StartCoroutine(IDialog1());
        }
        else if (curNode == dialogNode2_1 || curNode == dialogNode2_2 || curNode == dialogNode2_3)
        {
            StartCoroutine(IDialog2(curNode));
        }
        else if (curNode == dialogNode3)
        {
            StartCoroutine(IDialog3());
        }
    }

    private IEnumerator IDialog1()
    {
        DialogCanvas.Close();
        yield return StartCoroutine(screenDark.IDark());
        cameraPointsController.MoveCameraToPoint(1);
        DialogCanvas.Open();
        yield return StartCoroutine(screenDark.ITransparent());
    }

    private IEnumerator IDialog2(int curNode)
    {
        DialogCanvas.Close();
        yield return StartCoroutine(screenDark.IDark());
        cameraPointsController.MoveCameraToPoint(2);
        if (curNode == dialogNode2_1)
        {
            barmen.position = newBarmenTransform.position;
            barmen.rotation = newBarmenTransform.rotation;
            Barracuda.SetActive(true);
        }
        else if (curNode == dialogNode2_2)
        {
            Hypersnapsh.SetActive(true);
        }
        else
        {
            barmen.position = newBarmenTransform.position;
            barmen.rotation = newBarmenTransform.rotation;
            Micksture.SetActive(true);
        }
        DialogCanvas.Open();
        yield return StartCoroutine(screenDark.ITransparent());
    }

    private IEnumerator IDialog3()
    {
        DialogCanvas.Close();
        yield return StartCoroutine(screenDark.IDark());
        cameraPointsController.MoveCameraToPoint(3);
        DialogCanvas.Open();
        yield return StartCoroutine(screenDark.ITransparent());
    }

    private void Update()
    {
        if (!isEnd)
        {
            if (Input.GetKey(KeyCode.N))
            {
                End();
            }
        }
    }

    private void End()
    {
        if (!isEnd)
        {
            isEnd = true;
            StaticSettings.isCompleteSomething = true;
            SceneController.LoadNextStoryScene();
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameDialogs : MonoBehaviour, IDeactivated
{
    [SerializeField] private Text DialogNext; // Включить следующий диалог
    [SerializeField] private Text DialogText; // Текст диалога
    [SerializeField] private Image BorderImage; // Фон диалога

    TextInGame.Dialog[] Dialogs;
    TextInGame.InGameDialog[] InGameDialogs;
    internal TextNames Names;

    private int dialogNow;
    private int dialogsCount;
    private int inGameDialogNow;

    private bool isDialog; // Показывается ли диалог

    public delegate void Event();
    private static Event OnEndDialogEvent;
    public static Event NextInGameDialogEvent;

    public delegate void EventE(Event @event);
    public static EventE StartDialogEvent;
    public static EventE NextInGameDialogEEvent;

    public delegate void EventI(int integer);
    public static EventI ShowInGameDialogEvent;

    private void Awake()
    {
        StartDialogEvent = StartDialog;
        NextInGameDialogEvent = NextInGameDialog;
        NextInGameDialogEEvent = NextInGameDialog;
        ShowInGameDialogEvent = ShowInGameDialog;
    }

    private void Update()
    {
        if (isDialog)
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.RightShift))
            {
                NextDialog();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                PrevDialog();
            }
        }
    }

    private void StartDialog(Event OnEndDialog)
    {
        OnEndDialogEvent = OnEndDialog;
        SetActiveDialog(true);
        UpdateDialogText();
        DialogNext.enabled = true;
        isDialog = true;
    }
    private void PrevDialog()
    {
        if (dialogNow > 0)
        {
            dialogNow -= 1;
            UpdateDialogText();
        }
    }
    private void NextDialog()
    {
        if (dialogNow < dialogsCount - 1)
        {
            dialogNow += 1;
            UpdateDialogText();
        }
        else
        {
            //SetActiveGoal(true);
            SetActiveDialog(false);
            DialogNext.enabled = false;
            isDialog = false;
            OnEndDialogEvent();
        }
    }
    private void UpdateDialogText()
    {
        SetDialogText(DialogText, Names.names[Dialogs[dialogNow].nameid].textcolor, Dialogs[dialogNow].textsize, Dialogs[dialogNow].prevtext + Names.names[Dialogs[dialogNow].nameid].name + ": " + Dialogs[dialogNow].text);
    }

    #region InGame
    private void NextInGameDialog()
    {
        ShowInGameDialog(inGameDialogNow);
        inGameDialogNow += 1;
    }
    private void ShowInGameDialog(int index)
    {
        StartCoroutine(IInGameDialog(index));
    }

    private void NextInGameDialog(Event e)
    {
        StartCoroutine(IInGameDialogE(inGameDialogNow, e));
        inGameDialogNow += 1;
    }
    private IEnumerator IInGameDialogE(int index, Event e)
    {
        yield return IInGameDialog(index);
        e();
    }

    private IEnumerator IInGameDialog(int index)
    {
        SetActiveDialog(true);
        float t = InGameDialogs[index].time;
        foreach (TextInGame.Dialog dialog in InGameDialogs[index].dialogs)
        {
            SetDialogText(DialogText, Names.names[dialog.nameid].textcolor, dialog.textsize, dialog.prevtext + Names.names[dialog.nameid].name + ": " + dialog.text);
            yield return new WaitForSeconds(t);
        }
        SetActiveDialog(false);
    }
    #endregion

    private void SetActiveDialog(bool t)
    {
        BorderImage.enabled = t;
        DialogText.enabled = t;
    }

    private void SetDialogText(Text text, string color, int textsize, string textdialog)
    {
        ColorUtility.TryParseHtmlString("#" + color, out Color nameColor);
        text.color = nameColor;
        text.fontSize = textsize;
        text.text = textdialog;
    }

    internal void SetDialogTextAsset(TextInGame textInGame)
    {
        Dialogs = textInGame.dialogs;
        InGameDialogs = textInGame.ingamedialogs;
        dialogsCount = Dialogs.Length;
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        SetActiveDialog(false);
        DialogNext.enabled = false;
        isDialog = false;
        dialogNow = 0;
        inGameDialogNow = 0;
    }
}

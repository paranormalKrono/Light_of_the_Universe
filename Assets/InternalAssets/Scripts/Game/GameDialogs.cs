using System.Collections;
using System.Collections.Generic;
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
    private int currentInGameDialog = -1;

    private bool isDialog; // Показывается ли диалог
    private bool isInGameDialog; // Показывается ли игровой диалог

    public delegate void Event();
    private static Event OnEndDialogEvent;

    public delegate void EventE(Event @event);
    public static EventE StartDialogEvent;

    public delegate void EventI(int integer);
    public static EventI ShowInGameDialogEvent;
    public delegate IEnumerator IEventI(int integer);
    public static IEventI IShowInGameDialogEvent;

    public static event EventI OnNextDialog;

    private Queue<int> inGameDialogsQueue = new Queue<int>();

    private void Awake()
    {
        StartDialogEvent = StartDialog;
        ShowInGameDialogEvent = ShowInGameDialog;
        IShowInGameDialogEvent = IShowInGameDialog;
        _IInGameDialog = IInGameDialog();
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
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OnEndDialogEvent();
                DisableDialog();
            }
        }
    }

    public void Deactivate()
    {
        ClearInGameDialog();
        DisableDialog();
    }

    #region StartDialog

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
            OnNextDialog?.Invoke(dialogNow);
        }
        else
        {
            DisableDialog();
            OnEndDialogEvent();
        }
    }

    private void UpdateDialogText()
    {
        SetDialogText(DialogText, Names.names[Dialogs[dialogNow].nameid].textcolor, Dialogs[dialogNow].textsize, Dialogs[dialogNow].prevtext + Names.names[Dialogs[dialogNow].nameid].name + ": " + Dialogs[dialogNow].text);
    }


    private void DisableDialog()
    {
        DialogNext.enabled = false;
        isDialog = false;
        dialogNow = 0;
        OnNextDialog = null;
        SetActiveDialog(false);
    }

    #endregion

    #region InGame

    private void ShowInGameDialog(int index) => StartCoroutine(IShowInGameDialog(index));
    private IEnumerator IShowInGameDialog(int index)
    {
        if (index != currentInGameDialog && !inGameDialogsQueue.Contains(index))
        {
            if (isInGameDialog)
            {
                inGameDialogsQueue.Enqueue(index);
                while (currentInGameDialog != index)
                {
                    yield return null;
                }
                while (currentInGameDialog == index)
                {
                    yield return null;
                }
            }
            else
            {
                currentInGameDialog = index;
                _IInGameDialog = IInGameDialog();
                yield return StartCoroutine(_IInGameDialog);
            }
        }
    }

    private IEnumerator _IInGameDialog;
    private IEnumerator IInGameDialog()
    {
        isInGameDialog = true;
        SetActiveDialog(true);
        float t = InGameDialogs[currentInGameDialog].time;
        foreach (TextInGame.Dialog dialog in InGameDialogs[currentInGameDialog].dialogs)
        {
            if (Names.names[dialog.nameid].name == "")
            {
                SetDialogText(DialogText, Names.names[dialog.nameid].textcolor, dialog.textsize, dialog.prevtext + dialog.text);
            }
            else
            {
                SetDialogText(DialogText, Names.names[dialog.nameid].textcolor, dialog.textsize, dialog.prevtext + Names.names[dialog.nameid].name + ": " + dialog.text);
            }
            yield return new WaitForSeconds(t);
        }
        if (inGameDialogsQueue.Count > 0)
        {
            currentInGameDialog = inGameDialogsQueue.Dequeue();
            _IInGameDialog = IInGameDialog();
            StartCoroutine(_IInGameDialog);
        }
        else
        {
            ClearInGameDialog();
        }
    }

    private void ClearInGameDialog()
    {
        StopAllCoroutines();
        currentInGameDialog = -1;
        inGameDialogsQueue.Clear();
        isInGameDialog = false;
        SetActiveDialog(false);
    }

    #endregion


    internal void SetDialogTextAsset(TextInGame textInGame)
    {
        Dialogs = textInGame.dialogs;
        InGameDialogs = textInGame.ingamedialogs;
        dialogsCount = Dialogs.Length;
    }

    private void SetDialogText(Text text, string color, int textsize, string textdialog)
    {
        ColorUtility.TryParseHtmlString("#" + color, out Color nameColor);
        text.color = nameColor;
        text.fontSize = textsize;
        text.text = textdialog;
    }

    private void SetActiveDialog(bool t)
    {
        BorderImage.enabled = t;
        DialogText.enabled = t;
    }
}

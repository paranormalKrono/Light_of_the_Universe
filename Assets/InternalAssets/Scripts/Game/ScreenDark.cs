using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDark : MonoBehaviour
{
    [SerializeField] private float timeToDark = 1; // Время для изменения изображения
    [SerializeField] private Image ImageDark; // Тёмное изображение

    private bool isWorking;

    private Color colorBlack = Color.black;
    private Color colorClear = Color.clear;
    private Color tempColor;

    public delegate void EventHandlerB(bool b);
    public static EventHandlerB SetDarkEvent;
    public delegate IEnumerator IEnumeratorHandler();
    public static IEnumeratorHandler IDarkEvent;
    public static IEnumeratorHandler ITransparentEvent;

    private void Awake()
    {
        SetDarkEvent = SetDark;
        IDarkEvent = IDark;
        ITransparentEvent = ITransparent;
        ImageDark.gameObject.SetActive(true);
    }

    private void SetDark(bool t)
    {
        ImageDark.color = t ? colorBlack : colorClear;
    }



    private IEnumerator IChangeColorNow;


    private IEnumerator IDark() // Если нужно сделать переход на тёмное изображение, то делаем изображение непрозрачным 
    {
        if (isWorking)
        {
            StopCoroutine(IChangeColorNow);
        }
        IChangeColorNow = _IDark();
        isWorking = true;
        yield return StartCoroutine(IChangeColorNow);
        isWorking = false;
    }

    private IEnumerator ITransparent() // Если нужно сделать переход на прозрачное изображение, то делаем изображение прозрачным 
    {
        if (isWorking)
        {
            StopCoroutine(IChangeColorNow);
        }
        IChangeColorNow = _ITranparent();
        isWorking = true;
        yield return StartCoroutine(IChangeColorNow);
        isWorking = false;
    }



    private IEnumerator _IDark()
    {
        tempColor = ImageDark.color;
        while (tempColor.a < 1)
        {
            tempColor.a += Time.deltaTime / timeToDark;
            ImageDark.color = tempColor;
            yield return null;
        }
        ImageDark.color = colorBlack;
        yield return new WaitForSeconds(timeToDark);
    }
    private IEnumerator _ITranparent()
    {
        tempColor = ImageDark.color;
        while (tempColor.a > 0)
        {
            tempColor.a -= Time.deltaTime / timeToDark;
            ImageDark.color = tempColor;
            yield return null;
        }
        ImageDark.color = colorClear;
        yield return new WaitForSeconds(timeToDark);
    }


}
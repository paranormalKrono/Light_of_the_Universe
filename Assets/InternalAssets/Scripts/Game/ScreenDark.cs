using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDark : MonoBehaviour
{
    [SerializeField] private float timeToDark = 1; // Время для изменения изображения
    [SerializeField] private Image ImageDark; // Тёмное изображение

    private bool isWorking;

    private Color tempColor;

    public delegate IEnumerator IEnumeratorHandler();

    private void Awake()
    {
        ImageDark.gameObject.SetActive(true);
    }

    internal void SetDark(bool t)
    {
        ImageDark.color = t ? Color.black : Color.clear;
    }



    private IEnumerator IChangeColorNow;


    internal IEnumerator IDark() // Если нужно сделать переход на тёмное изображение, то делаем изображение непрозрачным 
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

    internal IEnumerator ITransparent() // Если нужно сделать переход на прозрачное изображение, то делаем изображение прозрачным 
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
        ImageDark.color = Color.black;
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
        ImageDark.color = Color.clear;
        yield return new WaitForSeconds(timeToDark);
    }

}
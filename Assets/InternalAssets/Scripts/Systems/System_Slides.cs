using UnityEngine;

public class System_Slides : MonoBehaviour
{
    [SerializeField] internal GameObject[] Slides;

    private int slideNow;
    private bool isSlides;

    internal delegate void EventDelegate();
    internal EventDelegate EndEvent;

    public void Initialise()
    {
        Slides[0].SetActive(true);
        isSlides = true;
    }
    private void Update()
    {
        if (isSlides)
        {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.RightShift))
            {
                NextSlide();
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                PrevSlide();
            }
        }
    }

    private void NextSlide()
    {
        if (slideNow < Slides.Length - 1)
        {
            Slides[slideNow].SetActive(false);
            slideNow += 1;
            Slides[slideNow].SetActive(true);
        }
        else
        {
            isSlides = false;
            EndEvent();
        }
    }
    private void PrevSlide()
    {
        if (slideNow > 0)
        {
            Slides[slideNow].SetActive(false);
            slideNow -= 1;
            Slides[slideNow].SetActive(true);
        }
    }
}

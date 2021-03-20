using UnityEngine;
using UnityEngine.UI;

public class DialogCanvas : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button button;
    [SerializeField] private AnimationClip animationOpen;
    [SerializeField] private AnimationClip animationClose;
    [SerializeField] private AnimationClip animationClosed;
    private bool isClose;

    public void OnButton()
    {
        if (isClose)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    public void InstantlyClose()
    {
        animator.Play(animationClosed.name);
        isClose = true;
    }
    public void Open()
    {
        animator.Play(animationOpen.name);
        isClose = false;
    }
    public void Close()
    {
        animator.Play(animationClose.name);
        isClose = true;
    }
}

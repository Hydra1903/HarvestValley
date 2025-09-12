using UnityEngine;

public class UIView : MonoBehaviour
{
    public string viewName;   
    public Animator animator;

    public void Show()
    {
        gameObject.SetActive(true);
        if (animator != null)
            animator.Play("On");
    }

    public void Hide()
    {
        if (animator != null)
            animator.Play("Off");
        else
            gameObject.SetActive(false);
    }
    public void OnHideAnimationEnd()
    {
    gameObject.SetActive(false);
    }
}


using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    public Slider actionBar;
    public Image frame;
    void Start()
    {
        
    }
    public float ActionTime(float timeAnimation)
    {
        actionBar.value += Time.deltaTime * 1 / timeAnimation;
        return actionBar.value;
    }
    public void Reset()
    {
        actionBar.value = 0;
        frame.fillAmount = 0;
    }
}

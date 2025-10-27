using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderLoading : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;
    void Update()
    {
        text.text = ((int)(slider.value * 100)).ToString() + "/100%";
    }
}

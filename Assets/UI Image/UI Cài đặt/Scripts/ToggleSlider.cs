using UnityEngine;
using UnityEngine.UI;

public class ToggleSlider : MonoBehaviour
{
    public Toggle toggle;
    public RectTransform handlePosition;
    public Image background;
    public Image handle;
    public Color onColorBackground = new Color(0.3f, 0.7f, 0.2f);
    public Color offColorBackground = new Color(0.5f, 0.4f, 0.2f);
    public Color onColorHandle = new Color(0.3f, 0.7f, 0.2f);
    public Color offColorHandle = new Color(0.5f, 0.4f, 0.2f);
    private Vector2 onPos;
    private Vector2 offPos;
    private Vector2 targetPos;

    public float speed = 10f;

    void Start()
    {
        float range = (GetComponent<RectTransform>().rect.width - handlePosition.rect.width) / 2;
        onPos = new Vector2(range, 0);
        offPos = new Vector2(-range, 0);

        toggle.onValueChanged.AddListener(OnToggle);
        targetPos = toggle.isOn ? onPos : offPos;
        handlePosition.anchoredPosition = targetPos;
        background.color = toggle.isOn ? onColorBackground : offColorBackground;
        handle.color = toggle.isOn ? onColorHandle : offColorHandle;
    }

    void Update()
    {
        handlePosition.anchoredPosition = Vector2.Lerp(handlePosition.anchoredPosition, targetPos, Time.deltaTime * speed);
    }

    void OnToggle(bool isOn)
    {
        targetPos = isOn ? onPos : offPos;
        background.color = isOn ? onColorBackground : offColorBackground;
        handle.color = isOn ? onColorHandle : offColorHandle;
    }
}

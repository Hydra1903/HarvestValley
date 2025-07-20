using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public GameObject tooltipPanel;
    public TextMeshProUGUI nameItemText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI typeText;
    public Image iconItem;

    public RectTransform canvasRectTransform;
    public Vector2 deviation;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        tooltipPanel.SetActive(false);
    }

    private void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,           
                Input.mousePosition,
                null,                        
                out localPos
            );
            tooltipPanel.GetComponent<RectTransform>().anchoredPosition = localPos + deviation;
        }
    }

    public void Show(string name, string season, string description, string type, Sprite icon)
    {
        tooltipPanel.SetActive(true);
        nameItemText.text = name;
        seasonText.text = season;
        descriptionText.text = description;
        typeText.text = type;
        iconItem.sprite = icon;
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}

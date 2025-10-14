using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public GameObject tooltipPanel;
    public TextMeshProUGUI nameItemText;
    public TextMeshProUGUI seasonText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI typeText;
    public Image iconItem;
    public Image iconType;
    public Sprite plant;
    public Sprite seed;
    public Sprite tool;
    public Sprite animalProduct;

    public RectTransform canvasRectTransform;
    public Vector2 deviation;

    Coroutine currentRoutine;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

    public void Show(string name, string season, string description, string type, Sprite icon, ItemType itemType)
    {
        nameItemText.text = name;
        seasonText.text = season;
        descriptionText.text = description;
        typeText.text = type;
        iconItem.sprite = icon;
        switch (itemType)
        {
            case ItemType.Plant:
                iconType.sprite = plant;
                break;
            case ItemType.Seed:
                iconType.sprite = seed;
                break;
            case ItemType.Tool:
                iconType.sprite = tool;
                break;
            case ItemType.AnimalProduct:
                iconType.sprite = animalProduct;
                break;
        }
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowDelay());
    }
    IEnumerator ShowDelay()
    {
        yield return new WaitForSeconds(0.7f);
        tooltipPanel.SetActive(true);
        currentRoutine = null;
    }
    public void Hide()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        tooltipPanel.SetActive(false);
    }
}

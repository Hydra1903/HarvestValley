using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ItemAdded : MonoBehaviour
{
    public static ItemAdded Instance { get; private set; }
    public GameObject panel;
    public Image iconItem;
    public TextMeshProUGUI textAmount;
    private Coroutine currentRoutine;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowItemAdded(Sprite icon, string amount)
    {       
        StartCoroutine(HandleRoutine());
        iconItem.sprite = icon;
        textAmount.text = "+"+amount;
        UIManager.Instance.ShowUI("ItemAdded");
        UISounds.Instance.PlaySound_CollectItem();
    }
    IEnumerator HandleRoutine()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ExecuteAfterDelay());
        yield return currentRoutine;
    }

    private IEnumerator ExecuteAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.HideUI("ItemAdded");
    }
}

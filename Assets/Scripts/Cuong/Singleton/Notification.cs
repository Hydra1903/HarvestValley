using UnityEngine;
using System.Collections;
using TMPro;
public class Notification : MonoBehaviour
{
    public static Notification Instance { get; private set; }
    public GameObject panelNotification;
    public TextMeshProUGUI textNotification;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowNotification(string message)
    {
        StartCoroutine(ExecuteAfterDelay());
        textNotification.text = message;
        panelNotification.SetActive(true);
    }

    private IEnumerator ExecuteAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        panelNotification.SetActive(false);
    }
}

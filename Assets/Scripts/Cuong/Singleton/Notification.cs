using UnityEngine;
using System.Collections;
using TMPro;
public class Notification : MonoBehaviour
{
    public static Notification Instance { get; private set; }
    public GameObject panelNotification;
    public TextMeshProUGUI textNotification;
    private Coroutine currentRoutine;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowNotification(string message)
    {
        StartCoroutine(HandleRoutine());
        textNotification.text = message;
        panelNotification.SetActive(true);
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
        yield return new WaitForSeconds(2f);
        panelNotification.SetActive(false);
    }
}

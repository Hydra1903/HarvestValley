using TMPro;
using UnityEngine;
using System.Collections;

public class CompleteAchivements : MonoBehaviour
{
    public static CompleteAchivements Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public TextMeshProUGUI nameAchivements;
    private Coroutine currentRoutine;
    public void ShowCompleteAchivements(string name)
    {
        StartCoroutine(HandleRoutine());
        nameAchivements.text = name;
        UIManager.Instance.ShowUI("CompleteAchivements");
        //UISounds.Instance.PlaySound_CollectItem();
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
        UIManager.Instance.HideUI("CompleteAchivements");
    }
}

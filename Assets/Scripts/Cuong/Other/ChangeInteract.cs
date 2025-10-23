using UnityEngine;
public enum EModeHand
{
    Mode1,
    Mode2
}
public enum EChangeInteractState
{
    On,
    Off
}
public class ChangeInteract : MonoBehaviour
{
    public static ChangeInteract Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public GameObject[] highlight;
    public EModeHand currentModeHand;
    public EChangeInteractState currentState = EChangeInteractState.Off;
    void Update()
    {
        if (currentState == EChangeInteractState.On)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                HideHightlight();
                highlight[0].SetActive(true);
                currentModeHand = EModeHand.Mode1;

            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                HideHightlight();
                highlight[1].SetActive(true);
                currentModeHand = EModeHand.Mode2;
            }
        }
    }
    public void CheckCurrentState()
    {
        if (currentState == EChangeInteractState.On)
        {
            UIManager.Instance.ShowUI("ChangeInteract");
        }
        else if (currentState == EChangeInteractState.Off)
        {
            UIManager.Instance.HideUI("ChangeInteract");
        }
    }
    public void HideHightlight()
    {
        for (int i = 0; i < highlight.Length; i++)
        {
            highlight[i].SetActive(false);
        }
    }
}

using UnityEngine;
public enum EModeHoe
{
    Mode1,
    Mode2,
    Mode3
}
public class ChangeMode : MonoBehaviour
{
    public static ChangeMode Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public GameObject[] highlight;
    public ItemData hoe;
    public EModeHoe currentModeHoe;
    void Start()
    {
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HideHightlight();
            highlight[0].SetActive(true);
            currentModeHoe = EModeHoe.Mode1;

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HideHightlight();
            highlight[1].SetActive(true);
            currentModeHoe = EModeHoe.Mode2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HideHightlight();
            highlight[2].SetActive(true);
            currentModeHoe = EModeHoe.Mode3;
        }
    }
    public void CheckCurrentItemHoe()
    {
        if (HotBarUI.Instance.currentItem != null)
        {
            if (HotBarUI.Instance.currentItem.itemData == hoe)
            {
                UIManager.Instance.ShowUI("ChangeMode");
            }
            else
            {
                UIManager.Instance.HideUI("ChangeMode");
            }
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

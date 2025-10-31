using UnityEngine;

public class AnimalInfo : MonoBehaviour
{
    [Header("References")]
    public InfoPanelUI panelUI;  // panel chung
    public AnimalData data;

    // Hàm gọi từ PlayerLookAtAnimal
    public void ShowInfo()
    {
        if (panelUI == null || data == null) return;
        panelUI.Show(data, this);
    }

    public void HideInfo()
    {
        panelUI?.Hide();
    }
}

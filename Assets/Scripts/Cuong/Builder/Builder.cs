using UnityEngine;

public class Builder : MonoBehaviour
{
    public int currentlevelBarn = 1;
    public GameObject[] BuildingBarn;
    public int[] priceBuildingBarn;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    #region ----- BUILDING BARN -----
    public void LoadBuildingBarn()
    {
        for (int i = 0; i < BuildingBarn.Length; i++)
        {
            if (currentlevelBarn == i + 1)
            {
                HideAllBarn();
                BuildingBarn[i].SetActive(true);
            }
        }
    }
    public void UpgradeBuildingBarn1to2()
    {       
        if (Gold.Instance.gold >= priceBuildingBarn[0])
        {
            currentlevelBarn++;
            Gold.Instance.gold -= priceBuildingBarn[0];
            LoadBuildingBarn();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
    }
    public void UpgradeBuildingBarn2to3()
    {
        if (Gold.Instance.gold >= priceBuildingBarn[0] && currentlevelBarn == 2)
        {
            currentlevelBarn++;
            Gold.Instance.gold -= priceBuildingBarn[1];
            LoadBuildingBarn();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
    }
    public void HideAllBarn()
    {
        for (int i = 0; i < BuildingBarn.Length; i++)
        {
            BuildingBarn[i].SetActive(false);
        }
    }
    #endregion
    public void UpdateBuildingHome()
    {

    }
    public void UpdateCropland2()
    {

    }
    public void UpdateCropland3()
    {

    }
    public void UpdateGrassland()
    {

    }
    public void UpdateBuildingPen1()
    {

    }
    public void UpdateBuildingPen2()
    {

    }
    public void UpdateBuildingGreenhouse1()
    {

    }
    public void UpdateBuildingGreenhouse2()
    {

    }
}

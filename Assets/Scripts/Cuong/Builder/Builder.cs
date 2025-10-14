using UnityEngine;

public class Builder : MonoBehaviour
{
    public int currentlevelBarn = 1;
    public GameObject[] BuildingBarn;
    public int[] priceBuildingBarn;

    public int currentlevelHome = 1;
    public GameObject[] BuildingHome;
    public int[] priceBuildingHome;

    public GameObject[] Farmland;
    public int[] priceFarmland;
    public bool isUnlockFarmland2;
    public bool isUnlockFarmland3;

    public GameObject Grassland;
    public int priceGrassland;
    public bool isUnlockGrassland;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
    #region ----- BUILDING BARN -----
    public void LoadBuildingBarn()
    {
        HideAllBarn();
        for (int i = 0; i < BuildingBarn.Length; i++)
        {
            if (currentlevelBarn == i + 1)
            {              
                BuildingBarn[i].SetActive(true);
            }
        }
    }
    public void UpgradeBuildingBarn1to2()
    {       
        if (Gold.Instance.gold >= priceBuildingBarn[0] && LevelManager.Instance.currentLevel >= 10)
        {
            currentlevelBarn++;
            Gold.Instance.gold -= priceBuildingBarn[0];
            LoadBuildingBarn();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else if (Gold.Instance.gold < priceBuildingBarn[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
    }
    public void UpgradeBuildingBarn2to3()
    {
        if (Gold.Instance.gold >= priceBuildingBarn[1] && currentlevelBarn == 2 && LevelManager.Instance.currentLevel >= 20)
        {
            currentlevelBarn++;
            Gold.Instance.gold -= priceBuildingBarn[1];
            LoadBuildingBarn();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else if (Gold.Instance.gold < priceBuildingBarn[1])
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

    #region ----- BUILDING HOME -----
    public void LoadBuildingHome()
    {
        HideAllHome();
        for (int i = 0; i < BuildingHome.Length; i++)
        {
            if (currentlevelHome == i + 1)
            {
                BuildingHome[i].SetActive(true);
            }
        }
    }
    public void UpdateBuildingHome1to2()
    {
        if (Gold.Instance.gold >= priceBuildingHome[0] && currentlevelHome == 1 && LevelManager.Instance.currentLevel >= 15)
        {
            currentlevelHome++;
            Gold.Instance.gold -= priceBuildingHome[0];
            LoadBuildingHome();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else if (Gold.Instance.gold < priceBuildingHome[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
    }
    public void HideAllHome()
    {
        for (int i = 0; i < BuildingHome.Length; i++)
        {
            BuildingHome[i].SetActive(false);
        }
    }
    #endregion
    #region ----- FARMLAND -----
    public void LoadFarmland(int index)
    {
        if (isUnlockFarmland2)
        {
            Farmland[index].SetActive(false);
        }
        if (isUnlockFarmland3)
        {
            Farmland[index].SetActive(false);
        }
    }
    public void UnlockFarmland2()
    {
        if (Gold.Instance.gold >= priceFarmland[0])
        {
            Gold.Instance.gold -= priceFarmland[0];
            isUnlockFarmland2 = true;
            LoadFarmland(0);
            Notification.Instance.ShowNotification("Đã mở khóa thành công!");
        }
        else if (Gold.Instance.gold < priceFarmland[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
    }
    public void UnlockFarmland3()
    {
        if (Gold.Instance.gold >= priceFarmland[1])
        {
            Gold.Instance.gold -= priceFarmland[1];
            isUnlockFarmland3 = true;
            LoadFarmland(1);
            Notification.Instance.ShowNotification("Đã mở khóa thành công!");
        }
        else if (Gold.Instance.gold < priceFarmland[1])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
    }
    public void LoadGrassland()
    {
        Grassland.SetActive(false);
    }
    public void UnlockGrassland()
    {
        if (Gold.Instance.gold >= priceGrassland)
        {
            Gold.Instance.gold -= priceGrassland;
            isUnlockGrassland = true;
            LoadGrassland();
            Notification.Instance.ShowNotification("Đã mở khóa thành công!");
        }
        else if (Gold.Instance.gold < priceGrassland)
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
    }
    #endregion
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

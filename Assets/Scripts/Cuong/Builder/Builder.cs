using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
public class Builder : MonoBehaviour
{
    public static Builder Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public BuilderUI builderUI;
    public int currentlevelBarn = 1;
    public GameObject[] BuildingBarn;
    public int[] priceBuildingBarn;

    public int currentlevelHome = 1;
    public GameObject[] BuildingHome;
    public int[] priceBuildingHome;

    public GameObject[] Farmland;
    public GameObject[] blockFarmland;
    public int[] priceFarmland;
    public bool isUnlockFarmland2;
    public bool isUnlockFarmland3;

    public GameObject Grassland;
    public GameObject blockGrassland;
    public int priceGrassland;
    public bool isUnlockGrassland;

    public int currentlevelPen1 = 0;
    public int currentlevelPen2 = 0;
    public GameObject[] BuildingPen1;
    public GameObject[] BuildingPen2;
    public GameObject[] blockPen;
    public GameObject[] interaction;
    public int[] priceBuildingPen1;
    public int[] priceBuildingPen2;
    public bool isUnlockPen1;
    public bool isUnlockPen2;

    public GameObject[] BuildingGreenhouse;
    public GameObject[] blockGreenhouse;
    public int[] priceBuildingGreenhouse;
    public bool isUnlockGreenhouse1;
    public bool isUnlockGreenhouse2;

    void Start()
    {
        LoadAllBuilding();
    }

    public void LoadAllBuilding()
    {
        LoadBuildingBarn();
        LoadBuildingHome();
        LoadFarmland();
        LoadGrassland();
        LoadBuildingPen();
        LoadBuildingGreenhouse();
    }
    #region ----- BUILDING BARN -----
    public void Barn2()
    {
        builderUI.OnPanelConfirm(() => UpgradeBuildingBarn_1to2());
    }
    public void Barn3()
    {
        builderUI.OnPanelConfirm(() => UpgradeBuildingBarn_2to3());
    }
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
        if (currentlevelBarn == 2)
        {
            builderUI.UpdateButton_UpdateBarnLv2();
        }
        else if (currentlevelBarn == 3)
        {
            builderUI.UpdateButton_UpdateBarnLv3();
        }
    }
    public void UpgradeBuildingBarn_1to2()
    {       
        if (Gold.Instance.gold >= priceBuildingBarn[0] && LevelManager.Instance.currentLevel >= 10)
        {
            currentlevelBarn = 2;
            Gold.Instance.gold -= priceBuildingBarn[0];
            LoadBuildingBarn();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else if (Gold.Instance.gold < priceBuildingBarn[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 10)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    public void UpgradeBuildingBarn_2to3()
    {
        if (Gold.Instance.gold >= priceBuildingBarn[1] && currentlevelBarn == 2 && LevelManager.Instance.currentLevel >= 20)
        {
            currentlevelBarn = 3;
            Gold.Instance.gold -= priceBuildingBarn[1];
            LoadBuildingBarn();
            builderUI.UpdateButton_UpdateBarnLv3();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else if (currentlevelBarn == 1)
        {
            Notification.Instance.ShowNotification("Chưa mở khóa kho lương thực cấp 2!");
        }
        else if (Gold.Instance.gold < priceBuildingBarn[1])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 20)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
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
    public void Home2()
    {
        builderUI.OnPanelConfirm(() => UpdateBuildingHome_1to2());
    }
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
        if (currentlevelHome == 2)
        {
            builderUI.UpdateButton_UpdateHomeLv2();
        }
    }
    public void UpdateBuildingHome_1to2()
    {
        if (Gold.Instance.gold >= priceBuildingHome[0] && currentlevelHome == 1 && LevelManager.Instance.currentLevel >= 15)
        {
            currentlevelHome = 2;
            Gold.Instance.gold -= priceBuildingHome[0];
            LoadBuildingHome();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else if (Gold.Instance.gold < priceBuildingHome[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 15)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
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
    public void Farmland2()
    {
        builderUI.OnPanelConfirm(() => UnlockFarmland2());
    }
    public void Farmland3()
    {
        builderUI.OnPanelConfirm(() => UnlockFarmland3());
    }
    public void Grassland1()
    {
        builderUI.OnPanelConfirm(() => UnlockGrassland());
    }
    public void LoadFarmland()
    {
        if (isUnlockFarmland2)
        {
            Farmland[0].SetActive(true);
            blockFarmland[0].SetActive(false);
            builderUI.UpdateButton_UnlockFarmland2();
        }
        if (isUnlockFarmland3)
        {
            Farmland[1].SetActive(true);
            blockFarmland[1].SetActive(false);
            builderUI.UpdateButton_UnlockFarmland3();
        }
    }
    public void UnlockFarmland2()
    {
        if (Gold.Instance.gold >= priceFarmland[0] && LevelManager.Instance.currentLevel >= 13)
        {
            Gold.Instance.gold -= priceFarmland[0];
            isUnlockFarmland2 = true;
            LoadFarmland();          
            Notification.Instance.ShowNotification("Đã mở khóa thành công!");
        }
        else if (Gold.Instance.gold < priceFarmland[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 15)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    public void UnlockFarmland3()
    {
        if (Gold.Instance.gold >= priceFarmland[1] && isUnlockFarmland2 && LevelManager.Instance.currentLevel >= 21)
        {
            Gold.Instance.gold -= priceFarmland[1];
            isUnlockFarmland3 = true;
            LoadFarmland();
            Notification.Instance.ShowNotification("Đã mở khóa thành công!");
        }
        else if (!isUnlockFarmland2)
        {
            Notification.Instance.ShowNotification("Chưa mở khóa khu đất trồng 2!");
        }
        else if (Gold.Instance.gold < priceFarmland[1])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 21)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    public void LoadGrassland()
    {
        if (isUnlockGrassland)
        {
            Grassland.SetActive(true);
            blockGrassland.SetActive(false);
            builderUI.UpdateButton_UnlockGrassland();
        }
    }
    public void UnlockGrassland()
    {
        if (Gold.Instance.gold >= priceGrassland && LevelManager.Instance.currentLevel >= 6)
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
        else if (LevelManager.Instance.currentLevel < 6)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    #endregion

    #region ----- BUILDING PEN -----
    public void Pen1Lv1()
    {
        builderUI.OnPanelConfirm(() => UnlockBuildingPen1());
    }
    public void Pen1Lv2()
    {
        builderUI.OnPanelConfirm(() => UpdateBuildingPen1_1to2());
    }
    public void Pen2Lv1()
    {
        builderUI.OnPanelConfirm(() => UnlockBuildingPen2());
    }
    public void Pen2Lv2()
    {
        builderUI.OnPanelConfirm(() => UpdateBuildingPen2_1to2());
    }
    public void LoadBuildingPen()
    {
        HidePen();
        if (isUnlockPen1)
        {
            blockPen[0].SetActive(false);
            interaction[0].SetActive(true);
            builderUI.UpdateButton_UnlockPen1();
            if (currentlevelPen1 == 2)
            {
                builderUI.UpdateButton_UpdatePen1Lv2();
            }
        }
        if (isUnlockPen2)
        {
            blockPen[1].SetActive(false);
            interaction[1].SetActive(true);
            builderUI.UpdateButton_UnlockPen2();
            if (currentlevelPen2 == 2)
            {
                builderUI.UpdateButton_UpdatePen2Lv2();
            }
        }
        for (int i = 0; i < BuildingPen1.Length; i++)
        {
            if (currentlevelPen1 == i + 1 && isUnlockPen1)
            {
                BuildingPen1[i].SetActive(true);
            }
            if (currentlevelPen2 == i + 1 && isUnlockPen2)
            {
                BuildingPen2[i].SetActive(true);
            }
        }
    }
    public void UnlockBuildingPen1()
    {
        if (Gold.Instance.gold >= priceBuildingPen1[0] && LevelManager.Instance.currentLevel >= 6)
        {
            Gold.Instance.gold -= priceBuildingPen1[0];
            currentlevelPen1 = 1;
            isUnlockPen1 = true;
            LoadBuildingPen();
            Notification.Instance.ShowNotification("Đã mở khóa thành công!");
        }
        else if (Gold.Instance.gold < priceBuildingPen1[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 6)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    public void UnlockBuildingPen2()
    {
        if (Gold.Instance.gold >= priceBuildingPen2[0] && LevelManager.Instance.currentLevel >= 11)
        { 
            Gold.Instance.gold -= priceBuildingPen2[0];
            currentlevelPen2 = 1;
            isUnlockPen2 = true;
            LoadBuildingPen();
            builderUI.UpdateButton_UnlockPen2();
            Notification.Instance.ShowNotification("Đã mở khóa thành công!");
        }
        else if (Gold.Instance.gold < priceBuildingPen2[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 11)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    public void UpdateBuildingPen1_1to2()
    {
        if (Gold.Instance.gold >= priceBuildingPen1[1] && LevelManager.Instance.currentLevel >= 18 && isUnlockPen1)
        {
            currentlevelPen1 = 2;
            Gold.Instance.gold -= priceBuildingPen1[1];
            LoadBuildingPen();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else if (!isUnlockPen1)
        {
            Notification.Instance.ShowNotification("Chưa mở khóa chuồng nuôi 1!");
        }
        else if (Gold.Instance.gold < priceBuildingPen1[1])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 18)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    public void UpdateBuildingPen2_1to2()
    {
        if (Gold.Instance.gold >= priceBuildingPen2[1] && LevelManager.Instance.currentLevel >= 25 && isUnlockPen2)
        {
            currentlevelPen2 = 2;
            Gold.Instance.gold -= priceBuildingPen2[1];
            LoadBuildingPen();
            Notification.Instance.ShowNotification("Đã nâng cấp thành công!");
        }
        else if (!isUnlockPen2)
        {
            Notification.Instance.ShowNotification("Chưa mở khóa chuồng nuôi 2!");
        }
        else if (Gold.Instance.gold < priceBuildingPen1[1])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 25)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    public void HidePen()
    {
        for (int i = 0; i < BuildingPen1.Length; i++)
        {
            BuildingPen1[i].SetActive(false);
            BuildingPen2[i].SetActive(false);
        }
    }
    #endregion

    #region ----- BUILDING GREENHOUSE -----
    public void Greenhouse1()
    {
        builderUI.OnPanelConfirm(() => UnlockBuildingGreenhouse1());
    }

    public void Greenhouse2()
    {
        builderUI.OnPanelConfirm(() => UnlockBuildingGreenhouse2());
    }
    public void LoadBuildingGreenhouse()
    {
        if (isUnlockGreenhouse1)
        {
            blockGreenhouse[0].SetActive(false);
            BuildingGreenhouse[0].SetActive(true);
            builderUI.UpdateButton_UnlockGreenhouse1();
        }
        if (isUnlockGreenhouse2)
        {
            blockGreenhouse[1].SetActive(false);
            BuildingGreenhouse[1].SetActive(true);
            builderUI.UpdateButton_UnlockGreenhouse2();
        }
    }
    public void UnlockBuildingGreenhouse1()
    {
        if (Gold.Instance.gold >= priceBuildingGreenhouse[0] && LevelManager.Instance.currentLevel >= 8)
        {
            Gold.Instance.gold -= priceBuildingGreenhouse[0];
            isUnlockGreenhouse1 = true;
            LoadBuildingGreenhouse();
            Notification.Instance.ShowNotification("Đã mở khóa thành công!");
        }
        else if (Gold.Instance.gold < priceBuildingGreenhouse[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 8)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    public void UnlockBuildingGreenhouse2()
    {
        if (Gold.Instance.gold >= priceBuildingGreenhouse[1] && LevelManager.Instance.currentLevel >= 17)
        {
            Gold.Instance.gold -= priceBuildingGreenhouse[1];
            isUnlockGreenhouse2 = true;
            LoadBuildingGreenhouse();
            Notification.Instance.ShowNotification("Đã mở khóa thành công!");
        }
        else if (Gold.Instance.gold < priceBuildingGreenhouse[0])
        {
            Notification.Instance.ShowNotification("Không đủ vàng!");
        }
        else if (LevelManager.Instance.currentLevel < 17)
        {
            Notification.Instance.ShowNotification("Chưa đạt cấp độ yêu cầu!");
        }
    }
    #endregion
}

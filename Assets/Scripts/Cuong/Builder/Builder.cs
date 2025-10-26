using System;
using System.Xml.Serialization;
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

    public bool[] isBuilding;
    public int[] constructionDays;
    public int[] dayCounter;

    public void SetIsBuilding(int index)
    {
        isBuilding[index] = true;
    }
    public void CheckCanBuild()
    {
        for (int i = 0; i < isBuilding.Length; i++)
        {
            if (isBuilding[i])
            {
                if(dayCounter[i] < constructionDays[i])
                {
                    dayCounter[i]++;
                }
                else if(dayCounter[i] == constructionDays[i])
                {
                    isBuilding[i] = false;
                    ConstructionCompleted(i);
                }
            }
        }
    }
    public void ConstructionCompleted(int index)
    {
        switch (index)
        {
            case 0:
                UpgradeBuildingBarn_1to2();
                break;
            case 1:
                UpgradeBuildingBarn_2to3();
                break;
            case 2:
                UpdateBuildingHome_1to2();
                break;
            case 3:
                UnlockFarmland2();
                break;
            case 4:
                UnlockFarmland3();
                break;
            case 5:
                UnlockGrassland();
                break;
            case 6:
                UnlockBuildingPen1();
                break;
            case 7:
                UpdateBuildingPen1_1to2();
                break;
            case 8:
                UnlockBuildingPen2();
                break;
            case 9:
                UpdateBuildingPen2_1to2();
                break;
            case 10:
                UnlockBuildingGreenhouse1();
                break;
            case 11:
                UnlockBuildingGreenhouse2();
                break;
        }
    }
    void Start()
    {
        //LoadAllBuilding();
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
        if (Gold.Instance.gold >= priceBuildingBarn[0] && LevelManager.Instance.currentLevel >= 10)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(0);
                builderUI.UpdateButton_UpdateBarnLv2();
            });
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
    public void Barn3()
    {
        if (Gold.Instance.gold >= priceBuildingBarn[1] && currentlevelBarn == 2 && LevelManager.Instance.currentLevel >= 20)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(1);
                builderUI.UpdateButton_UpdateBarnLv3();
            });
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
            builderUI.UpdateButton_UpdateBarnLv2();
            builderUI.UpdateButton_UpdateBarnLv3();
        }
    }
    public void UpgradeBuildingBarn_1to2()
    {
        currentlevelBarn = 2;
        Gold.Instance.gold -= priceBuildingBarn[0];
        LoadBuildingBarn();
    }
    public void UpgradeBuildingBarn_2to3()
    {
        currentlevelBarn = 3;
        Gold.Instance.gold -= priceBuildingBarn[1];
        LoadBuildingBarn();
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
        if (Gold.Instance.gold >= priceBuildingHome[0] && currentlevelHome == 1 && LevelManager.Instance.currentLevel >= 15)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(2);
                builderUI.UpdateButton_UpdateHomeLv2();
            });
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
        currentlevelHome = 2;
        Gold.Instance.gold -= priceBuildingHome[0];
        LoadBuildingHome();
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

        if (Gold.Instance.gold >= priceFarmland[0] && LevelManager.Instance.currentLevel >= 13)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(3);
                builderUI.UpdateButton_UnlockFarmland2();
            });
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
    public void Farmland3()
    {
        if (Gold.Instance.gold >= priceFarmland[1] && isUnlockFarmland2 && LevelManager.Instance.currentLevel >= 21)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(4);
                builderUI.UpdateButton_UnlockFarmland3();
            });
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
    public void Grassland1()
    {
        if (Gold.Instance.gold >= priceGrassland && LevelManager.Instance.currentLevel >= 6)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(5);
                builderUI.UpdateButton_UnlockGrassland();
            });
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
        Gold.Instance.gold -= priceFarmland[0];
        isUnlockFarmland2 = true;
        LoadFarmland();
    }
    public void UnlockFarmland3()
    {
        Gold.Instance.gold -= priceFarmland[1];
        isUnlockFarmland3 = true;
        LoadFarmland();
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
        Gold.Instance.gold -= priceGrassland;
        isUnlockGrassland = true;
        LoadGrassland();
    }
    #endregion

    #region ----- BUILDING PEN -----
    public void Pen1Lv1()
    {
        if (Gold.Instance.gold >= priceBuildingPen1[0] && LevelManager.Instance.currentLevel >= 6)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(6);
                builderUI.UpdateButton_UnlockPen1();
            });
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
    public void Pen1Lv2()
    {
        if (Gold.Instance.gold >= priceBuildingPen1[1] && LevelManager.Instance.currentLevel >= 18 && isUnlockPen1)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(7);
                builderUI.UpdateButton_UpdatePen1Lv2();
            });
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
    public void Pen2Lv1()
    {
        if (Gold.Instance.gold >= priceBuildingPen2[0] && LevelManager.Instance.currentLevel >= 11)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(8);
                builderUI.UpdateButton_UnlockPen2();
            });
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
    public void Pen2Lv2()
    {
        if (Gold.Instance.gold >= priceBuildingPen2[1] && LevelManager.Instance.currentLevel >= 25 && isUnlockPen2)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(9);
                builderUI.UpdateButton_UpdatePen2Lv2();
            });
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
        Gold.Instance.gold -= priceBuildingPen1[0];
        currentlevelPen1 = 1;
        isUnlockPen1 = true;
        LoadBuildingPen();
    }
    public void UnlockBuildingPen2()
    {
        Gold.Instance.gold -= priceBuildingPen2[0];
        currentlevelPen2 = 1;
        isUnlockPen2 = true;
        LoadBuildingPen();
    }
    public void UpdateBuildingPen1_1to2()
    {
        currentlevelPen1 = 2;
        Gold.Instance.gold -= priceBuildingPen1[1];
        LoadBuildingPen();
    }
    public void UpdateBuildingPen2_1to2()
    {
        currentlevelPen2 = 2;
        Gold.Instance.gold -= priceBuildingPen2[1];
        LoadBuildingPen();
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
        if (Gold.Instance.gold >= priceBuildingGreenhouse[0] && LevelManager.Instance.currentLevel >= 8)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(10);
                builderUI.UpdateButton_UnlockGreenhouse1();
            });
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

    public void Greenhouse2()
    {
        if (Gold.Instance.gold >= priceBuildingGreenhouse[1] && LevelManager.Instance.currentLevel >= 17)
        {
            builderUI.OnPanelConfirm(() =>
            {
                SetIsBuilding(11);
                builderUI.UpdateButton_UnlockGreenhouse2();
            });
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
        Gold.Instance.gold -= priceBuildingGreenhouse[0];
        isUnlockGreenhouse1 = true;
        LoadBuildingGreenhouse();
    }
    public void UnlockBuildingGreenhouse2()
    {
        Gold.Instance.gold -= priceBuildingGreenhouse[1];
        isUnlockGreenhouse2 = true;
        LoadBuildingGreenhouse();
    }
    #endregion
}

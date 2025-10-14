using UnityEngine;

public class Builder : MonoBehaviour
{
    public int currentlevelBarn = 1;
    public int currentlevelHome;

    private bool isCropland2Unlocked;
    private bool isCropland3Unlocked;
    private bool isGrasslandUnlocked;
    private bool isPen1Unlocked;
    private bool isPen2Unlocked;

    public GameObject DebrisCropland2;
    public GameObject DebrisCropland3;
    public GameObject DebrisGrassland;
    public GameObject DebrisPen1;
    public GameObject DebrisPen2;
    public GameObject DebrisGreenhouse1;
    public GameObject DebrisGreenhouse2;
    public GameObject[] BuildingBarn;
    public GameObject[] BuildingHome;
    public GameObject[] BuildingPen1;
    public GameObject[] BuildingPen2;

    public int[] priceBuilding;
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
        if (Gold.Instance.gold >= priceBuilding[1])
        {
            currentlevelBarn++;
            LoadBuildingBarn();
            Notification.Instance.ShowNotification("Ð? nâng c?p thành công!");
        }
    }
    public void HideAllBarn()
    {
        for (int i = 0; i < BuildingBarn.Length; i++)
        {
            BuildingBarn[0].SetActive(false);
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

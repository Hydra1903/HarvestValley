using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    public static Achievements Instance;
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public int[] Gold;
    public int[] Xp;

    public int plantedSeedsCount;
    public int harvestedCropsCount;
    public int typesOfCropsPlantedCount;
    public int timesWateredCount;
    public int greenhouseCropsHarvestedCount;
    public int animalProductsCollectedCount;
    public int farmProductsSoldCount;
    public int perennialHarvestsCount;
    public int buildingsUpgradedOrUnlockedCount;
    public int staminaUsedCount;
    public int totalMoneyEarnedCount;

    public bool[] isAchivementComplete = new bool[36];
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void AddPlantedSeeds(int count)
    {
        plantedSeedsCount += count;
        if (plantedSeedsCount == 1 && !isAchivementComplete[0])
        {
            isAchivementComplete[0] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Gieo hạt đầu tiên");
        }
    }
    public void AddHarvestedCrop(int count)
    {
        harvestedCropsCount += count;
        if (harvestedCropsCount == 100 && !isAchivementComplete[1])
        {
            isAchivementComplete[1] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Bội thu I");
        }
        else if (harvestedCropsCount == 1000 && !isAchivementComplete[2])
        {
            isAchivementComplete[2] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Bội thu II");
        }
        else if (harvestedCropsCount == 10000 && !isAchivementComplete[3])
        {
            isAchivementComplete[3] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Bội thu III");
        }
    }
    public void AddTypesOfCropsPlanted(int count)
    {
        typesOfCropsPlantedCount += count;
        if (typesOfCropsPlantedCount == 5 && !isAchivementComplete[4])
        {
            isAchivementComplete[4] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Nhà thực vật I");
        }
        else if (typesOfCropsPlantedCount == 10 && !isAchivementComplete[5])
        {
            isAchivementComplete[5] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Nhà thực vật II");
        }
        else if (typesOfCropsPlantedCount == 15 && !isAchivementComplete[6])
        {
            isAchivementComplete[6] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Nhà thực vật III");
        }
    }
    public void AddTimesWatered(int count)
    {
        timesWateredCount += count;
        if (timesWateredCount == 10 && !isAchivementComplete[7])
        {
            isAchivementComplete[7] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Chăm chỉ I");
        }
        else if (timesWateredCount == 100 && !isAchivementComplete[8])
        {
            isAchivementComplete[8] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Chăm chỉ II");
        }
        else if (timesWateredCount == 500 && !isAchivementComplete[9])
        {
            isAchivementComplete[9] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Chăm chỉ III");
        }
    }
    public void AddGreenhouseCropsHarvested(int count)
    {
        greenhouseCropsHarvestedCount += count;
        if (greenhouseCropsHarvestedCount == 100 && !isAchivementComplete[10])
        {
            isAchivementComplete[10] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Chuyên gia thủy canh I");
        }
        else if (greenhouseCropsHarvestedCount == 1000 && !isAchivementComplete[11])
        {
            isAchivementComplete[11] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Chuyên gia thủy canh II");
        }
    }
    public void AddAnimalProductsCollected(int count)
    {
        animalProductsCollectedCount += count;
        if (animalProductsCollectedCount == 10 && !isAchivementComplete[12])
        {
            isAchivementComplete[12] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Chăn nuôi I");
        }
        else if (animalProductsCollectedCount == 100 && !isAchivementComplete[13])
        {
            isAchivementComplete[13] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Chăn nuôi II");
        }
        else if (animalProductsCollectedCount == 1000 && !isAchivementComplete[14])
        {
            isAchivementComplete[14] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Chăn nuôi III");
        }
    }
    public void AddFarmProductsSold(int count)
    {
        farmProductsSoldCount += count;
        if (farmProductsSoldCount == 1 && !isAchivementComplete[15])
        {
            isAchivementComplete[15] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Phiên chợ đầu tiên");
        }
        else if (farmProductsSoldCount == 100 && !isAchivementComplete[16])
        {
            isAchivementComplete[16] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Tiền về I");
        }
        else if (farmProductsSoldCount == 1000 && !isAchivementComplete[17])
        {
            isAchivementComplete[17] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Tiền về II");
        }
        else if (farmProductsSoldCount == 10000 && !isAchivementComplete[18])
        {
            isAchivementComplete[18] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Tiền về III");
        }
    }
    public void AddPerennialHarvests(int count)
    {
        perennialHarvestsCount += count;
        if (perennialHarvestsCount == 10 && !isAchivementComplete[19])
        {
            isAchivementComplete[19] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Quả ngọt I");
        }
        else if (perennialHarvestsCount == 100 && !isAchivementComplete[20])
        {
            isAchivementComplete[20] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Quả ngọt II");
        }
        else if (perennialHarvestsCount == 1000 && !isAchivementComplete[21])
        {
            isAchivementComplete[21] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Quả ngọt III");
        }
    }
    public void AddBuildingsUpgradedOrUnlocked(int count)
    {
        buildingsUpgradedOrUnlockedCount += count;
        if (buildingsUpgradedOrUnlockedCount == 1 && !isAchivementComplete[22])
        {
            isAchivementComplete[22] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Phát triển I");
        }
        else if (buildingsUpgradedOrUnlockedCount == 3 && !isAchivementComplete[23])
        {
            isAchivementComplete[23] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Phát triển II");
        }
        else if (buildingsUpgradedOrUnlockedCount == 5 && !isAchivementComplete[24])
        {
            isAchivementComplete[24] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Phát triển III");
        }
    }
    public void CompleteAchivement_NewYear()
    {
        if (!isAchivementComplete[25])
        {
            isAchivementComplete[25] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Năm mới");
        }
    }
    public void CompleteAchivement_Automation()
    {
        if (!isAchivementComplete[26])
        {
            isAchivementComplete[26] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Tự động hóa");
        }
    }
    public void CompleteAchivement_IntoTheGreenhouse()
    {
        if (!isAchivementComplete[27])
        {
            isAchivementComplete[27] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Bước chân vào nhà kính");
        }
    }
    public void CompleteAchivement_Veteran()
    {
        if (LevelManager.Instance.currentLevel == 15 && !isAchivementComplete[28])
        {
            isAchivementComplete[28] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Kì cựu I");
        }
        else if (LevelManager.Instance.currentLevel == 30 && !isAchivementComplete[29])
        {
            isAchivementComplete[29] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Kì cựu II");
        }
    }
    public void AddStaminaUsedCount(int count)
    {
        staminaUsedCount += count;
        if (staminaUsedCount == 1000 && !isAchivementComplete[30])
        {
            isAchivementComplete[30] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Thể dục thể thao I");
        }
        else if (staminaUsedCount == 5000 && !isAchivementComplete[31])
        {
            isAchivementComplete[31] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Thể dục thể thao II");
        }
        else if (staminaUsedCount == 10000 && !isAchivementComplete[32])
        {
            isAchivementComplete[32] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Thể dục thể thao III");
        }
    }
    public void AddTotalMoneyEarnedCount(int count)
    {
        totalMoneyEarnedCount += count;
        if (totalMoneyEarnedCount == 10000 && !isAchivementComplete[33])
        {
            isAchivementComplete[33] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Đại gia I");
        }
        else if (totalMoneyEarnedCount == 100000 && !isAchivementComplete[34])
        {
            isAchivementComplete[34] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Đại gia II");
        }
        else if (totalMoneyEarnedCount == 500000 && !isAchivementComplete[35])
        {
            isAchivementComplete[35] = true;
            CompleteAchivements.Instance.ShowCompleteAchivements("Đại gia III");
        }
    }

}

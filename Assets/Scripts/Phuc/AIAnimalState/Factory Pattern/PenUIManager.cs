//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class PenUIManager : MonoBehaviour
//{
//    public Transform animalListParent; // Nội dung scroll view
//    public GameObject animalSlotPrefab; // Prefab cho mỗi con vật

//    private AnimalPen currentPen;

//    public void ShowPen(AnimalPen pen)
//    {
//        currentPen = pen;
//        UpdateAnimalList();
//    }

//    public void UpdateAnimalList()
//    {
//        // Xoá hết slot cũ
//        foreach (Transform child in animalListParent)
//        {
//            Destroy(child.gameObject);
//        }

//        // Nếu không có động vật
//        if (currentPen.animalsInPen.Count == 0)
//        {
//            // Có thể hiển thị "Chưa có động vật nào"
//            return;
//        }

//        // Tạo slot mới
//        foreach (Animal animal in currentPen.animalsInPen)
//        {
//            GameObject slot = Instantiate(animalSlotPrefab, animalListParent);
//            slot.transform.Find("NameText").GetComponent<TMP_Text>().text = animal.animalName;
//            slot.transform.Find("Icon").GetComponent<Image>().sprite = animal.icon;

//            Button sellBtn = slot.transform.Find("SellButton").GetComponent<Button>();
//            sellBtn.onClick.AddListener(() => SellAnimal(animal));
//        }
//    }

//    private void SellAnimal(Animal animal)
//    {
//        currentPen.animalsInPen.Remove(animal);
//        Destroy(animal.gameObject); // Xoá khỏi scene
//        // Cộng tiền cho người chơi
//        Inventory.Instance.AddMoney(animal.sellPrice);
//        UpdateAnimalList();
//    }
//}

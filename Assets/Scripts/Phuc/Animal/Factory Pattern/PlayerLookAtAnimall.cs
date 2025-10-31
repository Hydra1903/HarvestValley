//using UnityEngine;

//public class PlayerLookAtAnimal : MonoBehaviour
//{
//    [Header("Raycast Settings")]
//    public float lookRange = 5f;                     // Khoảng cách có thể nhìn thấy động vật
//    public LayerMask animalLayer;                    // Layer chứa động vật
//    public Camera playerCamera;                      // Camera của người chơi

//    [Header("UI References")]
//    public InfoPanelUI infoPanel;                    // Bảng thông tin hiển thị động vật

//    private GameObject currentTarget;

//    void Update()
//    {
//        CheckLookAtAnimal();
//    }

//    private void CheckLookAtAnimal()
//    {
//        if (playerCamera == null) return;

//        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit, lookRange, animalLayer))
//        {
//            GameObject hitObj = hit.collider.gameObject;

//            if (hitObj != currentTarget)
//            {
//                currentTarget = hitObj;
//                ShowAnimalInfo(currentTarget);
//            }
//        }
//        else
//        {
//            if (currentTarget != null)
//            {
//                HideAnimalInfo();
//                currentTarget = null;
//            }
//        }
//    }

//    private void ShowAnimalInfo(GameObject animal)
//    {
//        if (animal == null || infoPanel == null) return;

//        AnimalInfo info = animal.GetComponent<AnimalInfo>();
//        AnimalFedding feeding = animal.GetComponent<AnimalFedding>();

//        if (info != null && feeding != null)
//        {
//            infoPanel.UpdateAnimalInfo(
//                info.data.animalName,
//                feeding.GetStateText(),
//                info.data.productIcon,
//                feeding.GetDaysFed(),
//                feeding.CanHarvest()
//            );
//            infoPanel.Show();
//        }
//    }

//    private void HideAnimalInfo()
//    {
//        if (infoPanel != null)
//            infoPanel.Hide();
//    }
//}

using UnityEngine;

/// <summary>
/// Hướng dẫn setup hệ thống trồng cây tối ưu
/// </summary>
public class OptimizedPlantingGuide : MonoBehaviour
{
    [Header("Hướng dẫn Setup Tối Ưu")]
    [TextArea(15, 25)]
    public string setupInstructions = @"
=== HỆ THỐNG TRỒNG CÂY TỐI ƯU ===

ĐIỀU KHIỂN:
- Phím 1: Chọn cuốc (Hoe) - đào luống 5x5
- Phím 2: Chọn xẻng (Shovel) - đào hố 3x3  
- Phím 3: Chọn hạt giống (Seed) - trồng cây

KHI ĐANG DÙNG HẠT GIỐNG:
- Phím Q: Chọn cà rốt (1x1) - trồng trên luống
- Phím W: Chọn cà chua (2x2) - trồng trên luống
- Phím E: Chọn táo (3x3) - trồng trên hố

SETUP TRONG UNITY (TỐI ƯU):

1. FARMGRID COMPONENT:
   - Gán các prefab đào luống: dugSoilPrefab, holePrefab
   - Gán ghost cho đào luống: ghostPlotPrefab, ghostHolePrefab
   - PlantGhostManager sẽ được tạo tự động

2. PLANT PREFABS ARRAY:
   - Chỉ cần gán plantPrefab cho mỗi loại cây
   - KHÔNG cần tạo ghostPrefab riêng
   - Ví dụ:
     * Element 0: PlantType = Carrot, plantPrefab = CarrotPrefab
     * Element 1: PlantType = Tomato, plantPrefab = TomatoPrefab
     * Element 2: PlantType = Apple, plantPrefab = ApplePrefab

3. GHOST MATERIAL:
   - Tạo 1 Material duy nhất với:
     * Rendering Mode = Transparent
     * Albedo Alpha = 0.3-0.5 (trong suốt)
     * Màu nhạt (ví dụ: trắng hoặc xanh nhạt)
   - Gán vào PlantGhostManager.ghostMaterial

4. TỐI ƯU:
   - Chỉ cần 1 ghost material cho TẤT CẢ loại cây
   - Ghost được tạo động từ prefab gốc
   - Không cần tạo 40-50 ghost prefab riêng
   - Tiết kiệm memory và dễ quản lý

CÁCH HOẠT ĐỘNG:
1. Khi hover chuột, PlantGhostManager tạo ghost từ prefab gốc
2. Áp dụng ghost material lên tất cả renderer
3. Vô hiệu hóa collider, rigidbody, script không cần thiết
4. Hiển thị ghost tại vị trí chuột
5. Khi thay đổi loại cây, ghost được tạo lại tự động

LỢI ÍCH:
- Chỉ 1 material ghost cho tất cả cây
- Không giới hạn số lượng loại cây
- Dễ thêm cây mới (chỉ cần thêm vào plantPrefabs array)
- Tiết kiệm memory và performance
- Tự động áp dụng ghost cho bất kỳ prefab nào
";

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            GUI.Box(new Rect(10, 10, 500, 400), setupInstructions);
        }
    }
}
using UnityEngine;

/// <summary>
/// Hướng dẫn sử dụng hệ thống trồng cây
/// Đặt script này vào một GameObject trong scene để hiển thị hướng dẫn
/// </summary>
public class PlantingSystemGuide : MonoBehaviour
{
    [Header("Hướng dẫn sử dụng")]
    [TextArea(10, 20)]
    public string instructions = @"
=== HỆ THỐNG TRỒNG CÂY ===

ĐIỀU KHIỂN:
- Phím 1: Chọn cuốc (Hoe) - đào luống 5x5
- Phím 2: Chọn xẻng (Shovel) - đào hố 3x3  
- Phím 3: Chọn hạt giống (Seed) - trồng cây

KHI ĐANG DÙNG HẠT GIỐNG:
- Phím Q: Chọn cà rốt (1x1) - trồng trên luống
- Phím W: Chọn cà chua (2x2) - trồng trên luống
- Phím E: Chọn táo (3x3) - trồng trên hố

QUY TẮC TRỒNG CÂY:
- Cây 1x1, 2x2: chỉ trồng được trên luống (đã đào)
- Cây 3x3: chỉ trồng được trên hố (đã đào)
- Không thể trồng cây trên đất thường hoặc nơi đã có cây

CÁCH SỬ DỤNG:
1. Đào luống/hố trước (phím 1 hoặc 2)
2. Chuyển sang mode trồng cây (phím 3)
3. Chọn loại cây (Q/W/E)
4. Di chuyển chuột để xem ghost preview
5. Click chuột trái để trồng cây

SETUP TRONG UNITY:
- Gán các ghost prefab vào FarmGrid:
  * ghostPlant1x1Prefab
  * ghostPlant2x2Prefab  
  * ghostPlant3x3Prefab
- Thiết lập mảng plantPrefabs với:
  * PlantType tương ứng
  * Plant prefab
  * Ghost prefab
";

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            GUI.Box(new Rect(10, 10, 400, 300), instructions);
        }
    }
}
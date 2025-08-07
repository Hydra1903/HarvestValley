using UnityEngine;

/// <summary>
/// Hướng dẫn setup và sử dụng hệ thống PlantData
/// </summary>
public class PlantDataSystemGuide : MonoBehaviour
{
    [Header("Hướng dẫn Setup Hệ Thống PlantData")]
    [TextArea(20, 30)]
    public string setupInstructions = @"
=== HỆ THỐNG PLANTDATA - SCRIPTABLEOBJECT ===

TÍNH NĂNG CHÍNH:
✅ Dữ liệu cây trồng lưu trữ dưới dạng ScriptableObject
✅ Có thể tạo và chỉnh sửa trong Unity Inspector
✅ Hệ thống database quản lý tất cả loại cây
✅ Ghost preview tối ưu với 1 material duy nhất
✅ Dễ dàng thêm cây mới mà không cần code

CÁCH SETUP:

1. TẠO PLANTDATA ASSETS:
   - Right-click trong Project → Create → Farm System → Plant Data
   - Đặt tên: CarrotData, TomatoData, AppleData, v.v.
   - Điền thông tin trong Inspector:
     * Plant Name: Tên hiển thị
     * Plant Type: Enum tương ứng
     * Size: Small(1x1), Medium(2x2), Large(3x3)
     * Prefab: GameObject của cây
     * Icon: Sprite icon (tùy chọn)
     * Growth Time: Thời gian phát triển (giây)
     * Seed Cost: Giá hạt giống
     * Harvest Value: Giá trị thu hoạch
     * Description: Mô tả

2. TẠO PLANT DATABASE:
   - Right-click → Create → Farm System → Plant Database
   - Kéo tất cả PlantData vào mảng 'All Plants'
   - Chọn Default Plant (cây mặc định)
   - Click 'Validate Database' để kiểm tra

3. SETUP FARMGRID:
   - Gán Plant Database vào FarmGrid.plantDatabase
   - Tạo Ghost Material:
     * Rendering Mode = Transparent
     * Albedo Alpha = 0.3-0.5
     * Màu nhạt (trắng/xanh nhạt)
   - Gán Ghost Material vào FarmGrid.ghostMaterial

4. TẠO GHOST MATERIAL:
   - Tạo Material mới
   - Shader: Standard hoặc URP/Lit
   - Rendering Mode: Transparent
   - Albedo: Màu trắng, Alpha = 0.4
   - Metallic: 0, Smoothness: 0.5

ĐIỀU KHIỂN:
- Phím 1: Cuốc (đào luống 5x5)
- Phím 2: Xẻng (đào hố 3x3)
- Phím 3: Hạt giống (trồng cây)
- Q/W/E: Chuyển loại cây khi dùng hạt giống

LỢI ÍCH:

1. DỮ LIỆU TÁCH BIỆT:
   - Dữ liệu cây không hard-code trong script
   - Có thể chỉnh sửa mà không cần compile
   - Designer có thể tự tạo cây mới

2. QUẢN LÝ DỄ DÀNG:
   - Tất cả cây trong 1 database
   - Validate tự động kiểm tra lỗi
   - Tìm kiếm theo type, size, requirements

3. MỞ RỘNG LINH HOẠT:
   - Thêm thuộc tính mới vào PlantData
   - Thêm logic kiểm tra trong CanPlantOn()
   - Hỗ trợ unlimited số lượng cây

4. PERFORMANCE TỐI ƯU:
   - 1 ghost material cho tất cả cây
   - Ghost tạo động từ prefab gốc
   - Không tốn memory cho ghost không dùng

VÍ DỤ TẠO CÂY MỚI:
1. Tạo PlantData asset mới
2. Thêm PlantType mới vào enum (nếu cần)
3. Thêm vào Plant Database
4. Thêm key binding trong HandleToolSwitching (nếu cần)
5. Hoàn thành!

TROUBLESHOOTING:
- Nếu ghost không hiện: Kiểm tra ghostMaterial
- Nếu không trồng được: Kiểm tra PlantData.CanPlantOn()
- Nếu thiếu cây: Kiểm tra Plant Database
- Nếu lỗi null: Validate Database và kiểm tra prefab

ADVANCED FEATURES:
- Growth system với UpdateGrowth()
- Harvest system với CanHarvest()
- Water/Fertilizer requirements
- Economic system với cost/value
- Multiple harvest từ 1 cây
";

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            GUI.Box(new Rect(10, 10, 600, 500), setupInstructions);
        }
    }
}
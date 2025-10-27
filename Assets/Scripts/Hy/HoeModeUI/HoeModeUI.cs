using System.Collections.Generic;
using UnityEngine;

public class HoeModeUI : MonoBehaviour
{
    // Tự động thu thập nếu để trống; hoặc kéo tay vào nếu muốn cố định
    [SerializeField] private List<SoilManager> soils = new List<SoilManager>();
    [SerializeField] private List<FarmInput> farmInputs = new List<FarmInput>();

    private void Awake()
    {
        // Nếu chưa gán tay, tự tìm tất cả trong scene (kể cả khi có 3 farm)
        if (soils == null || soils.Count == 0)
            soils = new List<SoilManager>(FindObjectsByType<SoilManager>(FindObjectsSortMode.None));
        if (farmInputs == null || farmInputs.Count == 0)
            farmInputs = new List<FarmInput>(FindObjectsByType<FarmInput>(FindObjectsSortMode.None));
    }

    private void Update()
    {
        // Đọc trạng thái nút/toggle hiện tại như bạn đang làm
        var hoe = ChangeMode.Instance.currentModeHoe;      // EModeHoe
        var hand = ChangeInteract.Instance.currentModeHand; // EModeHand

        // Phát lệnh cho TẤT CẢ SoilManager
        foreach (var s in soils)
        {
            if (!s) continue;
            switch (hoe)
            {
                case EModeHoe.Mode1: s.SetHoeMode(HoeMode.DigFurrow5x5); break;
                case EModeHoe.Mode2: s.SetHoeMode(HoeMode.DigHole3x3); break;
                case EModeHoe.Mode3: s.SetHoeMode(HoeMode.Flatten); break;
            }
        }

        // Phát lệnh cho TẤT CẢ FarmInput
        foreach (var fi in farmInputs)
        {
            if (!fi) continue;
            switch (hand)
            {
                case EModeHand.Mode1: fi.SetHandMode(HandMode.Harvest); break;
                case EModeHand.Mode2: fi.SetHandMode(HandMode.Remove); break;
            }
        }
    }
}

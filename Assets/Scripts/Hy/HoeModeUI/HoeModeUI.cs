using UnityEngine;
using UnityEngine.UI;

public class HoeModeUI : MonoBehaviour
{
    [SerializeField] private SoilManager soil;

    private void Awake()
    {
        if (!soil) soil = FindAnyObjectByType<SoilManager>();
    }

    private void Update()
    {
        if (ChangeMode.Instance.currentModeHoe == EModeHoe.Mode1) soil.SetHoeMode(HoeMode.DigFurrow5x5);
        if (ChangeMode.Instance.currentModeHoe == EModeHoe.Mode2) soil.SetHoeMode(HoeMode.DigHole3x3);
        if (ChangeMode.Instance.currentModeHoe == EModeHoe.Mode3) soil.SetHoeMode(HoeMode.Flatten);
    }
}

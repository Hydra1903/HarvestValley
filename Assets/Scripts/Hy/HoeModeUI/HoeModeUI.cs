using UnityEngine;
using UnityEngine.UI;

public class HoeModeUI : MonoBehaviour
{
    [SerializeField] private SoilManager soil;
    [SerializeField] private FarmInput farmInput;

    private void Awake()
    {
        if (!soil) soil = FindAnyObjectByType<SoilManager>();
    }

    private void Update()
    {
        if (ChangeMode.Instance.currentModeHoe == EModeHoe.Mode1) soil.SetHoeMode(HoeMode.DigFurrow5x5);
        if (ChangeMode.Instance.currentModeHoe == EModeHoe.Mode2) soil.SetHoeMode(HoeMode.DigHole3x3);
        if (ChangeMode.Instance.currentModeHoe == EModeHoe.Mode3) soil.SetHoeMode(HoeMode.Flatten);
        if (ChangeInteract.Instance.currentModeHand == EModeHand.Mode1)  farmInput.SetHandMode(HandMode.Harvest);
        if (ChangeInteract.Instance.currentModeHand == EModeHand.Mode2) farmInput.SetHandMode(HandMode.Remove);
    }

}

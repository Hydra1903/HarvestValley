using UnityEngine;
using UnityEngine.UI;

public class HoeModeUI : MonoBehaviour
{
    [SerializeField] private SoilManager soil;
    [SerializeField] private Button digFurrrowButton;
    [SerializeField] private Button digHoleButton;
    [SerializeField] private Button flattenButton;

    private void Awake()
    {
        if (!soil) soil = FindAnyObjectByType<SoilManager>();
    }

    public void OnClickDigFurrow5x5() => soil.SetHoeMode(HoeMode.DigFurrow5x5);
    public void OnClickDigHole3x3() => soil.SetHoeMode(HoeMode.DigHole3x3);
    public void OnClickFlatten() => soil.SetHoeMode(HoeMode.Flatten);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) soil.SetHoeMode(HoeMode.DigFurrow5x5);
        if (Input.GetKeyDown(KeyCode.Alpha2)) soil.SetHoeMode(HoeMode.DigHole3x3);
        if (Input.GetKeyDown(KeyCode.Alpha3)) soil.SetHoeMode(HoeMode.Flatten);
    }
}

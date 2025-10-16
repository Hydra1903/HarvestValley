using UnityEngine;

public class PlantClickable : MonoBehaviour
{
    public int centerX { get; private set; }
    public int centerY { get; private set; }

    public FarmManager ownerFarm { get; private set; }
    public void Init(FarmManager farm, int cx, int cy)
    {
        ownerFarm = farm;
        centerX = cx; centerY = cy;
    }
    void OnEnable()
    {
        var o = GetComponentInChildren<Outline>();
        if (o)
        {
            o.OutlineMode = Outline.Mode.OutlineAll;
            o.OutlineColor = Color.red;
            o.OutlineWidth = 7f;
            // ép reset material props một nhịp
            o.enabled = false;
            o.enabled = true;  // OnEnable() sẽ re-append materials
        }
    }
}

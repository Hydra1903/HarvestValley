using UnityEngine;

public class MinimapWorld : MonoBehaviour
{
    [SerializeField]
    private bool followObject = false;
    [SerializeField]
    private Sprite minimapIcon;
    public Sprite MinimapIcon => minimapIcon;

    private void Start()
    {
        MiniMapController.Instance.RegisterMinimapWorldObject(this, followObject);
    }

    private void OnDestroy()
    {
        if (MiniMapController.Instance != null)
            MiniMapController.Instance.RemoveMinimapWorldObject(this);
    }
}

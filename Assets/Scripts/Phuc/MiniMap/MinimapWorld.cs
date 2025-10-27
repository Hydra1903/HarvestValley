using UnityEngine;

public class MinimapWorld : MonoBehaviour
{
    [SerializeField]
    private bool followObject = false;
    [SerializeField]
    private Sprite minimapIcon;
    public Sprite MinimapIcon => minimapIcon;
    public bool isPlayer;

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

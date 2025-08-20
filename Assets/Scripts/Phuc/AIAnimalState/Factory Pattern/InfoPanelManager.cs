using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    public static InfoPanelManager Instance { get; private set; }
    public InfoPanelUI sharedInfoPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        Instance = this;
    }
}

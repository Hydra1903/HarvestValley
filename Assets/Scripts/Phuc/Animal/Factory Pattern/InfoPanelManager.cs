using System.Collections.Generic;
using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    public static InfoPanelManager instance { get; private set; }

    public Dictionary<int, InfoPanelUI> panels = new Dictionary<int, InfoPanelUI>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void RegisterPanel(int penId, InfoPanelUI panel)
    {
        if (!panels.ContainsKey(penId))
        {
            panels.Add(penId, panel);
        }
    }

    public void UnregisterPanel(int penId)
    {
        if (panels.ContainsKey(penId))
        {
            panels.Remove(penId);
        }
    }

    public InfoPanelUI GetPanel(int penId)
    {
        if (panels.ContainsKey(penId))
            return panels[penId];
        return null;
    }
}

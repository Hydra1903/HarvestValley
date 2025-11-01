using System.Collections.Generic;
using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    private Dictionary<int, InfoPanelUI> panels = new Dictionary<int, InfoPanelUI>();

    public void RegisterPanel(int penId, InfoPanelUI panel)
    {
        if (panel == null) return;
        if (!panels.ContainsKey(penId))
            panels.Add(penId, panel);
        else
            panels[penId] = panel;
    }
    public void UnregisterPanel(int penId)
    {
        if (panels.ContainsKey(penId))
            panels.Remove(penId);
    }

    public InfoPanelUI GetPanel(int penId)
    {
        if (panels.TryGetValue(penId, out var panel))
            return panel;
        return null;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    public static InfoPanelManager instance { get; private set; }

    // Qu?n l? nhi?u InfoPanel theo penId
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

    // Ðãng k? panel m?i (m?i pen khi spawn th? g?i)
    public void RegisterPanel(int penId, InfoPanelUI panel)
    {
        if (!panels.ContainsKey(penId))
        {
            panels.Add(penId, panel);
        }
    }

    // Xóa panel khi pen b? h?y
    public void UnregisterPanel(int penId)
    {
        if (panels.ContainsKey(penId))
        {
            panels.Remove(penId);
        }
    }

    // L?y panel theo penId
    public InfoPanelUI GetPanel(int penId)
    {
        if (panels.ContainsKey(penId))
            return panels[penId];
        return null;
    }

    //// Refresh panel c?a 1 chu?ng c? th?
    //public void RefreshPanel(int penId)
    //{
    //    if (panels.ContainsKey(penId))
    //    {
    //        panels[penId].RefreshUI();
    //    }
    //}
}

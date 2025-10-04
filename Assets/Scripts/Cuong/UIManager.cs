using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Dictionary<string, UIView> views = new Dictionary<string, UIView>();

    void Awake()
    {
        if (Instance == null) Instance = this;

        UIView[] allViews = Object.FindObjectsByType<UIView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var view in allViews)
        {
            if (!views.ContainsKey(view.viewName))
                views.Add(view.viewName, view);
        }
    }

    public void ShowUI(string name)
    {
        if (views.TryGetValue(name, out var view))
        {
            view.Show();
        }
        else
        {
            //Debug.Log("Khong");
        }
    }

    public void HideUI(string name)
    {
        if (views.TryGetValue(name, out var view))
        {
            view.Hide();
        }
    }
}

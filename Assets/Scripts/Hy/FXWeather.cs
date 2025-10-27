using UnityEngine;
using System.Collections.Generic;

public class FXWeather : MonoBehaviour
{
    [Header("Tên Layer cho từng loại FX")]
    public string clearLayer = "ClearFX";
    public string rainLayer = "RainFX";
    public string stormLayer = "StormFX";
    public string snowLayer = "SnowFX";

    private List<GameObject> clearObjs = new();
    private List<GameObject> rainObjs = new();
    private List<GameObject> stormObjs = new();
    private List<GameObject> snowObjs = new();

    void Awake()
    {
        var allParticles = FindObjectsByType<ParticleSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var ps in allParticles)
        {
            string lname = LayerMask.LayerToName(ps.gameObject.layer);

            if (lname == clearLayer) clearObjs.Add(ps.gameObject);
            else if (lname == rainLayer) rainObjs.Add(ps.gameObject);
            else if (lname == stormLayer) stormObjs.Add(ps.gameObject);
            else if (lname == snowLayer) snowObjs.Add(ps.gameObject);
        }

        SetAllActive(false);

        Debug.Log($"🌤 FXWeatherByLayer_SetActive: Found {clearObjs.Count} Clear, {rainObjs.Count} Rain, {stormObjs.Count} Storm, {snowObjs.Count} Snow FX objects.");
    }

    private void SetAllActive(bool active)
    {
        foreach (var obj in clearObjs) if (obj) obj.SetActive(active);
        foreach (var obj in rainObjs) if (obj) obj.SetActive(active);
        foreach (var obj in stormObjs) if (obj) obj.SetActive(active);
        foreach (var obj in snowObjs) if (obj) obj.SetActive(active);
    }

    public void ApplyFX(WeatherState state)
    {
        SetAllActive(false);

        switch (state)
        {
            case WeatherState.Clear:
                foreach (var obj in clearObjs) if (obj) obj.SetActive(true);
                break;
            case WeatherState.Rainy:
                foreach (var obj in rainObjs) if (obj) obj.SetActive(true);
                break;
            case WeatherState.Stormy:
                foreach (var obj in stormObjs) if (obj) obj.SetActive(true);
                break;
            case WeatherState.Snowy:
                foreach (var obj in snowObjs) if (obj) obj.SetActive(true);
                break;
        }

        Debug.Log($"🌦 FXWeatherByLayer_SetActive: bật GameObject FX cho thời tiết {state}");
    }
}

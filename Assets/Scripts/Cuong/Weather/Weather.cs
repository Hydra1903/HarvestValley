using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public enum WeatherState
{
    Clear,
    Rainy,
    Stormy,
    Snowy
}
public class Weather : MonoBehaviour
{
    public static Weather Instance;
    public WeatherState currentWeather;
    public List<WeatherSchedule> listWeather = new List<WeatherSchedule>();
    public List<WeatherSchedule> listWeatherOfDay = new List<WeatherSchedule>();
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        listWeather.Add(new WeatherSchedule (WeatherState.Clear,80,6));
    }
    public void RamdomWeatherOfDay()
    {

    }


}

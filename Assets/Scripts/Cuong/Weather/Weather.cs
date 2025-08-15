using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
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
    public List<WeatherSchedule> listWeatherOfMonth = new List<WeatherSchedule>();
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        listWeather.Add(new WeatherSchedule (WeatherState.Clear));
        listWeather.Add(new WeatherSchedule(WeatherState.Rainy));
        listWeather.Add(new WeatherSchedule(WeatherState.Stormy));
        listWeather.Add(new WeatherSchedule(WeatherState.Snowy));
        SetAppearanceRate(Season.Instance.currentSeason);
        SetListWeatherOfMonth();
    }
    public void SetAppearanceRate(SeasonState season)
    {
        switch(season)
        {
            case SeasonState.Spring:
                listWeather[0].appearanceRate = 90;
                listWeather[1].appearanceRate = 10;
                listWeather[2].appearanceRate = 0;
                listWeather[3].appearanceRate = 0;
                break;
            case SeasonState.Summer:
                listWeather[0].appearanceRate = 55;
                listWeather[1].appearanceRate = 35;
                listWeather[2].appearanceRate = 10;
                listWeather[3].appearanceRate = 0;
                break;
            case SeasonState.Fall:
                listWeather[0].appearanceRate = 70;
                listWeather[1].appearanceRate = 25;
                listWeather[2].appearanceRate = 5;
                listWeather[3].appearanceRate = 0;
                break;
            case SeasonState.Winter:
                listWeather[0].appearanceRate = 40;
                listWeather[1].appearanceRate = 0;
                listWeather[2].appearanceRate = 0;
                listWeather[3].appearanceRate = 60;
                break;
        }
    }

    public void SetListWeatherOfMonth()
    {
        listWeatherOfMonth.Clear();
        for (int i = 0 ; i < 30 ; i++)
        {
            listWeatherOfMonth.Add(RamdomWeatherOfDay());
        }
    }
    public WeatherSchedule RamdomWeatherOfDay()
    {
        int randomAppearance = Random.Range(1, 101);

        WeatherSchedule weather = new WeatherSchedule();

        if (randomAppearance <= listWeather[0].appearanceRate) 
        {
            weather.weather = WeatherState.Clear; weather.appearanceRate = listWeather[0].appearanceRate; weather.timeRate = 24;
        }
        else if (randomAppearance <= listWeather[0].appearanceRate + listWeather[1].appearanceRate) 
        {
            weather.weather = WeatherState.Rainy; weather.appearanceRate = listWeather[1].appearanceRate; weather.timeRate = Random.Range(3, 7);
            weather.randomWeatherStartTime = RandomWeatherStartTime(weather.timeRate);
            weather.randomWeatherEndTime = weather.randomWeatherStartTime + weather.timeRate;
        }
        else if (randomAppearance <= listWeather[0].appearanceRate + listWeather[1].appearanceRate + listWeather[2].appearanceRate) 
        {
            weather.weather = WeatherState.Stormy; weather.appearanceRate = listWeather[2].appearanceRate; weather.timeRate = Random.Range(6, 9);
            weather.randomWeatherStartTime = RandomWeatherStartTime(weather.timeRate);
            weather.randomWeatherEndTime = weather.randomWeatherStartTime + weather.timeRate;
        }
        else 
        {
            weather.weather = WeatherState.Snowy; weather.appearanceRate = listWeather[3].appearanceRate; weather.timeRate = Random.Range(7, 10);
            weather.randomWeatherStartTime = RandomWeatherStartTime(weather.timeRate);
            weather.randomWeatherEndTime = weather.randomWeatherStartTime + weather.timeRate;
        }
        return weather;
    }

    public void SetCurrentWeather()
    {
        if (listWeatherOfMonth[GameTime.Instance.day - 1].weather == WeatherState.Clear)
        {
            currentWeather = WeatherState.Clear;
        }
        else if (listWeatherOfMonth[GameTime.Instance.day - 1].weather == WeatherState.Rainy)
        {
            if (GameTime.Instance.hour >= listWeatherOfMonth[GameTime.Instance.day - 1].randomWeatherStartTime && GameTime.Instance.hour < listWeatherOfMonth[GameTime.Instance.day - 1].randomWeatherEndTime)
            {
                currentWeather = WeatherState.Rainy;
            }
            else
            {
                currentWeather = WeatherState.Clear;
            }
        }
        else if (listWeatherOfMonth[GameTime.Instance.day - 1].weather == WeatherState.Stormy)
        {
            if (GameTime.Instance.hour >= listWeatherOfMonth[GameTime.Instance.day - 1].randomWeatherStartTime && GameTime.Instance.hour < listWeatherOfMonth[GameTime.Instance.day - 1].randomWeatherEndTime)
            {
                currentWeather = WeatherState.Stormy;
            }
            else
            {
                currentWeather = WeatherState.Clear;
            }
        }
        else if (listWeatherOfMonth[GameTime.Instance.day - 1].weather == WeatherState.Snowy)
        {
            if (GameTime.Instance.hour >= listWeatherOfMonth[GameTime.Instance.day - 1].randomWeatherStartTime && GameTime.Instance.hour < listWeatherOfMonth[GameTime.Instance.day - 1].randomWeatherEndTime)
            {
                currentWeather = WeatherState.Snowy;
            }
            else
            {
                currentWeather = WeatherState.Clear;
            }
        }
    }

    public int RandomWeatherStartTime(int timeRate)
    {
        int random;
        while (true)
        {
            random = Random.Range(6, 25);
            if ((random + timeRate) <= 24)
            {
                break;
            }
        }
        return random;
    }
    public void ShowListWeather()
    {
        for (int i = 0; i < 30; i++)
        {
            Debug.Log(i + 1 + " | " + listWeatherOfMonth[i].weather + " | " + listWeatherOfMonth[i].timeRate + " | "+ listWeatherOfMonth[i].randomWeatherStartTime + " | " + listWeatherOfMonth[i].randomWeatherEndTime);         
        }
    }


}

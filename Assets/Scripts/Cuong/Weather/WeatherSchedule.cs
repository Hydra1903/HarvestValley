[System.Serializable]
public class WeatherSchedule
{
    public WeatherState weather;
    public int appearanceRate;
    public int timeRate;
    public int randomWeatherStartTime;
    public int randomWeatherEndTime;
    public WeatherSchedule() { }
    public WeatherSchedule(WeatherState weather)
    {
        this.weather = weather;
    }
}


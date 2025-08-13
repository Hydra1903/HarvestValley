[System.Serializable]
public class WeatherSchedule
{
    public WeatherState weather;
    public int appearanceRate;
    public int timeRate;
    public WeatherSchedule(WeatherState weather, int appearanceRate, int timeRate)
    {
        this.weather = weather;
        this.appearanceRate = appearanceRate;
        this.timeRate = timeRate;
    }
}


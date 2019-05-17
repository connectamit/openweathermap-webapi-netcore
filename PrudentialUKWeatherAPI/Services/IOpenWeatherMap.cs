using PrudentialUKWeatherAPI.Entities;
using System.Collections.Generic;

namespace PrudentialUKWeatherAPI.Services
{
    public interface IOpenWeatherMap
    {
        List<City> GetAllCities();
        string GetCityWeather(string city);
        string GetCityWeather(int cityId);
        string GetCityWeather(string lat, string longt);
        string GetCityWeatherByZipCode(string zipcode, string country);
        //Call current weather data for several cities
        //Cities within a rectangle zone
        string GetCitiesWeather(Coordinates coordinates);
        //Cities in cycle
        string GetCitiesWeather(Coordinates coordinates, int countCities);
    }
}

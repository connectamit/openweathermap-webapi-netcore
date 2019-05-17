using Microsoft.AspNetCore.Mvc;
using PrudentialUKWeatherAPI.Entities;
using PrudentialUKWeatherAPI.Services;

namespace PrudentialUKWeatherAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityWeatherController : ControllerBase
    {
        private IOpenWeatherMap _openWeatherMap;
        public CityWeatherController(IOpenWeatherMap openWeatherMap)
        {
            _openWeatherMap = openWeatherMap;
        }

        [HttpGet("getallcities")]
        public IActionResult GetAllCities()
        {
            var cities = _openWeatherMap.GetAllCities();
            return Ok(cities);
        }

        [HttpGet("getcityweatherbyname")]
        //[Route("getcityweather/{cityName:string}")]
        //[ActionName("LoadCustomerbyName")]
        public IActionResult GetCityWeather(string cityName)
        {
            var city = _openWeatherMap.GetCityWeather(cityName);
            return Ok(city);
        }

        [HttpGet("getcityweatherbyid")]
        public IActionResult GetCityWeather(int cityId)
        {
            var city = _openWeatherMap.GetCityWeather(cityId);
            return Ok(city);
        }

        [HttpGet("getcityweatherbycoord")]
        public IActionResult GetCityWeather(string lat, string longt)
        {
            var city = _openWeatherMap.GetCityWeather(lat, longt);
            return Ok(city);
        }

        [HttpGet("getcityweatherbyzipcode")]
        public IActionResult GetCityWeatherByZipCode(string zipcode, string country)
        {
            var city = _openWeatherMap.GetCityWeatherByZipCode(zipcode, country);
            return Ok(city);
        }

        [HttpPost("getcitiesweatherbycoord")]
        public IActionResult GetCities([FromBody]Coordinates coordinates)
        {
            var city = _openWeatherMap.GetCitiesWeather(coordinates);
            return Ok(city);
        }

        [HttpGet("getcitiesweatherbyrectzone")]
        public IActionResult GetCities([FromQuery]Coordinates coordinates, int countries)
        {
            var city = _openWeatherMap.GetCitiesWeather(coordinates, countries);
            return Ok(city);
        }
       
    }
}

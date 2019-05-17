using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PrudentialUKWeatherAPI.Controllers;
using PrudentialUKWeatherAPI.Entities;
using PrudentialUKWeatherAPI.Helpers;
using PrudentialUKWeatherAPI.Services;

namespace PrudentialUKWeatherAPI.Test
{
    [TestClass]
    public class PrudentialUKWeatherAPITest
    {

        private static List<City> AllCities { get; set; }
        private static string CityName { get; set; }

        CityWeatherController _controller;
        IOpenWeatherMap _service;

        public PrudentialUKWeatherAPITest()
        {
            _service = new OpenWeatherMapFake();
            _controller = new CityWeatherController(_service);
        }

        [TestMethod]
        public void GetAllcities()
        {
            var result = _controller.GetAllCities();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetCityByName()
        {
            var result = _controller.GetCityWeather("London");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetCityById()
        {
            var result = _controller.GetCityWeather(1275339);
            Assert.IsNotNull(result);
        }

    }

    public class OpenWeatherMapFake : IOpenWeatherMap
    {
        private readonly AppSettings _appSettings;
        private const string _cityName = "q";
        private const string _cityId = "id";
        private const string _equals = "=";
        private const string _get = "GET";
        private const string _post = "POST";
        private const string _put = "PUT";
        private const string _delete = "DELETE";
        private const string _contentType = "application/json";
        private const string _allcities = @"C:\openweathermap\AllCities\openweathermap.txt";
        private const string _citiesweather = @"C:\openweathermap\CitiesWeather\";
        private const string _savedFileExt = ".txt";
        private const string _invalidCity = "Sorry! weather of this city does not exist with us";
        private static string CityName { get; set; }
        private static List<City> AllCities { get; set; }
        public OpenWeatherMapFake()
        {
            AllCities = new List<City> { new City {Id= "2643741",Name= "London" }
            , new City { Id = "2988507", Name = "Paris" }
            ,new City {Id= "2643741",Name= "Dublin"}
              ,new City {Id= "4229546",Name= "Washington"}
                ,new City {Id= "5128581",Name= "New York"}
              ,new City {Id= "1273294",Name= "Delhi"}
                ,new City {Id= "1275339",Name= "Mumbai"}
                  ,new City {Id= "6539761",Name= "Rome"}
                    ,new City {Id= "2950159",Name= "Berlin"}
                      ,new City {Id= "292223",Name= "Dubai"}
            };
        }

        public List<City> GetAllCities()
        {
            List<City> cities = new List<City>();
            var contents = File.ReadAllText(_allcities).Split('\n');
            var csv = from line in contents
                      select line.Split(',').ToArray();
            foreach (var city in csv)
            {
                if (string.IsNullOrEmpty(city[0])) continue;
                cities.Add(new City { Id = city[0], Name = city[1].TrimEnd('\r', '\n') });
            }
            if (cities.Count > 0)
                AllCities = cities;
            return cities;
        }

        public string GetCitiesWeatherByCycle(Coordinates coordinates, int countCities)
        {
            throw new System.NotImplementedException();
        }

        public string GetCitiesWeatherByRectZone(Coordinates coordinates)
        {
            throw new System.NotImplementedException();
        }

        public string GetCityWeather(string city)
        {
            if (ValidCityNameId(city))
            {
                StringBuilder builder = new StringBuilder(_cityName);
                builder.Append(_equals);
                builder.Append(city);
                StreamReader reader = GetResponse(builder.ToString());
                string str = reader.ReadLine();
                CityName = JObject.Parse(str)["name"].ToString();
                SaveFileCityWeather(str);
                return str;
            }
            else
            {
                return _invalidCity;
            }
        }

        public string GetCityWeather(int cityId)
        {
            if (ValidCityNameId(cityId.ToString()))
            {
                StringBuilder builder = new StringBuilder(Convert.ToString(_cityId));
                builder.Append(_equals);
                builder.Append(Convert.ToString(cityId));
                StreamReader reader = GetResponse(builder.ToString());
                string str = reader.ReadLine();
                CityName = JObject.Parse(str)["name"].ToString();
                SaveFileCityWeather(str);
                return str;
            }
            else
            {
                return _invalidCity;
            }
        }

        public string GetCityWeather(string lat, string longt)
        {
            throw new System.NotImplementedException();
        }

        public string GetCityWeatherByZipCode(string zipcode)
        {
            throw new System.NotImplementedException();
        }

        private void SaveFileCityWeather(string contents)
        {
            StringBuilder builder = new StringBuilder(_citiesweather);
            builder.Append(CityName);
            builder.Append(DateTime.Today.ToString("dd-MM-yyyy"));
            builder.Append(_savedFileExt);
            TextWriter txt = new StreamWriter(builder.ToString());
            txt.Write(contents);
            txt.Close();
        }

        private bool ValidCityNameId(string CityNameId)
        {
            var city = AllCities.FirstOrDefault(x => x.Name.Trim().ToLower() == CityNameId.Trim().ToLower() || x.Id.Trim().ToLower() == CityNameId.Trim().ToLower());
            bool ifexist = city != null ? true : false;
            return ifexist;
        }

        private StreamReader GetResponse(string requestParams, string method = "GET")
        {
            StringBuilder builder = new StringBuilder("http://api.openweathermap.org/data/2.5/weather?");
            builder.Append(requestParams);
            builder.Append("&appid=aa69195559bd4f88d79f9aadeb77a8f6");
            WebRequest webRequest = WebRequest.Create(builder.ToString());
            webRequest.Method = method;
            webRequest.ContentType = _contentType;
            WebResponse myWebResponse = webRequest.GetResponse();
            StreamReader reader = new StreamReader(myWebResponse.GetResponseStream());
            return reader;
        }

        public string GetCityWeatherByZipCode(string zipcode, string country)
        {
            throw new NotImplementedException();
        }

        public string GetCitiesWeather(Coordinates coordinates)
        {
            throw new NotImplementedException();
        }

        public string GetCitiesWeather(Coordinates coordinates, int countCities)
        {
            throw new NotImplementedException();
        }
    }


}

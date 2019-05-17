using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PrudentialUKWeatherAPI.Entities;
using PrudentialUKWeatherAPI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PrudentialUKWeatherAPI.Services
{
    public class OpenWeatherMap : IOpenWeatherMap
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
        private static List<City> AllCities { get; set; }
        private static string CityName { get; set; }

        public OpenWeatherMap(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
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
            StringBuilder builder = new StringBuilder(_appSettings.Apiurl);
            builder.Append("lat");
            builder.Append(_equals);
            builder.Append(lat);
            builder.Append("&");
            builder.Append("lon");
            builder.Append(_equals);
            builder.Append(longt);
            builder.Append(_appSettings.Apiid);
            WebRequest webRequest = WebRequest.Create(builder.ToString());
            webRequest.Method = _get;
            webRequest.ContentType = _contentType;
            WebResponse myWebResponse = webRequest.GetResponse();
            StreamReader reader = new StreamReader(myWebResponse.GetResponseStream());
            string str = reader.ReadLine();
            CityName = JObject.Parse(str)["name"].ToString();
            SaveFileCityWeather(str);
            return str;

        }

        public string GetCityWeatherByZipCode(string zipcode, string country)
        {
            StringBuilder builder = new StringBuilder(_appSettings.Apiurl);
            builder.Append("zip");
            builder.Append(_equals);
            builder.Append(zipcode);
            if (!string.IsNullOrEmpty(country))
            {
                builder.Append(",");
                builder.Append(country);
               
            }
            builder.Append(_appSettings.Apiid);
            WebRequest webRequest = WebRequest.Create(builder.ToString());
            webRequest.Method = _get;
            webRequest.ContentType = _contentType;
            WebResponse myWebResponse = webRequest.GetResponse();
            StreamReader reader = new StreamReader(myWebResponse.GetResponseStream());
            string str = reader.ReadLine();
            CityName = JObject.Parse(str)["name"].ToString();
            SaveFileCityWeather(str);
            return str;
        }

        public string GetCitiesWeather(Coordinates coordinates)
        {
            StringBuilder builder = new StringBuilder("http://api.openweathermap.org/data/2.5/box/");
            builder.Append("city?bbox=");
            builder.Append(coordinates.LongitudesLeft.ToString());
            builder.Append(",");
            builder.Append(coordinates.LatitudesBottom.ToString());
            builder.Append(",");
            builder.Append(coordinates.LongitudesRight.ToString());
            builder.Append(",");
            builder.Append(coordinates.LatitudesTop.ToString());
            builder.Append(",");
            builder.Append(coordinates.Zoom.ToString());
            builder.Append(",");
            builder.Append(_appSettings.Apiid);
            WebRequest webRequest = WebRequest.Create(builder.ToString());
            webRequest.Method = _get;
            webRequest.ContentType = _contentType;
            WebResponse myWebResponse = webRequest.GetResponse();
            StreamReader reader = new StreamReader(myWebResponse.GetResponseStream());
            string str = reader.ReadLine();
            //CityName = JObject.Parse(str)["name"].ToString();
            //SaveFileCityWeather(str);
            return str;
        }

        public string GetCitiesWeather(Coordinates coordinates, int countCities)
        {
            StringBuilder builder = new StringBuilder("http://api.openweathermap.org/data/2.5/");
            builder.Append("find?");
            builder.Append("lat");
            builder.Append(_equals);
            builder.Append(coordinates.LatitudesTop.ToString());
            builder.Append("&");
            builder.Append("lon");
            builder.Append(_equals);
            builder.Append(coordinates.LongitudesLeft.ToString());
            builder.Append("&");
            builder.Append("cnt");
            builder.Append(_equals);
            builder.Append(countCities.ToString());
            builder.Append(_appSettings.Apiid);
            WebRequest webRequest = WebRequest.Create(builder.ToString());
            webRequest.Method = _get;
            webRequest.ContentType = _contentType;
            WebResponse myWebResponse = webRequest.GetResponse();
            StreamReader reader = new StreamReader(myWebResponse.GetResponseStream());
            string str = reader.ReadLine();
            //CityName = JObject.Parse(str)["name"].ToString();
            //SaveFileCityWeather(str);
            return str;
        }

        /// <summary>
        /// Call for several city IDs
        /// </summary>
        /// <param name="requestParams"></param>
        /// <param name="method"></param>
        /// <returns></returns>

        private StreamReader GetResponse(string requestParams, string method = "GET")
        {
            StringBuilder builder = new StringBuilder(_appSettings.Apiurl);
            builder.Append(requestParams);
            builder.Append(_appSettings.Apiid);
            WebRequest webRequest = WebRequest.Create(builder.ToString());
            webRequest.Method = method;
            webRequest.ContentType = _contentType;
            WebResponse myWebResponse = webRequest.GetResponse();
            StreamReader reader = new StreamReader(myWebResponse.GetResponseStream());
            return reader;
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrudentialUKWeatherAPI.Entities
{
    public class City
    {
        public Weather Weather { get; set; }
        public Coordinates Coordinates { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string ZipCode { get; set; }
    }
}

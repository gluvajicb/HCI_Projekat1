using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    class WeatherInfo
    {
        public string ID { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }

        public List<DayInfo> Forecast { get; set; } = new List<DayInfo>();
    }

    class DayInfo
    {
        public DateTime Date { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public String Description { get; set; }

        public List<TimeInfo> Temperature { get; set; } = new List<TimeInfo>();

        public String DateStr
        {
            get
            {
                return this.Date.ToString("dd-MM-yyyy");
            }
        }

        public String Day
        {
            get
            {
                return this.Date.DayOfWeek.ToString();
            }
        }
    }

    class TimeInfo
    {
        public DateTime Time { get; set; }
        public int Temperature { get; set; }
        public string Description { get; set; }
    }
}

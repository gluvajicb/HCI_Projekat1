using System;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Wpf;

namespace WeatherApp
{
    class WeatherInfo
    {
        public string ID { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }

        public List<DayInfo> Forecast { get; set; } = new List<DayInfo>();

        public string[] Labels { get; set; } = new string[8];
        public SeriesCollection Collection { get; set; } = new SeriesCollection();
        public Func<int, string> YFormatter { get; set; }
        public string CurrentDay { get; set; }
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

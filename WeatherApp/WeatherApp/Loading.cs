using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace WeatherApp
{
    static class Loading
    {
        // API kljucevi i url-ovi
        private const string WEATHER_API_KEY = "6df33da3c349bde6441b5fa06f0e79cb";
        private const string IP_API_KEY = "edb1ac16c206497358cbff42556035a26a5f247780b2414d7c52cee0";
        public const string ForecastUrl = "http://api.openweathermap.org/data/2.5/forecast?id=@LOC@&mode=xml&units=imperial&APPID=" + WEATHER_API_KEY;
        private const string IpUrl = "https://api.ipdata.co?api-key=" + IP_API_KEY;

        /*
         * Ucitava podatke pomocu ip lokatora prilikom pokretanja aplikacije
         */
        public static WeatherInfo getInfo()
        {
            // Pronalazak lokacije putem ip adrese
            string ipResponse = loadIP(IpUrl);
            string cityName = getCityName(ipResponse);
            Console.WriteLine(cityName);


            // Pronalazak id grada
            string cityID = getCityId("Belgrade");
            Console.WriteLine($"Id: {cityID}");

            // Ucitavanje podataka (api poziv)
            string url = ForecastUrl.Replace("@LOC@", cityID);
            string weatherResponse = loadWeather(url);

            // Konvertovanje ucitanog json-a
            WeatherInfo info = convert(weatherResponse, cityID);

            return info;
        }

        /*
         * Ucitavanje lokacije pomocu ip adrese
         */
        public static string loadIP(String url)
        {
            string response = "";
            // Kreiranje web klijenta
            using (WebClient client = new WebClient())
            {
                response = client.DownloadString(url);
            }
            return response;
        }

        /*
         * Ucitavanje podataka o vremenu.
         */
        public static string loadWeather(String url)
        {
            XmlDocument doc = new XmlDocument();
            string jsonText = "";

            // Create a web client.
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(url);
                doc.LoadXml(response);
                jsonText = JsonConvert.SerializeXmlNode(doc);
            }
            return jsonText;
        }

        /*
         * Pronalazak id-a grada na osnovu njegovog imena.
         */
        public static string getCityId(String param)
        {
            List<JObject> cities = new List<JObject>();

            using (StreamReader r = new StreamReader("../../city.list.json"))
            {
                string json = r.ReadToEnd();
                cities = JsonConvert.DeserializeObject<List<JObject>>(json);
            }

            String cityName = "";

            foreach (JObject city in cities)
            {
                cityName = (string)city["name"];
                if (cityName.Equals(param, StringComparison.InvariantCultureIgnoreCase))
                {
                    return (string)city["id"];
                }
            }

            return "";
        }

        /*
         * Ucitavanje imena grada iz json-a (ip lokator)
         */
        public static string getCityName(string json)
        {
            dynamic d = JObject.Parse(json);
            return (string)d.city;
        }

        /*
         * Konvertovanje json odgovora u WeatherInfo objekat
         */
        public static WeatherInfo convert(string info, string id)
        {
            dynamic d = JObject.Parse(info);

            WeatherInfo weatherInfo = new WeatherInfo();

            weatherInfo.ID = id;
            weatherInfo.Location = d.weatherdata.location.name;
            weatherInfo.Country = d.weatherdata.location.country;
            var forecast = d.weatherdata.forecast.time;

            List<DayInfo> days = new List<DayInfo>();
            DayInfo day = new DayInfo();

            DateTime currentDate = DateTime.Today;
            day.Date = currentDate;

            foreach (var time in forecast)
            {
                DateTime date = DateTime.ParseExact((string)time["@from"], "MM/dd/yyyy HH:mm:ss", CultureInfo.InstalledUICulture);

                if (date.Day > currentDate.Day && date.Month >= currentDate.Month && date.Year >= currentDate.Year)
                {
                    day.Min = day.Temperature.Min(r => r.Temperature);
                    day.Max = day.Temperature.Max(r => r.Temperature);
                    day.Description = day.Temperature.GroupBy(s => s.Description)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key;

                    days.Add(day);
                    day = new DayInfo();
                    day.Date = date;
                    currentDate = date;
                }

                TimeInfo tempInfo = new TimeInfo();
                tempInfo.Time = date;
                tempInfo.Temperature = FarenheitToCelsius((double)time.temperature["@value"]);
                tempInfo.Description = time.symbol["@name"];
                day.Temperature.Add(tempInfo);
            }

            weatherInfo.Forecast = days;
            return weatherInfo;

        }

        /*
         * Konvertovanje farenhajta u celzijuse
         */
        private static int FarenheitToCelsius(double farenheit)
        {
            double celsius = (farenheit - 32) * 5 / 9;
            return (int)celsius;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WeatherInfo info;

        public MainWindow()
        {
            info = Loading.getInfo();
            DataContext = info;

            InitializeComponent();

            CityText.Text = "Enter City Name...";

            CityText.GotFocus += RemoveText;
            CityText.LostFocus += AddText;

            loadAllIcon();
        }
        /*
         * Ucitavanje svih ikonica
         */
        public void loadAllIcon()
        {
            string uri = getIconURI(info.Forecast[0].Temperature[0].Description, info.Forecast[0].Temperature[0].Time);
            BitmapImage icon = loadIcon(uri);
            CurrentWeatherIcon.Source = icon;

            uri = getIconURI(info.Forecast[0].Description, DateTime.Parse("12:00:00"));
            icon = loadIcon(uri);
            Day1.Source = icon;

            uri = getIconURI(info.Forecast[1].Description, DateTime.Parse("12:00:00"));
            icon = loadIcon(uri);
            Day2.Source = icon;

            uri = getIconURI(info.Forecast[2].Description, DateTime.Parse("12:00:00"));
            icon = loadIcon(uri);
            Day3.Source = icon;

            uri = getIconURI(info.Forecast[3].Description, DateTime.Parse("12:00:00"));
            icon = loadIcon(uri);
            Day4.Source = icon;

            uri = getIconURI(info.Forecast[4].Description, DateTime.Parse("12:00:00"));
            icon = loadIcon(uri);
            Day5.Source = icon;
        }

        /*
         * Za dinamicko ucitavanje ikonica.
         * Parametar je link do ikonice.
         */
        public BitmapImage loadIcon(string uri)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"pack://application:,,,/WeatherApp;component/" + uri, UriKind.Absolute);
            image.EndInit();
            return image;
        }

        /*
         * Na osnovu tipa vremena vraca link koju sliku treba ucitati
         */
        private string getIconURI(string description, DateTime time)
        {
            string uri = "";

            switch (description)
            {
                case "clear sky":
                    if (time.Hour < 18)
                        uri = "/Images/sunny.png";
                    else
                        uri = "/Images/clear_moon.png";
                    break;
                case "few clouds":
                    if (time.Hour < 18)
                        uri = "/Images/few_clouds.png";
                    else
                        uri = "/Images/night.png";
                    break;
                case "scattered clouds":
                    uri = "/Images/cloud.png";
                    break;
                case "broken clouds":
                    uri = "/Images/cloud.png";
                    break;
                case "shower rain":
                    uri = "/Images/rain.png";
                    break;
                case "rain":
                    uri = "/Images/rain.png";
                    break;
                case "light rain":
                    uri = "/Images/rain.png";
                    break;
                case "thunderstorm":
                    uri = "/Images/thunder.png";
                    break;
                case "snow":
                    uri = "/Images/snowflake.png";
                    break;
                case "mist":
                    uri = "/Images/fog.png";
                    break;

                default:
                    break;
            }

            return uri;
        }

        /*
         * Button click za side menu
         */
        private void SideMenu_Toggle(object sender, RoutedEventArgs e)
        {
            this.Menu.Visibility = this.Menu.Visibility == Visibility.Visible
                                ? Visibility.Collapsed
                                : Visibility.Visible;
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            string city = CityText.Name;
            string cityID = Loading.getCityId(city);

            if (cityID != "")
            {
                string url = Loading.ForecastUrl.Replace("@LOC@", cityID);
                string weatherResponse = Loading.loadWeather(url);
                info = Loading.convert(weatherResponse, cityID);
            }
        }

        public void RemoveText(object sender, EventArgs e)
        {
            CityText.Text = "";
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CityText.Text))
                CityText.Text = "Enter City Name...";
        }
    }
}

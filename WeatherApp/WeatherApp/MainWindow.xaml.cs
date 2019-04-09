using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Linq;
using LiveCharts.Wpf;
using LiveCharts;

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

            init();
        }

        private void init()
        {
            InitializeComponent();
            InitializeFavourites();

            CityText.Text = "Enter City Name...";

            CityText.GotFocus += RemoveText;
            CityText.LostFocus += AddText;

            loadAllIcon();

            info.CurrentDay = info.Forecast[0].Day;
            detailedView(0);
        }

        private void detailedView(int day)
        {
            LineSeries line = new LineSeries();
            line.Title = "";
            line.Values = new ChartValues<int>();
            line.PointGeometry = DefaultGeometries.Circle;

            int i = 0;

            foreach (TimeInfo t in info.Forecast[day].Temperature)
            {
                line.Values.Add(t.Temperature);
                info.Labels[i] = (t.Time.Hour) + ":00h";
                i++;
            }

            info.Collection.Add(line);

            info.YFormatter = value => value.ToString();
        }

        private void InitializeFavourites()
        {
            Menu.Items.Clear();

            foreach (string line in File.ReadLines("favourites.txt"))
            {
                MenuItem item = new MenuItem();
                item.Header = line.ToString();
                item.Click += ShowFavourite;
                Menu.Items.Add(item);
            }
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

            /*uri = getIconURI(info.Forecast[4].Description, DateTime.Parse("12:00:00"));
            icon = loadIcon(uri);
            Day5.Source = icon;*/
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
                case "light snow":
                    uri = "/Images/snowflake.png";
                    break;
                case "snowflake":
                    uri = "/Images/snowflake.png";
                    break;
                case "mist":
                    uri = "/Images/fog.png";
                    break;

                default:
                    uri = "/Images/noicon.png";
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

        private void ShowFavourite(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            string city = item.Header.ToString();
            string cityID = Loading.getCityId(city);

            if (cityID != "")
            {
                string url = Loading.ForecastUrl.Replace("@LOC@", cityID);
                string weatherResponse = Loading.loadWeather(url);
                info = Loading.convert(weatherResponse, cityID);
                init();
                Location.Text = info.Location;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("City you were looking for can't be found. Try again.");
            }
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            string city = CityText.Text;
            string cityID = Loading.getCityId(city);

            if (cityID != "")
            {
                string url = Loading.ForecastUrl.Replace("@LOC@", cityID);
                string weatherResponse = Loading.loadWeather(url);
                info = Loading.convert(weatherResponse, cityID);
                init();
                Location.Text = info.Location;

                AddToHistory();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("City you were looking for can't be found. Try again.");
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

        private void AddToFavourites(object sender, RoutedEventArgs e)
        {
            string path = "favourites.txt";
            if (!isInFavourites(info.Location))
            {
                using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
                {
                
                    sw.WriteLine(info.Location);
                    MessageBoxResult result = MessageBox.Show("City has been added to favourites.");
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Your city is already in favourites!");
            }
        }

        private void AddToHistory()
        {
            string path = "history.txt";

            using (StreamWriter sw = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
            {
                sw.WriteLine(info.Location);
                MessageBoxResult result = MessageBox.Show("City has been added to history.");
            }

            var lines = File.ReadAllLines(path).Skip(1);
            if (lines.Count() > 5) {
                File.WriteAllLines(path, lines);
            }
        }

        private bool isInFavourites(string city)
        {
            foreach (string line in File.ReadLines("favourites.txt"))
                if (city == line)
                    return true; 
            return false;
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            init();
        }
    }
}

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Marinex.Services
{
    /// <summary>
    /// Service untuk fetch real-time weather data dari OpenWeatherMap API
    /// Mendukung current weather, forecast, dan maritime-specific data
    /// </summary>
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BASE_URL = "https://api.openweathermap.org/data/2.5";

        public WeatherService(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        /// <summary>
        /// Get current weather by coordinates (untuk lokasi kapal)
        /// </summary>
        public async Task<WeatherData> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            try
            {
                string url = $"{BASE_URL}/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                string json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonSerializer.Deserialize<OpenWeatherMapResponse>(json, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return ConvertToWeatherData(weatherResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch weather data: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get current weather by city name
        /// </summary>
        public async Task<WeatherData> GetCurrentWeatherByCityAsync(string cityName)
        {
            try
            {
                string url = $"{BASE_URL}/weather?q={cityName}&appid={_apiKey}&units=metric";
                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                string json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonSerializer.Deserialize<OpenWeatherMapResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return ConvertToWeatherData(weatherResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch weather data: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get 5-day forecast (untuk planning voyage)
        /// </summary>
        public async Task<ForecastData> Get5DayForecastAsync(double latitude, double longitude)
        {
            try
            {
                string url = $"{BASE_URL}/forecast?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                string json = await response.Content.ReadAsStringAsync();
                var forecastResponse = JsonSerializer.Deserialize<OpenWeatherMapForecastResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return ConvertToForecastData(forecastResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch forecast data: {ex.Message}", ex);
            }
        }

        private WeatherData ConvertToWeatherData(OpenWeatherMapResponse response)
        {
            return new WeatherData
            {
                Location = response.Name,
                Latitude = response.Coord.Lat,
                Longitude = response.Coord.Lon,
                Temperature = response.Main.Temp,
                FeelsLike = response.Main.FeelsLike,
                TempMin = response.Main.TempMin,
                TempMax = response.Main.TempMax,
                Pressure = response.Main.Pressure,
                Humidity = response.Main.Humidity,
                WindSpeed = response.Wind.Speed,
                WindDegree = response.Wind.Deg,
                WindGust = response.Wind.Gust,
                Condition = response.Weather[0].Main,
                Description = response.Weather[0].Description,
                Icon = response.Weather[0].Icon,
                Clouds = response.Clouds.All,
                Visibility = response.Visibility,
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(response.Dt).DateTime,
                Sunrise = DateTimeOffset.FromUnixTimeSeconds(response.Sys.Sunrise).DateTime,
                Sunset = DateTimeOffset.FromUnixTimeSeconds(response.Sys.Sunset).DateTime
            };
        }

        private ForecastData ConvertToForecastData(OpenWeatherMapForecastResponse response)
        {
            var forecast = new ForecastData
            {
                City = response.City.Name,
                Latitude = response.City.Coord.Lat,
                Longitude = response.City.Coord.Lon,
                ForecastList = new System.Collections.Generic.List<WeatherData>()
            };

            foreach (var item in response.List)
            {
                forecast.ForecastList.Add(new WeatherData
                {
                    Location = response.City.Name,
                    Latitude = response.City.Coord.Lat,
                    Longitude = response.City.Coord.Lon,
                    Temperature = item.Main.Temp,
                    FeelsLike = item.Main.FeelsLike,
                    TempMin = item.Main.TempMin,
                    TempMax = item.Main.TempMax,
                    Pressure = item.Main.Pressure,
                    Humidity = item.Main.Humidity,
                    WindSpeed = item.Wind.Speed,
                    WindDegree = item.Wind.Deg,
                    WindGust = item.Wind.Gust,
                    Condition = item.Weather[0].Main,
                    Description = item.Weather[0].Description,
                    Icon = item.Weather[0].Icon,
                    Clouds = item.Clouds.All,
                    Visibility = item.Visibility,
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime
                });
            }

            return forecast;
        }

        /// <summary>
        /// Check apakah cuaca aman untuk sailing
        /// </summary>
        public static bool IsSafeForSailing(WeatherData weather)
        {
            // Kriteria keamanan maritim
            bool safeWind = weather.WindSpeed < 15; // < 15 m/s (~30 knots)
            bool safeVisibility = weather.Visibility > 1000; // > 1 km
            bool noStorm = !weather.Condition.Contains("Thunderstorm") && 
                           !weather.Condition.Contains("Squall");
            
            return safeWind && safeVisibility && noStorm;
        }

        /// <summary>
        /// Get sea condition description based on wind speed (Beaufort scale)
        /// </summary>
        public static string GetSeaCondition(double windSpeed)
        {
            // Wind speed in m/s
            return windSpeed switch
            {
                < 0.5 => "Calm - Mirror-like sea",
                < 2.0 => "Light Air - Ripples",
                < 3.5 => "Light Breeze - Small wavelets",
                < 5.5 => "Gentle Breeze - Large wavelets",
                < 8.0 => "Moderate Breeze - Small waves",
                < 11.0 => "Fresh Breeze - Moderate waves",
                < 14.0 => "Strong Breeze - Large waves",
                < 17.0 => "Near Gale - Sea heaps up",
                < 21.0 => "Gale - Moderately high waves",
                < 24.5 => "Strong Gale - High waves",
                < 28.5 => "Storm - Very high waves",
                < 32.5 => "Violent Storm - Exceptionally high waves",
                _ => "Hurricane Force - Air filled with foam and spray"
            };
        }

        /// <summary>
        /// Get warning level berdasarkan kondisi cuaca
        /// </summary>
        public static string GetWarningLevel(WeatherData weather)
        {
            if (weather.WindSpeed > 25 || weather.Condition.Contains("Thunderstorm"))
                return "CRITICAL - Do not sail";
            
            if (weather.WindSpeed > 15 || weather.Visibility < 1000)
                return "HIGH - Sailing not recommended";
            
            if (weather.WindSpeed > 10 || weather.Condition.Contains("Rain"))
                return "MODERATE - Caution advised";
            
            return "LOW - Safe for sailing";
        }

        // DTOs untuk OpenWeatherMap API Response
        private class OpenWeatherMapResponse
        {
            public CoordData Coord { get; set; }
            public WeatherInfo[] Weather { get; set; }
            public MainData Main { get; set; }
            public WindData Wind { get; set; }
            public CloudsData Clouds { get; set; }
            public int Visibility { get; set; }
            public long Dt { get; set; }
            public SysData Sys { get; set; }
            public string Name { get; set; }
        }

        private class OpenWeatherMapForecastResponse
        {
            public ForecastItem[] List { get; set; }
            public CityData City { get; set; }
        }

        private class ForecastItem
        {
            public long Dt { get; set; }
            public MainData Main { get; set; }
            public WeatherInfo[] Weather { get; set; }
            public CloudsData Clouds { get; set; }
            public WindData Wind { get; set; }
            public int Visibility { get; set; }
        }

        private class CoordData
        {
            public double Lon { get; set; }
            public double Lat { get; set; }
        }

        private class WeatherInfo
        {
            public string Main { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
        }

        private class MainData
        {
            public double Temp { get; set; }
            public double FeelsLike { get; set; }
            public double TempMin { get; set; }
            public double TempMax { get; set; }
            public int Pressure { get; set; }
            public int Humidity { get; set; }
        }

        private class WindData
        {
            public double Speed { get; set; }
            public int Deg { get; set; }
            public double Gust { get; set; }
        }

        private class CloudsData
        {
            public int All { get; set; }
        }

        private class SysData
        {
            public long Sunrise { get; set; }
            public long Sunset { get; set; }
        }

        private class CityData
        {
            public string Name { get; set; }
            public CoordData Coord { get; set; }
        }
    }

    // Data models untuk weather
    public class WeatherData
    {
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public int Pressure { get; set; } // hPa
        public int Humidity { get; set; } // %
        public double WindSpeed { get; set; } // m/s
        public int WindDegree { get; set; }
        public double WindGust { get; set; }
        public string Condition { get; set; } // Clear, Clouds, Rain, etc.
        public string Description { get; set; }
        public string Icon { get; set; }
        public int Clouds { get; set; } // %
        public int Visibility { get; set; } // meters
        public DateTime Timestamp { get; set; }
        public DateTime? Sunrise { get; set; }
        public DateTime? Sunset { get; set; }

        public string GetSeaCondition()
        {
            return WeatherService.GetSeaCondition(WindSpeed);
        }

        public string GetWarningLevel()
        {
            return WeatherService.GetWarningLevel(this);
        }

        public bool IsSafeForSailing()
        {
            return WeatherService.IsSafeForSailing(this);
        }
    }

    public class ForecastData
    {
        public string City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public System.Collections.Generic.List<WeatherData> ForecastList { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using SCCD.Services.Interfaces;

namespace SCCD.Services.Entities
{
    public class WeatherService : IWeatherService
    {
        private readonly IConfiguration _configuration;

        public WeatherService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, string Message, string Data)> ObtenerClima(string localidad, string fecha)
        {
            try
            {
                DateTime requestedDate;
                if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out requestedDate))
                {
                    return (false, "Formato de fecha invalido. Por favor, utilizar yyyy-MM-dd.", null);
                }

                int daysDifference = (requestedDate - DateTime.Now.Date).Days;
                string endpoint = string.Empty;
                if (daysDifference >= 0 && daysDifference <= 14)
                {
                    endpoint = "forecast.json";
                }
                else if (daysDifference >= 15 && daysDifference <= 300)
                {
                    endpoint = "future.json";
                }
                else
                {
                    return (false, "La fecha debe estar entre 0 a 300 dias desde la fecha actual.", null);
                }

                using HttpClient client = new HttpClient();
                //var config = new ConfigurationBuilder()
                //    .AddJsonFile("appsettings.json")
                //    .Build();
                string API_KEY = _configuration["WeatherAPIKey"];
                string url = $"http://api.weatherapi.com/v1/{endpoint}?key={API_KEY}&q={localidad}&dt={fecha}";

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    return (true, null, data);
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    return (false, errorResponse, null);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }

}

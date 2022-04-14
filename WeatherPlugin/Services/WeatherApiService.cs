namespace Loupedeck.WeatherPlugin.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;

    public class WeatherApiService
    {

        private readonly static HttpClient Client = new HttpClient();
        private readonly static string BaseURL = "https://api.weatherapi.com/v1/current.json";

        private readonly string _apiKey;
        public WeatherApiService(string apiKey = null)
        {
            this._apiKey = apiKey;
        }

        public async Task<Models.Weather> GetCurrentWeather(string locationQuery, string apiKey = null)
        {
            using (var tokeSource = new CancellationTokenSource())
            {
                HttpResponseMessage response = null;
                try
                {
                    apiKey = apiKey ?? this._apiKey;
                    response = await Client.GetAsync($"{BaseURL}?key={apiKey}&q={locationQuery}&aqi=no", tokeSource.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR - {ex.ToString()}");
                    throw;
                }

                return await response.Content.ReadAsAsync<Models.Weather>();
            }
        }


        public async Task<byte[]> GetIconBytes(Models.Weather weather)
        {
            if (string.IsNullOrEmpty(weather.current.condition.icon))
                return null;

            using (var iconResponse = await Client.GetAsync($"https:{weather.current.condition.icon}"))
            {
                return await iconResponse.Content.ReadAsByteArrayAsync();
            }
        }

    }
}

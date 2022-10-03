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
    using System.Net.Http.Json;
    using Loupedeck.WeatherPlugin.Models;

    public sealed class WeatherApiService
    {
        public const string ClientName = "openWeatherApi";
        private readonly static string BaseURL = "https://api.openweathermap.org/data/";
        
        private HttpClient _client;

        public WeatherApiService(IHttpClientFactory httpClientFactory = null)
        {            
            this._client = HttpClientFactory.Create();
            this._client.BaseAddress = new Uri(BaseURL);
        }

        ~WeatherApiService()
        {
            this._client.Dispose();
            this._client = null;
        }

        public async Task<WeatherResponse> GetCurrentWeather(string zipAndCountry, string apiKey, CancellationToken cancellationToken)
        {
            WeatherResponse response = null;
            try
            {
                response = await this._client.GetFromJsonAsync<WeatherResponse>($"2.5/weather?zip={zipAndCountry}&appid={apiKey}", cancellationToken);
                var iconBytes = await this._client.GetByteArrayAsync(new Uri($"https://openweathermap.org/img/wn/{response.weather[0].icon}@2x.png"));
                response.weather[0].iconBytes = iconBytes;
            }catch(Exception)
            {
                //throw;
            }

            return response;
        }

    }
}

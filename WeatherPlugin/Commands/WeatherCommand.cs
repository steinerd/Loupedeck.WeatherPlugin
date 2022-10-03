namespace Loupedeck.WeatherPlugin.Commands
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Timers;

    using Timer = System.Timers.Timer;

    public sealed class WeatherCommand : PluginDynamicCommand
    {
        private CancellationToken _instanceCancellation = new CancellationToken();

        private readonly Services.WeatherApiService _weatherApiService;
        private readonly static WeatherPlugin Parent = WeatherPlugin.Instance;
        private readonly Timer Timer = new Timer(TimeSpan.FromMinutes(15).TotalMilliseconds) { Enabled = false, AutoReset = true };

        static internal ConcurrentDictionary<string, Data> DataCache = new ConcurrentDictionary<string, Data>();


        internal class Data
        {
            public string Name { get; set; }
            public (float C, float F) Temperature { get; set; }
            public BitmapImage Icon { get; set; }
            public bool IsValid { get; set; }
            public bool IsLoading { get; set; }
            public bool IsToggled { get; set; }
            public bool HideName { get; set; }
        }

        public WeatherCommand() : base("Locations", "Weather Locations", "Weather Locations")
        {
            this.MakeProfileAction("text;Use zip then 2-letter ISO Country Code.\n\tThe input must look like 'Zip,ISOCountryCode:API Key[:HideName]'\n\tExample: 14220,US:6dc3123b6e844f9e9580b205552a8c");
            this.Timer.Elapsed += this.OnTimerElapse;
            this.Timer.Enabled = true;

            this._weatherApiService = new Services.WeatherApiService();
        }

        protected override Boolean OnLoad()
        {
            return base.OnLoad();
        }

        private void OnTimerElapse(object sender, ElapsedEventArgs e)
        {
            foreach (var ap in DataCache.Keys)
            {
                this.RetrieveData(ap, this._instanceCancellation);
                this.ActionImageChanged(ap);
                DataCache.TryRemove(ap, out var _);
            }

            this.Timer.AutoReset = true;
            this.Timer.Enabled = true;
            this._instanceCancellation = new CancellationToken(false);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (string.IsNullOrEmpty(actionParameter))
            {
                return null;
            }

            Data data = this.GetData(actionParameter);
            if (!data.IsValid)
            {
                return null;
            }

            var iconBuilder = new BitmapBuilder(imageSize);
            iconBuilder.Clear(BitmapColor.Black);

            if (data.Icon != null)
            {
                iconBuilder.DrawImage(data.Icon);
            }

            iconBuilder.FillRectangle(0, 0, iconBuilder.Width, iconBuilder.Height, new BitmapColor(0, 0, 0, 128));

            var locationName = string.Empty;
            if (!data.HideName)
            {
                locationName = $"{data.Name}\n\u00a0\n";
            }

            if(data.Temperature.C != data.Temperature.F)
            {
                iconBuilder.DrawText($"{locationName}{Math.Round(data.Temperature.C)}°C/{Math.Round(data.Temperature.F)}°F", color: BitmapColor.Black, fontSize: 17);
                iconBuilder.DrawText($"{locationName}{Math.Round(data.Temperature.C)}°C/{Math.Round(data.Temperature.F)}°F", color: BitmapColor.White);
            }
            else
            {
                iconBuilder.DrawText($"PARAMETER\nERROR", color: BitmapColor.Black, fontSize: 17);
                iconBuilder.DrawText($"PARAMETER\nERROR", color: BitmapColor.White);
            }

            var renderedImage = iconBuilder.ToImage();
            iconBuilder.Dispose();

            return renderedImage;
        }

        protected override void RunCommand(String actionParameter) => System.Diagnostics.Process.Start($"weather:{actionParameter.Split(':')[0]}");

        private async void RetrieveData(string actionParameter, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(actionParameter))
            {
                return;
            }

            Data data = this.GetData(actionParameter);

            if (data.IsLoading)
            {
                return;
            }

            data.IsLoading = true;

            try
            {
                var paramArgs = actionParameter.Split(':');
                var locationQuery = paramArgs[0].Trim();
                var apiKey = paramArgs[1].Trim();

                if (paramArgs.Length > 2)
                {
                    data.HideName = bool.Parse(paramArgs[2] ?? "false");
                }

                var weather = await this._weatherApiService.GetCurrentWeather(locationQuery, apiKey, cancellationToken);
                if (weather == null)
                {
                    // Data is from an older version, clearing. 
                    data.Name = "ERR";
                    data.Temperature = (C: 0, F: 0);
                    return;
                }

                data.Name = weather.name;
                data.Temperature = (C: weather.main.celsius, F: weather.main.fahrenheit);

                data.Icon = BitmapImage.FromBase64String(Convert.ToBase64String(weather.weather[0].iconBytes));
            }
            finally
            {
                data.IsLoading = false;
                data.IsValid = true;
                this.ActionImageChanged(actionParameter);
            }
        }

        private Data GetData(string actionParameter)
        {
            if (DataCache.TryGetValue(actionParameter, out var data))
            {
                return data;
            }

            data = new Data();
            DataCache.TryAdd(actionParameter, data);
            
            this.RetrieveData(actionParameter, this._instanceCancellation);

            return data;
        }

    }
}

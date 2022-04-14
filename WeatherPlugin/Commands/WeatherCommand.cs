namespace Loupedeck.WeatherPlugin.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Timers;

    using Timer = System.Timers.Timer;

    public class WeatherCommand : PluginDynamicCommand
    {
        private readonly Services.WeatherApiService weatherApiService = new Services.WeatherApiService();
        private readonly static WeatherPlugin Parent = WeatherPlugin.Instance;
        private System.Timers.Timer Timer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds) { Enabled = false, AutoReset = true };

        internal IDictionary<string, Data> DataCache = new Dictionary<string, Data>();

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
            this.MakeProfileAction("text;Use Zip, City Name, Postal Code, or even IP address to help get location.     The input must be 'Location:API Key[:HideName]'    Example: Paris:6dc3123b6e844f9e9580b205552a8c");
            this.Timer.Elapsed += this.OnTimerElapse;
            this.Timer.Enabled = true;
        }

        protected override Boolean OnLoad()
        {
            return base.OnLoad();
        }

        private void OnTimerElapse(object sender, ElapsedEventArgs e)
        {
            foreach (var ap in this.DataCache.Keys)
            {
                this.RetrieveData(ap);
                this.ActionImageChanged(ap);
            }
            this.Timer.AutoReset = true;
            this.Timer.Enabled = true;
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            if (string.IsNullOrEmpty(actionParameter))
                return null;

            Data data = this.GetData(actionParameter);
            if (!data.IsValid)
                return null;

            var iconBuilder = new BitmapBuilder(imageSize);
            iconBuilder.Clear(BitmapColor.Black);

            if (data.Icon != null)
                iconBuilder.DrawImage(data.Icon);

            iconBuilder.FillRectangle(0, 0, iconBuilder.Width, iconBuilder.Height, new BitmapColor(0, 0, 0, 128));

            var locationName = string.Empty;
            if (!data.HideName)
            {
                locationName = $"{data.Name}\n\u00a0\n";
            }

            iconBuilder.DrawText($"{locationName}{Math.Round(data.Temperature.C)}°C/{Math.Round(data.Temperature.F)}°F", color: BitmapColor.Black, fontSize: 17);
            iconBuilder.DrawText($"{locationName}{Math.Round(data.Temperature.C)}°C/{Math.Round(data.Temperature.F)}°F", color: BitmapColor.White);

            var renderedImage = iconBuilder.ToImage();
            iconBuilder.Dispose();

            return renderedImage;
        }

        protected override void RunCommand(String actionParameter) => System.Diagnostics.Process.Start($"weather:{actionParameter.Split(':')[0]}");

        private async void RetrieveData(string actionParameter)
        {
            if (string.IsNullOrEmpty(actionParameter))
                return;

            Data data = this.GetData(actionParameter);

            if (data.IsLoading)
                return;

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

                Models.Weather weather = null;
                try
                {
                    weather = await this.weatherApiService.GetCurrentWeather(locationQuery, apiKey);
                }
                catch (Exception ex)
                {
                    return;
                }

                data.Name = weather.location.name;
                data.Temperature = (C: weather.current.temp_c, F: weather.current.temp_f);

                var iconBytes = await this.weatherApiService.GetIconBytes(weather);
                data.Icon = BitmapImage.FromBase64String(Convert.ToBase64String(iconBytes));

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
            if (this.DataCache.TryGetValue(actionParameter, out var data))
                return data;

            data = new Data();
            this.DataCache.Add(actionParameter, data);

            this.RetrieveData(actionParameter);

            return data;
        }

    }
}

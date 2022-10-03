namespace Loupedeck.WeatherPlugin
{
    using System;

    public sealed class WeatherApplication : ClientApplication
    {
        public WeatherApplication()
        {

        }

        protected override String GetProcessName() => "";

        protected override String GetBundleName() => "";
    }
}
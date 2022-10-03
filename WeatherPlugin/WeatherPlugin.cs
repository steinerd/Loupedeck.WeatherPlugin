namespace Loupedeck.WeatherPlugin
{
    using System;
    using Microsoft.Win32;

    public sealed class WeatherPlugin : Plugin
    {
        internal static WeatherPlugin Instance { get; private set; }
        public override Boolean HasNoApplication => true;
        public override Boolean UsesApplicationApiOnly => true;

        public WeatherPlugin()
        {
            WeatherPlugin.Instance = this;
            
        }

        public override void Load() => this.Init();

        public override void Unload() { }

        private void OnApplicationStarted(Object sender, EventArgs e) { }

        private void OnApplicationStopped(Object sender, EventArgs e) { }

        public override void RunCommand(String commandName, String parameter) { }

        public override void ApplyAdjustment(String adjustmentName, String parameter, Int32 diff) { }

        private void Init()
        {
            this.Info.Icon16x16 = EmbeddedResources.ReadImage("Loupedeck.WeatherPlugin.Resources.16.png");
            this.Info.Icon32x32 = EmbeddedResources.ReadImage("Loupedeck.WeatherPlugin.Resources.32.png");
            this.Info.Icon48x48 = EmbeddedResources.ReadImage("Loupedeck.WeatherPlugin.Resources.48.png");
            this.Info.Icon256x256 = EmbeddedResources.ReadImage("Loupedeck.WeatherPlugin.Resources.256.png");
            RegisterMyProtocol();
        }

        static void RegisterMyProtocol()
        {
            var name = nameof(WeatherPlugin).Replace("Plugin", "").ToLower();
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(name);

            if (key == null)
            {
                key = Registry.ClassesRoot.CreateSubKey(name);
                key.SetValue(string.Empty, $"URL: {name} Protocol");
                key.SetValue("URL Protocol", string.Empty);

                key = key.CreateSubKey(@"shell\open\command");
                key.SetValue(string.Empty, @"""C:\Windows\System32\cmd.exe"" /c ""START https://weather.com/""");                
            }

            key.Close();
        }

    }
}

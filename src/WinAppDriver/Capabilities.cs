namespace WinAppDriver
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class Capabilities
    {
        public Capabilities()
        {
        }

        public Capabilities(IDictionary<string, object> capabilities)
        {
            // TODO simplify data copy
            this.PlatformName = this.ExtractStringCap(capabilities, "platformName");
            this.PackageName = this.ExtractStringCap(capabilities, "packageName");
            this.App = this.ExtractStringCap(capabilities, "app");
            this.MD5 = this.ExtractStringCap(capabilities, "md5");
            this.InstallCommand = this.ExtractStringCap(capabilities, "installCommand");
            this.UninstallCommand = this.ExtractStringCap(capabilities, "uninstallCommand");
            this.ResetCommand = this.ExtractStringCap(capabilities, "resetCommand");
            this.OpenCommand = this.ExtractStringCap(capabilities, "openCommand");
            this.CloseCommand = this.ExtractStringCap(capabilities, "closeCommand");
        }

        [JsonProperty("platformName")]
        public string PlatformName { get; set; }

        [JsonProperty("packageName")]
        public string PackageName { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("md5")]
        public string MD5 { get; set; }

        [JsonProperty("installCommand")]
        public string InstallCommand { get; set; }

        [JsonProperty("uninstallCommand")]
        public string UninstallCommand { get; set; }

        [JsonProperty("resetCommand")]
        public string ResetCommand { get; set; }

        [JsonProperty("openCommand")]
        public string OpenCommand { get; set; }

        [JsonProperty("closeCommand")]
        public string CloseCommand { get; set; }

        private string ExtractStringCap(IDictionary<string, object> caps, string key)
        {
            object value;
            caps.TryGetValue(key, out value);
            return (string)value;
        }
    }
}
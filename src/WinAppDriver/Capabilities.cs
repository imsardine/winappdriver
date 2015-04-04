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
        }

        [JsonProperty("platformName")]
        public string PlatformName { get; set; }

        [JsonProperty("packageName")]
        public string PackageName { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("md5")]
        public string MD5 { get; set; }

        private string ExtractStringCap(IDictionary<string, object> caps, string key)
        {
            object value;
            caps.TryGetValue(key, out value);
            return (string)value;
        }
    }
}
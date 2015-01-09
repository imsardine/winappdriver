namespace WinAppDriver
{
    using Newtonsoft.Json;

    internal class Capabilities
    {
        [JsonProperty("platformName")]
        public string PlatformName { get; set; }

        public string AppUserModelId { get; set; }

        public string App { get; set; }
    }
}
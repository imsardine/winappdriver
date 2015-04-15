namespace WinAppDriver
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    internal enum Platform
    {
        Windows,
        WindowsModern,
        WindowsPhone
    }
}
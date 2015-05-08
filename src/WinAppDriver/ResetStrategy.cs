namespace WinAppDriver
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    internal enum ResetStrategy
    {
        [EnumMember(Value = "fastReset")]
        Fast,

        [EnumMember(Value = "fullReset")]
        Full,

        [EnumMember(Value = "noReset")]
        No
    }
}
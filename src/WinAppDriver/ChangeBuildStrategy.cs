namespace WinAppDriver
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    internal enum ChangeBuildStrategy
    {
        [EnumMember(Value = "reinstall")]
        Reinstall,

        [EnumMember(Value = "upgrade")]
        Upgrade
    }
}
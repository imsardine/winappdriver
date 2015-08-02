namespace WinAppDriver
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    internal enum LocatorStrategy
    {
        [EnumMember(Value = "id")]
        Id,

        [EnumMember(Value = "name")]
        Name,

        [EnumMember(Value = "tag name")]
        TagName,

        [EnumMember(Value = "class name")]
        ClassName,

        [EnumMember(Value = "xpath")]
        XPath,

        [EnumMember(Value = "css selector")]
        CssSelector,

        [EnumMember(Value = "link text")]
        LinkText,

        [EnumMember(Value = "partial link text")]
        PartialLinkText,
    }
}
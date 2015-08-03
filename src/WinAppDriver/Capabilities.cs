namespace WinAppDriver
{
    using System.ComponentModel;
    using Newtonsoft.Json;

    internal class Capabilities
    {
        [DefaultValue(Platform.Windows)]
        [JsonProperty("platformName")]
        public Platform Platform { get; set; }

        [JsonProperty("appID")]
        public string AppID { get; set; }

        [JsonProperty("packageName")]
        public string PackageName { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("appChecksum")]
        public string AppChecksum { get; set; }

        [JsonProperty("checkInstalledCommand")]
        public string CheckInstalledCommand { get; set; }

        [JsonProperty("installCommand")]
        public string InstallCommand { get; set; }

        [JsonProperty("uninstallCommand")]
        public string UninstallCommand { get; set; }

        [JsonProperty("backupCommand")]
        public string BackupStatesCommand { get; set; }

        [JsonProperty("restoreCommand")]
        public string RestoreStatesCommand { get; set; }

        [JsonProperty("openCommand")]
        public string OpenCommand { get; set; }

        [JsonProperty("closeCommand")]
        public string CloseCommand { get; set; }

        [DefaultValue(50)]
        [JsonProperty("keystrokeDelay")]
        public int KeystrokeDelay { get; set; }

        [DefaultValue(ChangeBuildStrategy.Reinstall)]
        [JsonProperty("changeBuildStrategy")]
        public ChangeBuildStrategy ChangeBuildStrategy { get; set; }

        [DefaultValue(ResetStrategy.Fast)]
        [JsonProperty("resetStrategy")]
        public ResetStrategy ResetStrategy { get; set; }
    }
}
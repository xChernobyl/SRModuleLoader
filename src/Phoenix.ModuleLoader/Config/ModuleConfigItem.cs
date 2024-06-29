using Newtonsoft.Json;

namespace Phoenix.ModuleLoader.Config
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModuleConfigItem
    {
        [JsonProperty(PropertyName = "Module Name")]
        public string ModuleName { get; set; }

        [JsonProperty(PropertyName = "Module path")]
        public string ModulePath { get; set; }

        [JsonProperty(PropertyName = "Spoof IP")]
        public string SpoofIP { get; set; }

        public ModuleConfigItem()
        {
            ModulePath = "[Dummy name]";
            ModulePath = "[Dummy path]";
            SpoofIP = "[Dummy IP]";
        }

        public ModuleConfigItem(string moduleName, string modulePath, string spoofIP)
        {
            ModuleName = moduleName;
            ModulePath = modulePath;
            SpoofIP = spoofIP;
        }
    }
}

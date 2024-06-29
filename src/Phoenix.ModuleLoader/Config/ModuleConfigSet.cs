using System.Collections.Generic;
using Newtonsoft.Json;

namespace Phoenix.ModuleLoader.Config
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModuleConfigSet
    {
        [JsonProperty(PropertyName = "Module configs")]
        public List<ModuleConfigItem> ModuleConfigs { get; private set; }

        public ModuleConfigSet()
        {
            ModuleConfigs = new List<ModuleConfigItem>();
        }
    }
}

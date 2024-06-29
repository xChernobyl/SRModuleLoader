using System.IO;
using System.Windows.Forms;
using Phoenix.ModuleLoader.Config;

namespace Phoenix.ModuleLoader
{
    internal class ConfigManager
    {
        public static ModuleConfigSet ModuleConfigSet;

        private static ListView ListView;

        public static void AssignListView(ListView lv) => 
            ConfigManager.ListView = lv;

        public static bool TryLoad(string path)
        {
            if (!File.Exists(path))
            {
                ModuleConfigSet = new ModuleConfigSet();
                //ModuleConfigSet.ModuleConfigs.Add(
                //    new ModuleConfigItem("Machine Manager #1", "Dummy path", "192.168.0.130")
                //    );
                JsonSerializationHelper.TrySerializeToFile<ModuleConfigSet>(
                    ConfigManager.ModuleConfigSet, "settings.json", settings: null, encoding: null);

            }

            bool deserialized = JsonSerializationHelper.TryDeserializeFromFile<ModuleConfigSet>(
                out ConfigManager.ModuleConfigSet, path, settings: null, encoding: null);

            return deserialized;
        }

        public static bool TrySave(string path)
        {
            ModuleConfigSet.ModuleConfigs.Clear();

            //Write items from ListView to json.
            foreach (ListViewItem item in ConfigManager.ListView.Items)
            {
                string moduleName = item.SubItems[0].Text;
                string modulePath = item.SubItems[1].Text;
                string spoofIp = item.SubItems[2].Text;


                ModuleConfigSet.ModuleConfigs.Add(
                    new ModuleConfigItem(moduleName, modulePath, spoofIp)
                    );
            }

            bool serialized = JsonSerializationHelper.TrySerializeToFile<ModuleConfigSet>(
                in ConfigManager.ModuleConfigSet, path, settings: null, encoding: null);

            return serialized;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Yaml.Serialization;

namespace SparseApp.Plugins
{
    public class PluginManager: IPluginManager
    {
        protected Dictionary<String, IPlugin> plugins = new Dictionary<String, IPlugin>();

        public Dictionary<String, IPlugin> Plugins
        {
            get
            {
                return this.plugins;
            }
        }

        public PluginManager()
        {
        }

        public virtual void ImportFile(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                var serializer = new YamlSerializer();
                Plugin plugin = (Plugin)serializer.Deserialize(stream, typeof(Plugin))[0];
                AddPlugin(Path.GetFileNameWithoutExtension(file), plugin);
            }
        }

        public virtual void AddPlugin(string name, IPlugin plugin)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                var serializer = new YamlSerializer();
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(name + ".yml", FileMode.OpenOrCreate, FileAccess.Write, store))
                {
                    serializer.Serialize(stream, plugin);
                    stream.Close();
                }
                store.Close();
            }
            plugins.Add(name, plugin);
        }

        public virtual void RemovePlugin(string name)
        {
            plugins.Remove(name);
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                store.DeleteFile(name + ".yml");
                store.Close();
            }
        }

        public virtual List<string> LoadPlugins()
        {
            List<string> pluginsFailedToLoad = new List<string>();
            plugins = new Dictionary<String, IPlugin>();
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                var serializer = new YamlSerializer();
                foreach (string file in store.GetFileNames("*.yml"))
                {
                    try
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(file, FileMode.Open, FileAccess.Read, store))
                        {
                            Plugin plugin = (Plugin)serializer.Deserialize(stream, typeof(Plugin))[0];
                            plugins.Add(Path.GetFileNameWithoutExtension(file), plugin);
                        }
                    }
                    catch
                    {
                        pluginsFailedToLoad.Add(file);
                    }
                }
                store.Close();
            }
            return pluginsFailedToLoad;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using System.Yaml.Serialization;

namespace SparseApp.Plugins
{
    public class Manager
    {
        protected string folder;

        protected Dictionary<String, Plugin> plugins = new Dictionary<String, Plugin>();

        public Dictionary<String, Plugin> Plugins
        {
            get
            {
                return this.plugins;
            }
        }

        public Manager(string folder)
        {
            this.folder = folder;
        }

        public virtual void LoadAvailablePlugins()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                plugins = new Dictionary<String, Plugin>();
                var serializer = new YamlSerializer();
                foreach (string file in store.GetFileNames("*.yml"))
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var plugin = serializer.Deserialize(stream, typeof(Plugin))[0];
                        plugins.Add(Path.GetFileNameWithoutExtension(file), (Plugin)plugin);
                    }
                }
            }
        }
    }
}

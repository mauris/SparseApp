using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
            plugins = new Dictionary<String, Plugin>();
            var serializer = new YamlSerializer();
            foreach (string file in Directory.EnumerateFiles(folder, "*.yml"))
            {
                var plugin = serializer.DeserializeFromFile(file, typeof(Plugin))[0];
                plugins.Add(Path.GetFileNameWithoutExtension(file), (Plugin)plugin);
            }
        }
    }
}

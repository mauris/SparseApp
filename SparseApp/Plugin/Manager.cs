using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Yaml.Serialization;

namespace SparseApp.Plugin
{
    public class Manager
    {
        protected string folder;

        protected List<Plugin> plugins = new List<Plugin>();

        public List<Plugin> Plugins
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

        public virtual void LoadPlugins()
        {
            plugins = new List<Plugin>();
            var serializer = new YamlSerializer();
            foreach (string file in Directory.EnumerateFiles(folder, "*.yml"))
            {
                var plugin = serializer.DeserializeFromFile(file, typeof(Plugin))[0];
                plugins.Add((Plugin)plugin);
            }
        }
    }
}

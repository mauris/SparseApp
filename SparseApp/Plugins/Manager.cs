﻿using System;
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
        protected Dictionary<String, Plugin> plugins = new Dictionary<String, Plugin>();

        public Dictionary<String, Plugin> Plugins
        {
            get
            {
                return this.plugins;
            }
        }

        public Manager()
        {
        }

        public virtual void AddPlugin(string name, Plugin plugin)
        {
            plugins.Add(name, plugin);
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                var serializer = new YamlSerializer();
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(name + ".yml", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    serializer.Serialize(stream, plugin);
                }
            }
        }

        public virtual void RemovePlugin(string name)
        {
            plugins.Remove(name);
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                store.DeleteFile(name + ".yml");
            }
        }

        public virtual void LoadAvailablePlugins()
        {
            plugins = new Dictionary<String, Plugin>();
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
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

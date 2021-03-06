﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Plugins
{
    public interface IPluginManager
    {
        Dictionary<String, IPlugin> Plugins { get; }

        void ImportFile(string file);

        void AddPlugin(string name, IPlugin plugin);

        void RemovePlugin(string name);

        List<string> LoadPlugins();
    }
}

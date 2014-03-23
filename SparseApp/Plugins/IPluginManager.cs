using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Plugins
{
    interface IPluginManager
    {
        public void AddPlugin(string name, Plugin plugin);

        public void RemovePlugin(string name);

        public void LoadPlugins();
    }
}

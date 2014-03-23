using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Plugins
{
    public interface IPluginManager
    {
        void AddPlugin(string name, Plugin plugin);

        void RemovePlugin(string name);

        void LoadPlugins();
    }
}

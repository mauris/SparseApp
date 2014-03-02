using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}

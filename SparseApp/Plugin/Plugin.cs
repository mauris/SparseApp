using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Plugin
{
    public class Plugin
    {
        public bool RunOnStart { get; set; }

        public bool RunOnRepoLoad { get; set; }

        public string Name { get; set; }

        public string Command { get; set; }
    }
}

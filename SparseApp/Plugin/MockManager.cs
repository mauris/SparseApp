using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Plugin
{
    public class MockManager : Manager
    {
        public MockManager(string folder)
            : base("")
        {
        }

        public override void LoadAvailablePlugins()
        {
        }
    }
}

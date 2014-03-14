using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Plugins
{
    public class MockManager : Manager
    {
        public MockManager(string folder)
            : base("")
        {
            plugins.Add("Composer Install", new Plugin { Command = "composer install", Name = "Composer Install" });
            plugins.Add("Composer Update", new Plugin { Command = "composer update", Name = "Composer Update" });
            plugins.Add("git pull", new Plugin { Command = "git pull", Name = "git pull" });
            plugins.Add("PHPUnit", new Plugin { Command = "phpunit -c test", Name = "PHPUnit" });
        }

        public override void LoadAvailablePlugins()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Repositories
{
    public class MockManager : Manager
    {
        public MockManager()
            : base()
        {
            repositories = new List<Repository>();

            repositories.Add(
                new Repository
                {
                    Path = @"C:\Users\Sam\Documents\GitHub\router",
                    Plugins = new List<string> { "Composer Update", "Composer Install", "git pull", "PHPUnit" }
                }
            );

            repositories.Add(
                new Repository
                {
                    Path = @"C:\Users\Sam\Documents\GitHub\config",
                    Plugins = new List<string> { "Composer Update", "Composer Install", "git pull", "PHPUnit" }
                }
            );

            repositories.Add(
                new Repository
                {
                    Path = @"C:\Users\Sam\Documents\GitHub\packfire-framework",
                    Plugins = new List<string> { "Composer Update", "Composer Install", "git pull", "PHPUnit" }
                }
            );
        }

        public override void LoadRepositories()
        {
        }

        public override void SaveRepositories()
        {
        }
    }
}

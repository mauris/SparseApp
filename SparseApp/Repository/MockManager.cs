using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Repository
{
    public class MockManager : Manager
    {
        public MockManager()
            : base("")
        {
            repositories = new List<Repository>();
            repositories.Add(new Repository { Path = @"C:\Users\Sam\Documents\GitHub\router" });
            repositories.Add(new Repository { Path = @"C:\Users\Sam\Documents\GitHub\config" });
            repositories.Add(new Repository { Path = @"C:\Users\Sam\Documents\GitHub\packfire-framework" });
            repositories.Add(new Repository { Path = @"C:\Users\Sam\Documents\GitHub\cdac-rms" });
        }

        public override void LoadRepositories()
        {
        }

        public override void SaveRepositories()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ProtoBuf;

namespace SparseApp.Repository
{
    class Manager
    {
        protected string configFile;

        protected List<Repository> repositories = new List<Repository>();

        public Manager(string file)
        {
            this.configFile = file;
        }

        public void LoadRepositories()
        {
            using (var file = File.OpenRead(this.configFile))
            {
                repositories = Serializer.Deserialize<List<Repository>>(file);
            }
        }

        public void SaveRepositories()
        {
            using (var file = File.Create(this.configFile))
            {
                Serializer.Serialize(file, repositories);
            }
        }
    }
}

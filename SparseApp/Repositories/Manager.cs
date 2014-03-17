using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using ProtoBuf;

namespace SparseApp.Repositories
{
    public class Manager
    {
        protected List<Repository> repositories = new List<Repository>();

        protected const string filename = "repositories";

        public List<Repository> Repositories
        {
            get
            {
                return this.repositories;
            }
        }

        public Manager()
        {
        }

        public virtual void LoadRepositories()
        {
            repositories = new List<Repository>();
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                if (store.FileExists(filename))
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        repositories = Serializer.Deserialize<List<Repository>>(stream);
                    }
                }
            }
        }

        public virtual void SaveRepositories()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    Serializer.Serialize(stream, repositories);
                }
            }
        }
    }
}

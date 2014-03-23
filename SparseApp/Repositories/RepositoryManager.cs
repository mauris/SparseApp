using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;
using ProtoBuf;

namespace SparseApp.Repositories
{
    public class RepositoryManager: IRepositoryManager
    {
        protected List<IRepository> repositories = new List<IRepository>();

        protected const string filename = "repositories";

        public List<IRepository> Repositories
        {
            get
            {
                return this.repositories;
            }
        }

        public RepositoryManager()
        {
        }

        public virtual void LoadRepositories()
        {
            repositories = new List<IRepository>();
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (store.FileExists(filename))
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.Open, FileAccess.Read, store))
                    {
                        repositories = Serializer.Deserialize<List<IRepository>>(stream);
                    }
                }
                store.Close();
            }
        }

        public virtual void SaveRepositories()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (store.FileExists(filename))
                {
                    store.DeleteFile(filename);
                }
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.CreateNew, FileAccess.Write, store))
                {
                    Serializer.Serialize(stream, repositories);
                    stream.Close();
                }
                store.Close();
            }
        }
    }
}

﻿using System;
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
        protected List<Repository> repositories = new List<Repository>();

        protected const string filename = "repositories";

        public List<Repository> Repositories
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
            repositories = new List<Repository>();
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (store.FileExists(filename))
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.Open, FileAccess.Read, store))
                    {
                        repositories = Serializer.Deserialize<List<Repository>>(stream);
                    }
                }
                store.Close();
            }
        }

        public virtual void SaveRepositories()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (store.FileExists(filename + ".bak"))
                {
                    store.DeleteFile(filename + ".bak");
                }
                if (store.FileExists(filename))
                {
                    store.CopyFile(filename, filename + ".bak");
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Repositories
{
    public interface IRepositoryManager
    {
        List<IRepository> Repositories { get; }

        void LoadRepositories();

        void SaveRepositories();
    }
}

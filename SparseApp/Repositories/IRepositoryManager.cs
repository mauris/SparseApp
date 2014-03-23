using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Repositories
{
    public interface IRepositoryManager
    {
        List<Repository> Repositories { get; }

        void LoadRepositories();

        void SaveRepositories();
    }
}

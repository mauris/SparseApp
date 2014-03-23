using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Repositories
{
    interface IRepositoryManager
    {
        void LoadRepositories();

        void SaveRepositories();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Repositories
{
    public interface IRepository
    {
        string Path { get; set; }

        List<string> Plugins { get; set; }

        string Basename { get; }
    }
}

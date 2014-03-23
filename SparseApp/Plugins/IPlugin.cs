using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparseApp.Plugins
{
    public interface IPlugin
    {
        string Name { get; set; }

        void Run(string path);

        string GetOutput();

        bool IsRunning();

        void Halt();
    }
}

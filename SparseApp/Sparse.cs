using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SparseApp
{
    class Sparse
    {
        [STAThread]
        public static void Main(string[] args)
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}

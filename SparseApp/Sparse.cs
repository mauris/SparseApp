using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Mono.Options;
using System.IO.IsolatedStorage;

namespace SparseApp
{
    class Sparse
    {
        [STAThread]
        public static void Main(string[] args)
        {
            bool cleanup = false;
            OptionSet set = new OptionSet()
            {
               {
                   "cleanup", "Clean up the isolated storage used by Sparse when uninstlaling", v => cleanup = v != null
               }
            };

            if (cleanup)
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    store.Remove();
                }
            }
            else
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}

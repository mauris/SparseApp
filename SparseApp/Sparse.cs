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
            bool reset = false;
            OptionSet set = new OptionSet()
                .Add("reset", v => reset = v != null);

            try
            {
                set.Parse(args);
            }
            catch (OptionException)
            {
            }

            if (reset)
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    store.Remove();
                }
                Console.WriteLine("Sparse has been reset successfully.");
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

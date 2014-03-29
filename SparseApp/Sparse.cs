using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Mono.Options;
using System.IO.IsolatedStorage;
using Ninject;

namespace SparseApp
{
    class Sparse
    {
        [STAThread]
        public static void Main(string[] args)
        {
            bool reset = false;
            bool silentReset = false;
            OptionSet set = new OptionSet()
                .Add("reset", v => reset = v != null)
                .Add("silent", v => silentReset = v != null);

            try
            {
                set.Parse(args);
            }
            catch (OptionException)
            {
            }

            if (reset)
            {
                ResetSparse(silentReset);
            }
            else
            {
                StandardKernel kernel = new StandardKernel(new DefaultModule());
                App app = kernel.Get<App>();
                app.InitializeComponent();
                app.Run();
            }
        }

        public static void ResetSparse(bool isSilent)
        {
            if (!isSilent)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to reset Sparse and remove all repositories and plugins?\nWarning: This action is irreverisble.", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                store.Remove();
            }

            if (!isSilent)
            {
                MessageBox.Show("Sparse has been reset to its original state.", "Reset successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

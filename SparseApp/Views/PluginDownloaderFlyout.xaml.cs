using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using System.Threading;
using System.Net;
using System.IO;
using SparseApp.Plugins;

namespace SparseApp.Views
{
    /// <summary>
    /// Interaction logic for PluginRepoInstallFlyout.xaml
    /// </summary>
    public partial class PluginDownloaderFlyout : Flyout
    {
        protected Thread searchAction;

        private Github repository = new Github();

        public PluginDownloaderFlyout()
        {
            InitializeComponent();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (searchAction != null)
            {
                searchAction.Abort();
                searchAction = null;
            }
            searchAction = new Thread(new ThreadStart(Search));
            searchAction.IsBackground = true;
            searchAction.Start();
        }

        private void Search()
        {
            try
            {
                Thread.Sleep(200);
                string term = "";
                Dispatcher.Invoke(new Action(() => term = txtSearch.Text));

                List<PluginFile> files = repository.Search(term);
                Dispatcher.Invoke(new Action(() => lstSearchResult.DataContext = files));
            }
            catch (ThreadAbortException)
            {

            }
        }
    }
}

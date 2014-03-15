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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RepositoryManager = SparseApp.Repositories.MockManager;
using PluginManager = SparseApp.Plugins.MockManager;
using SparseApp.Repositories;
using SparseApp.Plugins;
using System.Threading;
using System.Windows.Threading;

namespace SparseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        protected RepositoryManager repo;

        protected PluginManager plugins;

        public MainWindow()
        {
            InitializeComponent();

            plugins = new PluginManager("");
            plugins.LoadAvailablePlugins();

            repo = new RepositoryManager("");
            repo.LoadRepositories();
            lstRepositories.DataContext = repo.Repositories;
        }

        private void lstRepositories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Repository repository = (Repository)lstRepositories.SelectedItem;

            List<Plugin> values = plugins.Plugins.Where(item => repository.Plugins.Contains(item.Key)).Select(item => item.Value).ToList<Plugin>();
            lstPlugins.DataContext = values;
            txtPluginOutput.Text = "";
        }

        private void lstPlugins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstPlugins.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                Plugin plugin = (Plugin)lstPlugins.SelectedItem;

                ThreadStart start = delegate()
                {
                    plugin.Run(repository.Path);
                    while (plugin.IsRunning)
                    {
                        Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            new Action(() => txtPluginOutput.Text = plugin.Output)
                        );
                        Thread.Sleep(100);
                    }
                };
                new Thread(start).Start();
            }
        }

        private void lstPlugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPlugins.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                Plugin plugin = (Plugin)lstPlugins.SelectedItem;
                txtPluginOutput.Text = "";
            }
        }
    }
}

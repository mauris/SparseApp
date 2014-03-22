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
using Ookii.Dialogs.Wpf;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;

namespace SparseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        protected RepositoryManager repo;

        protected PluginManager plugins;

        protected Thread ConsoleUpdatingThread;

        public MainWindow()
        {
            InitializeComponent();

            plugins = new PluginManager();
            plugins.LoadAvailablePlugins();
            lstAvailablePlugins.DataContext = plugins.Plugins;

            repo = new RepositoryManager();
            repo.LoadRepositories();
            lstRepositories.DataContext = repo.Repositories;
            txtStatus.Text = "Select a repository";

            SetWelcomeText();
        }

        private void SetWelcomeText()
        {
            txtPluginOutput.Text = @"Welcome to Sparse.
   ____                     
  / __/__  ___ ________ ___ 
 _\ \/ _ \/ _ `/ __(_-</ -_)
/___/ .__/\_,_/_/ /___/\__/ 
   /_/                      

When plugin runs, the output will be shown here.

You have " + repo.Repositories.Count + " " + (repo.Repositories.Count > 1 ? "repositories" : "repository") + " registered and "
           + plugins.Plugins.Count + " " + (plugins.Plugins.Count > 1 ? "plugins" : "plugin") + " available.";
        }

        private void lstRepositories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRepositories.SelectedItem == null)
            {
                ((VisualBrush)icoPluginIndicator.OpacityMask).Visual = (Visual)FindResource("appbar_arrow_left");
                txtPluginStatus.Text = "Select a repository on the left to get started.";
                lstPlugins.DataContext = new List<Plugin>();
                pnlPluginInfo.Visibility = System.Windows.Visibility.Visible;
                pnlPluginActions.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;

                pnlPluginInfo.Visibility = System.Windows.Visibility.Collapsed;
                pnlPluginActions.Visibility = System.Windows.Visibility.Visible;

                List<Plugin> values = plugins.Plugins.Where(item => repository.Plugins.Contains(item.Key)).Select(item => item.Value).ToList<Plugin>();
                lstPlugins.DataContext = values;

                txtPluginOutput.Text = "";
                txtStatus.Text = "Select a plugin";
                lstPlugins.SelectedIndex = -1;

                if (values.Count == 0)
                {
                    ((VisualBrush)icoPluginIndicator.OpacityMask).Visual = (Visual)FindResource("appbar_puzzle");
                    txtPluginStatus.Text = "This repository has no plugins installed.";
                    pnlPluginInfo.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void lstPlugins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstRepositories.SelectedItem != null && lstPlugins.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                Plugin plugin = (Plugin)lstPlugins.SelectedItem;

                plugin.Run(repository.Path);

                if (this.Width == 480)
                {
                    flyOutput.IsOpen = true;
                }

                RunConsoleUpdating(plugin);
            }
        }

        private void lstPlugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRepositories.SelectedItem != null && lstPlugins.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                Plugin plugin = (Plugin)lstPlugins.SelectedItem;

                RunConsoleUpdating(plugin);
            }
        }

        private void RunConsoleUpdating(Plugin plugin)
        {
            if (ConsoleUpdatingThread != null)
            {
                ConsoleUpdatingThread.Abort();
            }

            txtStatus.Text = "Running...";
            prgProgress.IsActive = true;
            ThreadStart start = delegate()
            {
                while (plugin.IsRunning)
                {
                    Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        new Action(() => txtPluginOutput.Text = plugin.Output)
                    );
                    Thread.Sleep(100);
                }
                Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => txtPluginOutput.Text = plugin.Output)
                );
                Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => txtStatus.Text = "Ready")
                );
                Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => prgProgress.IsActive = false)
                );
            };
            ConsoleUpdatingThread = new Thread(start);
            ConsoleUpdatingThread.Start();
        }

        private void MetroWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<string, Plugin> item in plugins.Plugins)
            {
                item.Value.Halt();
            }
            repo.SaveRepositories();
        }

        private void btnAddRepository_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            var result = dialog.ShowDialog(this);

            if (result == true)
            {
                Repository repository = new Repository()
                {
                    Path = dialog.SelectedPath
                };

                repo.Repositories.Add(repository);
                lstRepositories.Items.Refresh();
            }
        }

        private async void btnUninstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (lstRepositories.SelectedItem != null && lstPlugins.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                Plugin plugin = (Plugin)lstPlugins.SelectedItem;

                var result = await this.ShowMessageAsync("Uninstall Plugin \"" + plugin.Name + "\" from repository", "Are you sure you want to uninstall plugin \"" + plugin.Name + "\" from repository \"" + repository.Basename + "\"?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Yes", NegativeButtonText = "No" });

                if (result == MessageDialogResult.Affirmative)
                {
                    string key = plugins.Plugins.Where(pair => (plugin == pair.Value)).Select(pair => pair.Key).FirstOrDefault();
                    repository.Plugins.RemoveAll(item => (item == key));

                    lstRepositories_SelectionChanged(sender, null);
                }
            }
        }

        private void btnRunPlugin_Click(object sender, RoutedEventArgs e)
        {
            lstPlugins_MouseDoubleClick(sender, null);
        }

        private void mnuRepositoryOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (lstRepositories.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                Process.Start(repository.Path);
            }
        }

        private void mnuRepositoryRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lstRepositories.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                repo.Repositories.Remove(repository);
                lstRepositories.Items.Refresh();
            }
        }

        private void mnuPluginRun_Click(object sender, RoutedEventArgs e)
        {
            lstPlugins_MouseDoubleClick(sender, null);
        }

        private void mnuPluginUninstall_Click(object sender, RoutedEventArgs e)
        {
            btnUninstallPlugin_Click(sender, e);
        }

        private void btnInstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            flyPluginManager.IsOpen = true;
        }

        private void btnFlyInstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (lstAvailablePlugins.SelectedItem != null && lstRepositories.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                KeyValuePair<string, Plugin> entry = (KeyValuePair<string, Plugin>)lstAvailablePlugins.SelectedItem;
                if (repository.Plugins.Count(item => item == entry.Key) == 0)
                {
                    repository.Plugins.Add(entry.Key);

                    List<Plugin> values = plugins.Plugins.Where(item => repository.Plugins.Contains(item.Key)).Select(item => item.Value).ToList<Plugin>();
                    lstPlugins.DataContext = values;
                }
                else
                {
                    this.ShowMessageAsync("Plugin already installed", "The plugin \"" + entry.Value.Name + "\" has already been installed to the repository.", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "OK" });
                }
            }
        }

        private async void btnFlyRemovePlugin_Click(object sender, RoutedEventArgs e)
        {
            if (lstAvailablePlugins.SelectedItem != null)
            {
                KeyValuePair<string, Plugin> entry = (KeyValuePair<string, Plugin>)lstAvailablePlugins.SelectedItem;
                List<Repository> inUseRepos = new List<Repository>();

                // do check for repositories that are using this plugin
                foreach (Repository repository in repo.Repositories)
                {
                    if (repository.Plugins.Contains(entry.Key))
                    {
                        inUseRepos.Add(repository);
                    }
                }

                string message = "Are you sure you want to totally remove plugin \"" + entry.Value.Name + "\" from Sparse?";
                if (inUseRepos.Count > 0)
                {
                    message += "\n\nWarning: This plugin has been installed to " + inUseRepos.Count + " " + (inUseRepos.Count > 1 ? "repositories" : "repository") + ". Removing the plugin will uninstall it from these repositories";
                }

                var result = await this.ShowMessageAsync("Remove Plugin \"" + entry.Value.Name + "\" from Sparse", message, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Yes", NegativeButtonText = "No" });

                if (result == MessageDialogResult.Affirmative)
                {
                    // uninstall the plugin from all repos
                    foreach (Repository repository in inUseRepos)
                    {
                        repository.Plugins.RemoveAll(item => item == entry.Key);
                    }
                    plugins.RemovePlugin(entry.Key); // remove plugin
                    lstAvailablePlugins.Items.Refresh(); // refresh view

                    // refresh plugins for current repo
                    Repository currentRepo = (Repository)lstRepositories.SelectedItem;
                    List<Plugin> values = plugins.Plugins.Where(item => currentRepo.Plugins.Contains(item.Key)).Select(item => item.Value).ToList<Plugin>();
                    lstPlugins.DataContext = values;
                }
            }
        }

        private void btnAddPlugin_Click(object sender, RoutedEventArgs e)
        {
            flyPluginInstaller.IsOpen = true;
        }
    }
}

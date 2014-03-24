﻿using System;
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
using SparseApp.Repositories;
using SparseApp.Plugins;
using System.Threading;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SparseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        protected IRepositoryManager repositoryManager;

        protected IPluginManager pluginManager;

        protected Thread ConsoleUpdatingThread;

        public MainWindow()
        {
            InitializeComponent();
            pluginManager = ((App)App.Current).PluginManager;
            repositoryManager = ((App)App.Current).RepositoryManager;

            pluginManager.LoadPlugins();
            repositoryManager.LoadRepositories();

            lstAvailablePlugins.DataContext = pluginManager.Plugins;
            lstRepositories.DataContext = repositoryManager.Repositories;
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

You have " + (repositoryManager.Repositories.Count == 0 ? "no" : repositoryManager.Repositories.Count.ToString()) + " " + (repositoryManager.Repositories.Count == 1 ? "repository" : "repositories") + " registered and "
           + (pluginManager.Plugins.Count == 0 ? "no" : pluginManager.Plugins.Count.ToString()) + " " + (pluginManager.Plugins.Count == 1 ? "plugin" : "plugins") + " available.";
        }

        private void lstRepositories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRepositories.SelectedItem == null)
            {
                ((VisualBrush)icoPluginIndicator.OpacityMask).Visual = (Visual)FindResource("appbar_arrow_left");
                txtPluginStatus.Text = "Select a repository on the left to get started.";
                lstPlugins.DataContext = new List<Plugin>();
                pnlPluginInfo.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;

                txtPluginOutput.Text = "";
                txtStatus.Text = "Select a plugin";
                lstPlugins.SelectedIndex = -1;

                RefreshPluginsForRepository(repository);
            }
        }

        private void lstPlugins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstRepositories.SelectedItem != null && lstPlugins.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                IPlugin plugin = (IPlugin)lstPlugins.SelectedItem;

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
                IPlugin plugin = (IPlugin)lstPlugins.SelectedItem;

                RunConsoleUpdating(plugin);
            }
        }

        private void RunConsoleUpdating(IPlugin plugin)
        {
            if (ConsoleUpdatingThread != null)
            {
                ConsoleUpdatingThread.Abort();
            }

            txtStatus.Text = "Running...";
            prgProgress.IsActive = true;
            ThreadStart start = delegate()
            {
                while (plugin.IsRunning())
                {
                    Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        new Action(() => txtPluginOutput.Text = plugin.GetOutput())
                    );
                    Thread.Sleep(100);
                }
                Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => txtPluginOutput.Text = plugin.GetOutput())
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

        private void btnAddRepository_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            var result = dialog.ShowDialog(this);

            if (result == true)
            {
                if (repositoryManager.Repositories.Where(repository => repository.Path == dialog.SelectedPath).Count() == 0)
                {
                    Repository repository = new Repository()
                    {
                        Path = dialog.SelectedPath
                    };

                    repositoryManager.Repositories.Add(repository);
                    lstRepositories.Items.Refresh();
                }
                else
                {
                    this.ShowMessageAsync("Repository already exists", "The selected folder \"" + dialog.SelectedPath + "\" has already been registered in Sparse.", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "OK" });
                }
            }
        }

        private async void btnUninstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (lstRepositories.SelectedItem != null && lstPlugins.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                IPlugin plugin = (IPlugin)lstPlugins.SelectedItem;

                var result = await this.ShowMessageAsync("Uninstall Plugin \"" + plugin.Name + "\" from repository", "Are you sure you want to uninstall plugin \"" + plugin.Name + "\" from repository \"" + repository.Basename + "\"?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Yes", NegativeButtonText = "No" });

                if (result == MessageDialogResult.Affirmative)
                {
                    string key = pluginManager.Plugins.Where(pair => (plugin == pair.Value)).Select(pair => pair.Key).FirstOrDefault();
                    repository.Plugins.RemoveAll(item => (item == key));

                    RefreshPluginsForRepository(repository);
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
                repositoryManager.Repositories.Remove(repository);
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

        private void btnManagePlugins_Click(object sender, RoutedEventArgs e)
        {
            flyPluginManager.IsOpen = true;
        }

        private void btnFlyInstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (lstAvailablePlugins.SelectedItem != null && lstRepositories.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                KeyValuePair<string, IPlugin> entry = (KeyValuePair<string, IPlugin>)lstAvailablePlugins.SelectedItem;
                if (repository.Plugins.Count(item => item == entry.Key) == 0)
                {
                    repository.Plugins.Add(entry.Key);

                    List<IPlugin> values = pluginManager.Plugins.Where(item => repository.Plugins.Contains(item.Key)).Select(item => item.Value).ToList<IPlugin>();
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
                KeyValuePair<string, IPlugin> entry = (KeyValuePair<string, IPlugin>)lstAvailablePlugins.SelectedItem;
                List<Repository> inUseRepos = new List<Repository>();

                // do check for repositories that are using this plugin
                foreach (Repository repository in repositoryManager.Repositories)
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
                    pluginManager.RemovePlugin(entry.Key); // remove plugin
                    lstAvailablePlugins.Items.Refresh(); // refresh view

                    // refresh plugins for current repo
                    Repository currentRepo = (Repository)lstRepositories.SelectedItem;
                    List<IPlugin> values = pluginManager.Plugins.Where(item => currentRepo.Plugins.Contains(item.Key)).Select(item => item.Value).ToList<IPlugin>();
                    lstPlugins.DataContext = values;
                }
            }
        }

        private void btnAddPlugin_Click(object sender, RoutedEventArgs e)
        {
            flyPluginInstaller.IsOpen = true;
        }

        private void btnFormAddPlugin_Click(object sender, RoutedEventArgs e)
        {
            bool validate = true;

            if (txtPluginAddName.Text.Trim() == "")
            {
                validate = false;
            }

            if (txtPluginAddCommand.Text.Trim() == "")
            {
                validate = false;
            }

            if (validate)
            {
                Plugin plugin = new Plugin()
                {
                    Name = txtPluginAddName.Text.Trim(),
                    Command = txtPluginAddCommand.Text.Trim()
                };

                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                string filename = r.Replace(plugin.Name, "");

                pluginManager.AddPlugin(filename, plugin);
                lstAvailablePlugins.Items.Refresh();

                btnFormAddPluginCancel_Click(sender, e);
            }
        }

        private void btnFormAddPluginCancel_Click(object sender, RoutedEventArgs e)
        {
            flyPluginInstaller.IsOpen = false;
            txtPluginAddCommand.Text = "";
            txtPluginAddName.Text = "";
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (KeyValuePair<string, IPlugin> item in pluginManager.Plugins)
            {
                item.Value.Halt();
            }
            repositoryManager.SaveRepositories();
        }

        private void lstAvailablePlugins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstAvailablePlugins.SelectedItem != null && lstRepositories.SelectedItem != null)
            {
                Repository repository = (Repository)lstRepositories.SelectedItem;
                KeyValuePair<string, IPlugin> entry = (KeyValuePair<string, IPlugin>)lstAvailablePlugins.SelectedItem;
                if (repository.Plugins.Count(item => item == entry.Key) == 0)
                {
                    repository.Plugins.Add(entry.Key);
                }
                else
                {
                    repository.Plugins.RemoveAll(item => item == entry.Key);
                }

                RefreshPluginsForRepository(repository);
            }
        }

        private void RefreshPluginsForRepository(Repository repository)
        {
            List<IPlugin> values = pluginManager.Plugins.Where(item => repository.Plugins.Contains(item.Key)).Select(item => item.Value).ToList<IPlugin>();
            lstPlugins.DataContext = values;

            if (values.Count > 0)
            {
                pnlPluginInfo.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ((VisualBrush)icoPluginIndicator.OpacityMask).Visual = (Visual)FindResource("appbar_puzzle");
                txtPluginStatus.Text = "This repository has no plugins installed.";
                pnlPluginInfo.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void btnTools_Click(object sender, RoutedEventArgs e)
        {
            btnTools.ContextMenu.PlacementTarget = btnTools;
            btnTools.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            btnTools.ContextMenu.IsOpen = true;
        }

        private void btnAddPluginDropDown_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            btnAddPlugin.ContextMenu.PlacementTarget = btnAddPlugin;
            btnAddPlugin.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            btnAddPlugin.ContextMenu.IsOpen = true;
        }

        private void ShowAboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            flyAbout.IsOpen = true;
        }

        private async void AddPluginFromFileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Title = "Import Sparse plugins from file...";
            dialog.Multiselect = true;
            bool? result = dialog.ShowDialog(this);
            if (result == true)
            {
                var controller = await this.ShowProgressAsync("Importing Plugins...", "Loading files for import...");
                controller.SetCancelable(true);

                int progress = 0;
                foreach (string file in dialog.FileNames)
                {
                    bool imported = false;
                    while (!imported)
                    {
                        if (controller.IsCanceled)
                        {
                            break;
                        }

                        try
                        {
                            controller.SetMessage("Importing " + file);
                            pluginManager.ImportFile(file);
                            imported = true;
                        }
                        catch
                        {
                        }
                        if (!imported)
                        {
                            await controller.CloseAsync();
                            MessageDialogResult messageResult = await this.ShowMessageAsync("Plugin Import Failed", "Sparse tried to import \"" + file + "\" as a plugin but the operation failed. Would you like to retry import, skip this file or cancel the rest of the import operation?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, new MetroDialogSettings() { AffirmativeButtonText = "Retry import", NegativeButtonText = "Skip this file", FirstAuxiliaryButtonText = "Cancel Import" });
                            switch (messageResult)
                            {
                                case MessageDialogResult.Affirmative:
                                    break;
                                case MessageDialogResult.Negative:
                                    imported = true;
                                    break;
                                case MessageDialogResult.FirstAuxiliary:
                                    return;
                            }
                            controller = await this.ShowProgressAsync("Importing Plugins...", "Loading files for import...");
                        }
                    }
                    ++progress;
                    controller.SetProgress(progress / dialog.FileNames.Count());
                }
                await controller.CloseAsync();
            }
        }
    }

    public class SelectedIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value != -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Value cannot be converted back lah deh!");
        }
    }
}

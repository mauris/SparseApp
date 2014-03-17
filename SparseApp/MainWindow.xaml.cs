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

        protected Thread ConsoleUpdatingThread;

        public MainWindow()
        {
            InitializeComponent();

            plugins = new PluginManager();
            plugins.LoadAvailablePlugins();

            repo = new RepositoryManager();
            repo.LoadRepositories();
            lstRepositories.DataContext = repo.Repositories;
            txtStatus.Text = "Select a repository";
        }

        private void lstRepositories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Repository repository = (Repository)lstRepositories.SelectedItem;

            List<Plugin> values = plugins.Plugins.Where(item => repository.Plugins.Contains(item.Key)).Select(item => item.Value).ToList<Plugin>();
            lstPlugins.DataContext = values;

            txtPluginOutput.Text = "";
            txtStatus.Text = "Select a plugin";
            lstPlugins.SelectedIndex = -1;
        }

        private void lstPlugins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstPlugins.SelectedItem != null)
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
            if (lstPlugins.SelectedItem != null)
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
        }
    }
}

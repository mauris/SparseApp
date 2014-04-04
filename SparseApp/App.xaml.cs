using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using SparseApp.Repositories;
using SparseApp.Plugins;
using Ninject;
using NLog;

namespace SparseApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [Inject]
        public Logger Logger { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            StandardKernel kernel = new StandardKernel(new DefaultModule());
            MainWindow mainWindow = kernel.Get<MainWindow>();
            mainWindow.Load();
            mainWindow.Show();
        }
    }
}

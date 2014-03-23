using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using SparseApp.Repositories;
using SparseApp.Plugins;

namespace SparseApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IRepositoryManager RepositoryManager { get; set; }

        public IPluginManager PluginManager { get; set; }

        public App()
            : base()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using SparseApp.Repositories;
using SparseApp.Plugins;
using Ninject;

namespace SparseApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [Inject]
        public IRepositoryManager RepositoryManager { get; set; }

        [Inject]
        public IPluginManager PluginManager { get; set; }

        public App()
            : base()
        {
        }
    }
}

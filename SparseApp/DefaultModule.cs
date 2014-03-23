using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using SparseApp.Plugins;
using SparseApp.Repositories;

namespace SparseApp
{
    class DefaultModule: NinjectModule 
    {
        public override void Load()
        {
            Bind<IPluginManager>().To<PluginManager>();
            Bind<IRepositoryManager>().To<RepositoryManager>();
        }
    }
}

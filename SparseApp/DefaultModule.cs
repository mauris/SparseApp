using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using SparseApp.Plugins;
using SparseApp.Repositories;
using NLog;

namespace SparseApp
{
    class DefaultModule: NinjectModule 
    {
        public override void Load()
        {
            Bind<MainWindow>().ToSelf();
            Bind<IPluginManager>().To<PluginManager>();
            Bind<IRepositoryManager>().To<RepositoryManager>();
            Bind<Logger>().ToMethod(
                delegate(Ninject.Activation.IContext ctx) {
                    return LogManager.GetCurrentClassLogger();
                }
            ).InSingletonScope();
        }
    }
}

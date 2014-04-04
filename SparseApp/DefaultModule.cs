using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using SparseApp.Plugins;
using SparseApp.Repositories;
using NLog;
using SparseApp.Updates;
using Ninject;
using Ninject.Activation;

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
                delegate(IContext ctx) {
                    return LogManager.GetCurrentClassLogger();
                }
            ).InSingletonScope();
            Bind<UpdateService>().ToSelf();
        }
    }
}

#Sparse
> Sparse is a UI tool for running repetitive CLI tools at a double click.

Discussion and Support at [![Gitter chat](https://badges.gitter.im/mauris/SparseApp.png)](https://gitter.im/mauris/SparseApp)

1. Register your repositories and working folders
2. Install plugins for your repositories
3. Double click on the plugin to run
4. ???
5. PROFIT!!!

We are at 95% progress!

##Screenshots

22 Mar 2014 screenshots.

![](http://i.imgur.com/Da0nR1Y.png)
![](http://i.imgur.com/FtK90n9.png)
![](http://i.imgur.com/k29sZav.png)

##Specs

Sparse is built with Microsoft .NET Framework 4.0 Full, InstallShield LE for Visual Studio 2013 and Microsoft Visual Studio Ultimate 2013.

##Libraries

Sparse uses the following open source libraries to achieve awesomeness:

- [MahApps.Metro](https://github.com/MahApps/MahApps.Metro) - Awesome WPF Metro styling
- [Mono.Options](http://www.nuget.org/packages/Mono.Options) - Arguments parsing
- [Microsoft Async](https://www.nuget.org/packages/Microsoft.Bcl.Async) - for using `async` and `await` in Microsoft .NET 4.0 Framework
- [Ninject](http://ninject.org/) - Ninja-styled IoC implementation and concrete object loading.
- [Ookii Dialogs for WPF](http://www.ookii.org/software/dialogs/) - Windows Vista+ styled common dialogs (for selecting repository folders)
- [protobuf-net](http://code.google.com/p/protobuf-net/) - Protocol Buffers serialization in .NET, for saving repositories information
- [YAMLSerializer for .NET](http://yamlserializer.codeplex.com/) - for serializing / deserializing plugins

## License

The project is released open source under the [New BSD License](http://opensource.org/licenses/BSD-3-Clause).
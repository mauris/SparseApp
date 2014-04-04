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
using MahApps.Metro.Controls;
using System.Reflection;
using SparseApp.Models;
using System.Yaml.Serialization;
using System.IO;
using Ninject;
using SparseApp.Updates;
using System.Threading;

namespace SparseApp.Views
{
    /// <summary>
    /// Interaction logic for AboutFlyout.xaml
    /// </summary>
    public partial class AboutFlyout : Flyout
    {
        private Thread updateThread;

        [Inject]
        public UpdateService Updater { get; set; }

        public AboutFlyout()
        {
            InitializeComponent();
        }

        private void LoadLicenses()
        {
            List<OpenSourceLicense> licenses = new List<OpenSourceLicense>();

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "MahApps.Metro",
                    Website = "http://mahapps.com/MahApps.Metro/",
                    License = SparseApp.Properties.Resources.LicenseMahAppsMetro
                }
            );

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "Mono.Options",
                    Website = "https://github.com/mono/mono/",
                    License = SparseApp.Properties.Resources.LicenseMonoOptions
                }
            );

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "Ninject",
                    Website = "http://ninject.org/",
                    License = SparseApp.Properties.Resources.LicenseNinject
                }
            );

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "Microsoft Async",
                    Website = "https://www.nuget.org/packages/Microsoft.Bcl.Async",
                    License = SparseApp.Properties.Resources.LicenseMicrosoftAsync
                }
            );

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "Ookii.Dialogs",
                    Website = "http://www.ookii.org/software/dialogs/",
                    License = SparseApp.Properties.Resources.LicenseOokiiDialogs
                }
            );

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "Protocol Buffers for .NET",
                    Website = "http://code.google.com/p/protobuf-net/",
                    License = SparseApp.Properties.Resources.LicenseProtocolBufNet
                }
            );

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "YamlSerializer",
                    Website = "http://yamlserializer.codeplex.com/",
                    License = SparseApp.Properties.Resources.LicenseYamlSerializer
                }
            );

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "Sparse",
                    Website = "http://github.com/mauris/sparseapp/",
                    License = SparseApp.Properties.Resources.LicenseSparseApp
                }
            );

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "NLog",
                    Website = "http://nlog-project.org/",
                    License = SparseApp.Properties.Resources.LicenseNLog
                }
            );

            licenses.Add(
                new OpenSourceLicense()
                {
                    Name = "Json.NET",
                    Website = "http://james.newtonking.com/json",
                    License = SparseApp.Properties.Resources.LicenseJsonNet
                }
            );

            lstLicenses.DataContext = licenses.OrderBy(item => item.Name).ToList();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void ResetSparseHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Sparse.ResetSparse(false);
        }

        private void OpenLogHyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/sparse.log"))
            {
                System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + "/sparse.log");
            }
            else
            {
                MessageBox.Show("Log file is empty. Surprisingly. ", "Log file not found", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateCheck(object version)
        {
            bool updateAvailable = Updater.Check(version.ToString());
            Dispatcher.Invoke(new Action(() => btnUpdateAvailable.Visibility = updateAvailable ? Visibility.Visible : Visibility.Collapsed));
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl origin = (TabControl)sender;
            if(origin.SelectedIndex == 2)
            {
                if (updateThread == null || !updateThread.IsAlive)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    updateThread = new Thread(new ParameterizedThreadStart(UpdateCheck));
                    updateThread.IsBackground = true;
                    updateThread.Start(assembly.GetName().Version.ToString());
                }
            }
        }

        private void Flyout_Initialized(object sender, EventArgs e)
        {
            ((App)App.Current).Kernel.Inject(this);
            Assembly assembly = Assembly.GetExecutingAssembly();
            txtAppVersion.Text = "version " + assembly.GetName().Version.ToString();
            LoadLicenses();
        }
    }
}

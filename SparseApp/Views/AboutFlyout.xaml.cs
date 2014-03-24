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

namespace SparseApp.Views
{
    /// <summary>
    /// Interaction logic for AboutFlyout.xaml
    /// </summary>
    public partial class AboutFlyout : Flyout
    {
        public AboutFlyout()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            txtAppVersion.Text = "version " + assembly.GetName().Version.ToString();
            LoadLicenses();
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

            lstLicenses.DataContext = licenses;
        }
    }
}

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
        }
    }
}

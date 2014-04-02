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
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace SparseApp.Views
{
    /// <summary>
    /// Interaction logic for PluginRepoInstallFlyout.xaml
    /// </summary>
    public partial class PluginDownloaderFlyout : Flyout
    {
        protected Thread searchAction;

        public PluginDownloaderFlyout()
        {
            InitializeComponent();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (searchAction != null)
            {
                searchAction.Abort();
                searchAction = null;
            }
            searchAction = new Thread(new ThreadStart(Search));
            searchAction.IsBackground = true;
            searchAction.Start();
        }

        private void Search()
        {
            try
            {
                Thread.Sleep(500);
                string term = "";
                Dispatcher.Invoke(new Action(() => term = txtSearch.Text));

                WebClient client = new WebClient();
                client.Headers.Add("Accept", "application/vnd.github.v3+json");
                client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0;" +
                   ".NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; InfoPath.2;" +
                   ".NET CLR 3.5.21022; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)");
                client.UseDefaultCredentials = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                string data = client.DownloadString(new Uri("https://api.github.com/repos/mauris/SparseApp.Plugins/git/trees/master"));
                JObject objects = JsonConvert.DeserializeObject<JObject>(data);
                List<PluginFile> plugins = objects["tree"]
                    .Where(item => ((string)item["path"]).EndsWith(".yml", StringComparison.InvariantCultureIgnoreCase))
                    .Select(item => new PluginFile()
                    {
                        Name = Path.GetFileNameWithoutExtension(item["path"].ToString()),
                        Url = item["url"].ToString(),
                        Size = item["size"].ToString(),
                        Hash = item["sha"].ToString()
                    })
                    .ToList<PluginFile>();
            }
            catch (ThreadAbortException)
            {

            }
        }
    }

    class PluginFile
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Size { get; set; }
        public string Hash { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SparseApp.Plugins
{
    class Github
    {
        protected List<PluginFile> files = new List<PluginFile>();

        protected DateTime? lastSearch = null;

        public List<PluginFile> Files {
            get {
                return files;
            }
        }

        public List<PluginFile> Search(string term)
        {
            if (!lastSearch.HasValue || DateTime.Compare(lastSearch.Value, DateTime.Now.AddHours(72)) > 0)
            {
                DownloadList();
            }

            return files.Where(item => item.Name.ToLower().Contains(term.ToLower())).ToList();
        }

        protected void DownloadList()
        {
            WebClient client = new WebClient();
            client.Headers.Add("Accept", "application/vnd.github.v3+json");
            client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0;" +
               ".NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; InfoPath.2;" +
               ".NET CLR 3.5.21022; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)");
            client.UseDefaultCredentials = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            string data = client.DownloadString(new Uri("https://api.github.com/repos/mauris/SparseApp.Plugins/git/trees/master"));
            JObject objects = JsonConvert.DeserializeObject<JObject>(data);
            files = objects["tree"]
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
    }

    class PluginFile
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Size { get; set; }
        public string Hash { get; set; }
    }
}

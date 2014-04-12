using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SparseApp.Updates
{
    public class UpdateService
    {
        private DateTime? lastCheck = null;

        protected const string url = "https://api.github.com/repos/mauris/sparseapp/releases";

        protected string releaseUrl = "";

        public string Release
        {
            get
            {
                return releaseUrl;
            }
        }

        public bool Check(string version)
        {
            if (!lastCheck.HasValue || DateTime.Compare(lastCheck.Value, DateTime.Now.AddMinutes(5)) > 0)
            {
                lastCheck = DateTime.Now;
                Fetch(version);
            }

            return releaseUrl != "";
        }

        private void Fetch(string version)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Accept", "application/vnd.github.v3+json");
            client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0;" +
               ".NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; InfoPath.2;" +
               ".NET CLR 3.5.21022; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)");
            client.UseDefaultCredentials = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            string data = client.DownloadString(new Uri(url));
            JArray objects = JsonConvert.DeserializeObject<JArray>(data);
            if (objects.Count > 0)
            {
                JToken release = objects
                    .Where(item => item["tag_name"].ToString() != version)
                    .First();
                if (release["assets"].Count() > 0)
                {
                    releaseUrl = release["assets"][0]["url"].ToString();
                }
            }
        }
    }
}

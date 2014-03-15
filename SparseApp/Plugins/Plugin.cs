using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SparseApp.Plugins
{
    public class Plugin
    {
        public bool RunOnStart { get; set; }

        public bool RunOnRepoLoad { get; set; }

        public string Name { get; set; }

        public string Command { get; set; }

        protected Process process;

        protected string output = "";

        public string Output
        {
            get
            {
                if (process != null)
                {
                    output += process.StandardOutput.ReadToEnd();
                }
                return output;
            }
        }

        public void Run(string path)
        {
            output = "";

            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + Command);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.WorkingDirectory = path;
            procStartInfo.CreateNoWindow = true;

            process = new Process();
            process.StartInfo = procStartInfo;
            process.Start();

            string result = process.StandardOutput.ReadToEnd();

            Console.WriteLine(result);
        }
    }
}

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
                return output;
            }
        }

        public bool IsRunning
        {
            get
            {
                if (process != null)
                {
                    try
                    {
                        Process.GetProcessById(process.Id);
                        return true;
                    }
                    catch (ArgumentException)
                    {
                    }
                }
                return false;
            }
        }

        public void Run(string path)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + Command);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.WorkingDirectory = path;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.RedirectStandardError = true;

            process = new Process();
            process.StartInfo = procStartInfo;

            process.OutputDataReceived += (sender, args) => output += args.Data;
            process.ErrorDataReceived += (sender, args) => output += args.Data;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
        }
    }
}

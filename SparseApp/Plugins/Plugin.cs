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

        protected bool isRunning = false;

        public bool IsRunning
        {
            get
            {
                return isRunning;
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

            output = "";

            if (process != null)
            {
                process.Kill();
            }

            process = new Process();
            process.StartInfo = procStartInfo;

            process.OutputDataReceived += (sender, args) => output += args.Data;
            process.ErrorDataReceived += (sender, args) => output += args.Data;

            isRunning = true;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.Exited += (sender, args) => isRunning = false;
        }

        public void Halt()
        {
            if (process != null)
            {
                try
                {
                    process.Kill();
                }
                catch
                {

                }
            }
        }
    }
}

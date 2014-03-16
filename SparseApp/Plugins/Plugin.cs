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
                try
                {
                    process.Kill();
                }
                catch
                {

                }
            }

            process = new Process();
            process.StartInfo = procStartInfo;

            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (sender, args) => output += args.Data + "\n";
            process.ErrorDataReceived += (sender, args) => output += args.Data + "\n";
            process.Exited += (sender, args) => isRunning = false;

            isRunning = true;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
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

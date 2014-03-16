﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

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

            if (isRunning)
            {
                process.Kill();
            }

            output = "";

            process = new Process();
            process.StartInfo = procStartInfo;

            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (sender, args) => output += processLine(args.Data);
            process.ErrorDataReceived += (sender, args) => output += processLine(args.Data);
            process.Exited += (sender, args) => isRunning = false;

            isRunning = true;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        protected string processLine(string text)
        {
            if (text != null)
            {
                return Regex.Replace(text, "\u001B\\[[;\\d]*(m|K)", "") + "\n";
            }
            return text;
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

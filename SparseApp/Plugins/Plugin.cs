using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Yaml.Serialization;

namespace SparseApp.Plugins
{
    public class Plugin
    {
        public string Name { get; set; }

        public string Command { get; set; }

        protected Process process;

        protected string output = "";

        [YamlSerialize(YamlSerializeMethod.Never)]
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
            try
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
            catch (Exception ex)
            {
                output = "Exception occurred. Tried running the following command:\n\n> " + Command + "\nat " + path + "\n\nPlease review the following exception message:\n\n" + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace;
                isRunning = false;
            }
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

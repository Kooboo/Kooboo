//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Kooboo.App.Upgrade
{
    public class CmdHelper
    {
        public static void StartDotnetApp(string path)
        {
            var cmd = string.Format("dotnet {0}", path);
            Excute(cmd);
        }
        public static string Excute(string cmd, bool waitForExit)
        {
            var process = Excute(cmd);
            string result = process.StandardOutput.ReadToEnd();

            if (!waitForExit)
            {
                process.WaitForExit();
            }

            return result;
        }

        private static Process Excute(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            return process;
        }
        
    }
}

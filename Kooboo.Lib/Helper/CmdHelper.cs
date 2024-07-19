//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Diagnostics;

namespace Kooboo.Lib.Helper
{
    public class CmdHelper
    {
        public static string ExeCmd(string cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;
            p.Start();

            p.StandardInput.WriteLine(cmd);
            p.StandardInput.WriteLine("exit");
            string strOutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return strOutput;
        }

        public static string ExeCmds(List<string> cmds, bool close)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;

            p.Start();

            foreach (var cmd in cmds)
            {
                p.StandardInput.WriteLine(cmd);
            }

            p.StandardInput.Close();

            var error = p.StandardError.ReadLine();
            if (!string.IsNullOrEmpty(error) || close)
            {
                //p.StandardInput.WriteLine("exit");
            }
            string strOutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return strOutput;
        }

        public static string ExeCmds(List<string> cmds, out Process p)
        {
            p = new Process();
            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;
            p.Start();

            foreach (var cmd in cmds)
            {
                p.StandardInput.WriteLine(cmd);
            }

            var error = p.StandardError.ReadLine();
            if (!string.IsNullOrEmpty(error))
            {
                p.StandardInput.WriteLine("exit");
            }
            string strOutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return strOutput;
        }

        public static void ExeCmdNoWait(string cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;
            p.Start();

            p.StandardInput.WriteLine(cmd);

        }

    }
}

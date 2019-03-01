//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Kooboo.Engines
{
    public class SassHelper
    {
        public static string Compile(string content,bool isSass=false)
        {
            var inputFilePath = Path.Combine(Path.GetFullPath("."), Guid.NewGuid().ToString() + (isSass?".sass":".scss"));
            File.WriteAllText(inputFilePath, content);

            var outputFilePath = Path.Combine(Path.GetFullPath("."), Guid.NewGuid().ToString() + ".css");

            var sasscmd = isSass ? "-a" : "";

            try
            {
                var command = string.Format("/usr/bin/sassc {2} {0} {1}", inputFilePath, outputFilePath, sasscmd);
                Process proc = new Process();
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \" " + command + " \"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();

                while (!proc.StandardOutput.EndOfStream)
                {
                    System.Threading.Thread.Sleep(10);
                }

                var output = File.ReadAllText(outputFilePath);

                return output;
            }
            catch(Exception ex)
            {

            }
            finally
            {
                if (File.Exists(inputFilePath))
                    File.Delete(inputFilePath);
                if (File.Exists(outputFilePath))
                    File.Delete(outputFilePath);
            }

            return string.Empty;
        }
    }
}

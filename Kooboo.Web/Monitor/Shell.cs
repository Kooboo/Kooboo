using System.Diagnostics;
using System.Text;

namespace Kooboo.Web.Monitor
{
    public class Shell
    {
        public static string Execute(string command, string args, string cwd)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            if (!string.IsNullOrWhiteSpace(args)) startInfo.Arguments = args;
            if (!string.IsNullOrWhiteSpace(cwd)) startInfo.WorkingDirectory = cwd;

            var process = new Process()
            {
                StartInfo = startInfo
            };

            process.Start();
            var stringBuilder = new StringBuilder();
            process.BeginOutputReadLine();

            process.OutputDataReceived += (s, e) =>
            {
                stringBuilder.AppendLine(e.Data);
            };

            process.WaitForExit();
            return stringBuilder.ToString();
        }
    }
}

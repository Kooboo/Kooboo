using System.Linq;

namespace Kooboo.Web.Monitor.Units
{
    public class Usage : IMonitor
    {
        public string Name => "Usage";

        record Process(int Pid, double Cpu, double Memory, string Cmd);

        public object GetValue()
        {
            var psResult = Shell.Execute("ps", "axo pid,pcpu,pmem,cmd", null);
            var records = psResult.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            var result = new List<Process>();

            foreach (var item in records)
            {
                var cells = item.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                int pid = 0;
                double cpu = 0;
                double memory = 0;
                string cmd = string.Empty;
                if (cells.Length > 0) int.TryParse(cells[0], out pid);
                if (cells.Length > 1) double.TryParse(cells[1], out cpu);
                if (cells.Length > 2) double.TryParse(cells[2], out memory);
                if (cells.Length > 3) cmd = cells[3];
                result.Add(new Process(pid, cpu, memory, cmd));
            }

            return result;
        }
    }
}

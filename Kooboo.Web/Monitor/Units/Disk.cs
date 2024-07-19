using System.Linq;

namespace Kooboo.Web.Monitor.Units
{
    public class Disk : IMonitor
    {
        public string Name => "Disk";

        public record Record(string Source, long Size, long Used, long Avail);
        public object GetValue()
        {
            var psResult = Shell.Execute("df", "-m / --output=source,size,used,avail", null);
            var records = psResult.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            var result = new List<Record>();

            foreach (var item in records)
            {
                var cells = item.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                string source = string.Empty;
                long size = 0;
                long used = 0;
                long avail = 0;
                if (cells.Length > 0) source = cells[0];
                if (cells.Length > 1) long.TryParse(cells[1], out size);
                if (cells.Length > 2) long.TryParse(cells[2], out used);
                if (cells.Length > 3) long.TryParse(cells[3], out avail);
                result.Add(new Record(source, size, used, avail));
            }

            return result;
        }
    }
}

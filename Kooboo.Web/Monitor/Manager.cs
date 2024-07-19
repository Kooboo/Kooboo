using System.Runtime.InteropServices;

namespace Kooboo.Web.Monitor
{
    public class Manager
    {

        static List<IMonitor> monitors = Lib.IOC.Service.GetInstances<IMonitor>();


        public static Dictionary<string, object> GetServerStatus()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new Exception("Server Monitor only support in linux server");
            }

            var result = new Dictionary<string, object>();

            foreach (var item in monitors)
            {
                if (!result.ContainsKey(item.Name))
                {
                    result.Add(item.Name, item.GetValue());
                }
            }

            return result;
        }
    }
}

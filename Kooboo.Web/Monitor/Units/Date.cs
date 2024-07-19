namespace Kooboo.Web.Monitor
{
    public class Date : IMonitor
    {
        public string Name => "Date";

        public object GetValue()
        {
            return new
            {
                UtcNow = DateTime.UtcNow,
                Zone = DateTimeOffset.Now.Offset
            };
        }
    }
}

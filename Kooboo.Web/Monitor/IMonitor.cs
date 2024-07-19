namespace Kooboo.Web.Monitor
{
    public interface IMonitor
    {
        public string Name { get; }
        public object GetValue();
    }
}

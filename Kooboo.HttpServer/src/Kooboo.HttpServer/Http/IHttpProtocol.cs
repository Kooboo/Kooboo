using System.Threading.Tasks;

namespace Kooboo.HttpServer.Http
{
    public interface IHttpProtocol
    {
        Task ProcessRequestsAsync();
    }
}
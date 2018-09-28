using System;
using Kooboo.Data.Context;
using Kooboo.HttpServer; 
using System.Threading.Tasks;

namespace Kooboo.Data.Server
{
   
    public class ServerHandler : IHttpHandler
    {
        public Func<RenderContext, Task> _handle;

        public ServerHandler(Func<RenderContext, Task> handle)
        {
            _handle = handle;
        }

        public async Task Handle(HttpContext context)
        {
            RenderContext renderContext = await WebServerContext.GetRenderContext(context);
            try
            {
                await _handle(renderContext);
                await WebServerContext.SetResponse(context, renderContext);
            }
            catch (Exception ex)
            {
                renderContext.Response.StatusCode = 500;
                renderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(ex.Message);
                await WebServerContext.SetResponse(context, renderContext);
            }
         
        }
    } 
}

using Kooboo.Data.Context;
using Kooboo.Data.Server;
using System.Threading.Tasks;

namespace Kooboo.Data.SSL
{
    public class SslCertMiddleWare : IKoobooMiddleWare
    {
        public IKoobooMiddleWare Next
        {
            get; set;
        }
        public async Task Invoke(RenderContext context)
        {
            /// http://your.domain.name/.well-known/acme-challenge/
            if (context.Request.RelativeUrl != null && context.Request.RelativeUrl.ToLower().StartsWith("/.well-known/acme-challenge"))
            {
                var host = context.Request.Host;
  
                string validate = context.Request.QueryString.Get("validate");

                if (string.IsNullOrWhiteSpace(validate))
                {
                    // this is for real... not for validation check. 
                    string token = SslService.GetToken(host);
                    if (!string.IsNullOrWhiteSpace(token))
                    { 
                        context.Response.Body = System.Text.Encoding.UTF8.GetBytes(token);
                        context.Response.ContentType = "text/html;charset=utf-8; ";
                        context.Response.End = true;
                    }
                }
                else
                {
                    var checkok =  SslService.Verify(host, validate);

                    if (checkok)
                    {
                        context.Response.Body = System.Text.Encoding.UTF8.GetBytes("[OK]");
                        context.Response.ContentType = "text/html;charset=utf-8; ";
                        context.Response.End = true;
                    }
                    else
                    {
                        context.Response.Body = System.Text.Encoding.UTF8.GetBytes("[error]");
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "text/html;charset=utf-8; ";
                        context.Response.End = true;
                    }
                } 
            }
            else
            {
                await Next.Invoke(context);
            }
        }


   
    }
}




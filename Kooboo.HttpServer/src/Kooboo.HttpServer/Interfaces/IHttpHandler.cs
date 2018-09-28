using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.HttpServer.Http;

namespace Kooboo.HttpServer
{
    public interface IHttpHandler
    {
        Task Handle(HttpContext context);
    }

    public class SampleHandler : IHttpHandler
    {
        public async Task Handle(HttpContext context)
        {
            //get form data
            //var ms = new System.IO.MemoryStream();
            //await context.Request.InputStream.CopyToAsync(ms);
            //var by = ms.ToArray();
            
            //var qs = context.Request.QueryString;
            //var cookies = context.Request.Cookies;

            //var cookie = new System.Net.Cookie("aa", "bb");
            //cookie.Expires = DateTime.Now.AddDays(1);
            //context.Response.AppendCookie(cookie);

            var response = "hello world";
            var bytes = Encoding.UTF8.GetBytes(response);
            //context.Response.Headers.HeaderContentLength=bytes.Length.ToString();
            //context.Response.Headers.HeaderContentType="text/plain";
            await context.Features.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            
            //using (var writer = new System.IO.StreamWriter(context.Features.Response.Body))
            //{
            //    foreach (var each in context.Features.Request.Headers)
            //    {
            //        writer.WriteLine(each.Key + ": " + each.Value.ToString());
            //    }
            //}
            //return Task.FromResult(false);
        }
    }
}

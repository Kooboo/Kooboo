//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;

namespace Kooboo.Web.Server
{
    public class TextLogMiddleware : IKoobooMiddleWare
    {
        StringBuilder sb = new StringBuilder();
        int counter = 0;
        DateTime lasttime = DateTime.Now;
        private int maxseconds { get; set; } = 30;

        private int maxcount { get; set; } = 100;

        public TextLogMiddleware(int CacheItemCount, int MaxTimeEclipseSeconds)
        {
            maxcount = CacheItemCount;
            maxseconds = MaxTimeEclipseSeconds;
        }


        public string GetFileName()
        {
            string basedir = AppDomain.CurrentDomain.BaseDirectory;
            basedir = System.IO.Path.Combine(basedir, "syslog");
            string filename = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            return System.IO.Path.Combine(basedir, filename);
        }

        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            string content = context.Request.IP + " " + context.Request.RawRelativeUrl + " " + context.Request.Body + "::::";

            foreach (var item in context.Request.QueryString.Keys)
            {
                content += item.ToString();
            }

            foreach (var item in context.Request.Forms)
            {
                content += item.ToString();
            }

            content += "\r\n\r\n";

            sb.Append(content);
            counter += 1;

            if (counter > maxcount || (DateTime.Now - lasttime).TotalSeconds > maxseconds)
            {
                counter = 0;
                lasttime = DateTime.Now;
                string log = sb.ToString();
                string filename = GetFileName();
                IOHelper.EnsureFileDirectoryExists(filename);
                System.IO.File.AppendAllText(filename, log);
                sb.Clear();
            }
            if (Next != null)
            {
                await Next.Invoke(context);
            }
        }
    }
}

using Kooboo.Data.Context;
using Kooboo.Data.Server;
using Kooboo.Lib.Compatible;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Kooboo.Web.Frontend
{
    class MonacoCacheMiddleware : IKoobooMiddleWare
    {

        static bool _isLoadding = false;
        readonly static string _monacoVersion = "1.0";

        public MonacoCacheMiddleware()
        {
            Task.Run(DownloadMonacoAsync);
        }

        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            if (context.Request.Path.ToLower().StartsWith("/_admin/scripts/lib/vs"))
            {
                if (_isLoadding) throw new Exception("resource is loadding");
                await DownloadMonacoAsync();
            }

            await Next.Invoke(context);
        }

        private static async Task DownloadMonacoAsync()
        {
            var dir = Path.Combine(Data.AppSettings.RootPath, "_Admin/Scripts/lib/");
            var extractDir = Path.Combine(dir, "vs");
            var fileName = dir + $"vs{_monacoVersion}.zip";
            var currentMonacoVersion = Data.AppSettings.MonacoVersion ?? "";
            if (currentMonacoVersion != _monacoVersion)
            {
                try
                {
                    _isLoadding = true;
                    var client = new System.Net.WebClient();
                    var uri = new Uri($"https://cdn.jsdelivr.net/gh/kooboo/monaco@master/vs{_monacoVersion}.zip");
                    var bytes = await client.DownloadDataTaskAsync(uri);

                    if (bytes != null)
                    {
                        if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                        IOHelper.EnsureFileDirectoryExists(dir);
                        if (bytes.Length > 0) File.WriteAllBytes(fileName, bytes);
                    }

                    ZipFile.ExtractToDirectory(fileName, dir);
                    if (File.Exists(fileName)) File.Delete(fileName);
                    Data.AppSettings.SetConfigValue("MonacoVersion", _monacoVersion);
                    _isLoadding = false;
                }
                catch (Exception e)
                {
                    _isLoadding = false;
                    if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                    if (File.Exists(fileName)) File.Delete(fileName);
                    Data.AppSettings.SetConfigValue("MonacoVersion", currentMonacoVersion);
                    throw e;
                }
            }
        }

    }
}

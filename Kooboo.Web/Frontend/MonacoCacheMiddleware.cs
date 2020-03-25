using Kooboo.Data;
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
using VirtualFile;
using VirtualFile.Zip;

namespace Kooboo.Web.Frontend
{
    class MonacoCacheMiddleware : IKoobooMiddleWare
    {

        static bool _isLoadding = false;
        readonly static string _monacoVersion = "1.1";
        static bool _zipExist = false;
        static string MonacoZipPath => Path.Combine(AppSettings.ModulePath, "Kooboo.Monaco.Module.zip");

        public MonacoCacheMiddleware()
        {
            _zipExist = File.Exists(MonacoZipPath);
            Task.Run(DownloadMonacoAsync);
        }

        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            if (context.Request.Path.ToLower().StartsWith("/_admin/kooboo.monaco.module"))
            {
                if (_isLoadding) throw new Exception("resource is loadding");
                await DownloadMonacoAsync();
            }

            await Next.Invoke(context);
        }

        private static async Task DownloadMonacoAsync()
        {
            var currentMonacoVersion = Data.AppSettings.MonacoVersion ?? "";
            var fileBakName = MonacoZipPath + ".bak";

            if (currentMonacoVersion != _monacoVersion || !_zipExist)
            {
                try
                {
                    _isLoadding = true;
                    var client = new System.Net.WebClient();
                    var uri = new Uri($"https://cdn.jsdelivr.net/gh/kooboo/monaco@master/vs{_monacoVersion}.zip");
                    var bytes = await client.DownloadDataTaskAsync(uri);

                    if (bytes != null && bytes.Length > 0)
                    {
                        if (File.Exists(MonacoZipPath))
                        {
                            VirtualResources.Setup(s => s.UnloadZip(MonacoZipPath));
                            if (File.Exists(fileBakName)) File.Delete(fileBakName);
                            File.Move(MonacoZipPath, fileBakName);
                        }

                        File.WriteAllBytes(MonacoZipPath, bytes);

                        VirtualResources.Setup(s => s.LoadZip(
                            MonacoZipPath,
                            AppSettings.RootPath,
                            new Lib.VirtualFile.Zip.ZipOption
                            {
                                Cache = true
                            }));
                        _zipExist = true;
                    }

                    AppSettings.SetConfigValue("MonacoVersion", _monacoVersion);
                    if (File.Exists(fileBakName)) File.Delete(fileBakName);
                    _isLoadding = false;
                }
                catch (Exception e)
                {
                    _isLoadding = false;
                    if (File.Exists(fileBakName)) File.Move(fileBakName, MonacoZipPath);
                    AppSettings.SetConfigValue("MonacoVersion", currentMonacoVersion);
                    _zipExist = File.Exists(MonacoZipPath);
                    throw e;
                }
            }
        }

    }
}

using System.IO;
using System.Net;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Attributes;
using Kooboo.Data;
using Kooboo.Data.Language;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Storage;

namespace Kooboo.Web.Api.Implementation
{
    public class Download : StorageBase, IApi
    {
        public string ModelName => "download";

        public bool RequireSite => true;

        public bool RequireUser => true;

        private static readonly Dictionary<string, string> TypeMaps = new Dictionary<string, string>
        {
            ["file"] = "Files",
            ["media"] = "MediaLibrary"
        };

        [RequireModel(typeof(BulkDownloadRequest))]
        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        public string Images(ApiCall call)
        {
            var provider = GetStorageProvider(call);
            var req = (BulkDownloadRequest)call.Context.Request.Model;
            var zipFile = provider.BulkDownloadMediaFiles(req);
            if (string.IsNullOrEmpty(zipFile))
            {
                throw new Exception(Hardcoded.GetValue("file not found", call.Context));
            }

            return zipFile;
        }

        [RequireModel(typeof(BulkDownloadRequest))]
        [Permission(Feature.FILE, Action = Data.Permission.Action.VIEW)]
        public string Files(ApiCall call)
        {
            var provider = GetStorageProvider(call);
            var req = (BulkDownloadRequest)call.Context.Request.Model;
            var zipFile = provider.BulkDownloadCmsFiles(req);
            if (string.IsNullOrEmpty(zipFile))
            {
                throw new Exception(Hardcoded.GetValue("file not found", call.Context));
            }

            return zipFile;
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.FILE, Action = Data.Permission.Action.VIEW)]
        [RequireParameters("type", "file")]
        public BinaryResponse Package(ApiCall call)
        {
            var fileName = call.GetValue("file");
            if (string.IsNullOrEmpty(fileName))
            {
                throw new FileNotFoundException();
            }

            var zipFile = IOHelper.CombinePath(AppSettings.TempDataPath, fileName);
            if (!File.Exists(zipFile))
            {
                throw new FileNotFoundException();
            }

            var fileStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read);
            var response = new BinaryResponse
            {
                ContentType = "application/zip",
                Stream = fileStream,
            };
            var type = call.GetValue("type") ?? "unknown";
            var newFileName = $"{TypeMaps.GetValueOrDefault(type, "Download")}_{DateTime.Now:yyyy-MM-dd_HH:mm}.zip";
            response.Headers.Add("Content-Disposition", $"attachment;filename={newFileName}");
            return response;
        }

        [RequireParameters("type", "id")]
        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.FILE, Action = Data.Permission.Action.VIEW)]
        public BinaryResponse Single(ApiCall call)
        {
            var type = call.GetValue("type");
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException("type");
            }

            var id = call.GetGuidValue("id");
            var siteDb = call.WebSite.SiteDb();
            var (name, content) = GetObjectBytes(siteDb, type, id);
            if (content == null || name == null)
            {
                throw new FileNotFoundException();
            }

            var response = new BinaryResponse
            {
                ContentType = "application/octet-stream",
                BinaryBytes = content
            };
            response.Headers.Add("Content-Disposition", $"attachment;filename={WebUtility.UrlEncode(name)}");
            return response;
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.FILE, Action = Data.Permission.Action.VIEW)]
        [RequireParameters("provider", "key")]
        public IResponse Storage(ApiCall call)
        {
            var provider = call.GetValue("provider");
            var key = call.GetValue("key");

            var client = StorageProviderFactory.GetStorageClient(provider, call.Context);
            var url = client.GenerateDownloadUrl(key);

            return new MetaResponse
            {
                RedirectUrl = url,
            };
        }

        private static (string, byte[]) GetObjectBytes(SiteDb siteDb, string type, Guid id)
        {
            if (type == "file")
            {
                var obj = siteDb.Files.Get(id);
                return (obj?.Name, obj?.ContentBytes);
            }

            if (type == "media")
            {
                var obj = siteDb.Images.Get(id);
                return (obj?.Name, obj?.ContentBytes);
            }

            return (null, null);
        }
    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using Csv;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Storage;
using Kooboo.Sites.Storage.Kooboo;
using Kooboo.Web.Api.Implementation;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api
{
    public class UploadApi : StorageBase, IApi
    {
        public string ModelName
        {
            get
            {
                return "upload";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }


        public List<IEmbeddableItemListViewModel> Style(ApiCall call)
        {
            List<IEmbeddableItemListViewModel> result = new List<IEmbeddableItemListViewModel>();

            foreach (var f in call.Context.Request.Files)
            {
                var siteobject = call.WebSite.SiteDb().Styles.Upload(f.Bytes, f.FileName, call.Context.User.Id);
                result.Add(new IEmbeddableItemListViewModel(call.WebSite.SiteDb(), siteobject));
            }
            return result;
        }

        public List<IEmbeddableItemListViewModel> Script(ApiCall call)
        {
            List<IEmbeddableItemListViewModel> result = new List<IEmbeddableItemListViewModel>();

            foreach (var f in call.Context.Request.Files)
            {

                var siteobject = call.WebSite.SiteDb().Scripts.Upload(f.Bytes, f.FileName, call.Context.User.Id);
                result.Add(new IEmbeddableItemListViewModel(call.WebSite.SiteDb(), siteobject));
            }
            return result;
        }

        [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.EDIT)]
        public List<MediaStorageFileModel> Image(ApiCall call)
        {
            string Folder = call.Context.Request.Forms.Get("Folder")?.ToString() ?? string.Empty;
            string prefix = call.Context.Request.Forms.Get("Prefix")?.ToString() ?? string.Empty;


            if (!Folder.EndsWith("/") && !Folder.EndsWith("\\"))
            {
                Folder = Folder + "/";
            }

            var result = new List<MediaStorageFileModel>();
            var storage = GetStorageProvider(call);
            if (storage is KoobooStorageProvider)
            {
                if (Folder == "/")
                {
                    Folder = KoobooStorageProvider.HandleCustomSettingPrefix(call.WebSite, Folder);
                }
            }
            foreach (var item in call.Context.Request.Files)
            {
                string filename = FormatKey(item.FileName);
                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    filename = $"{prefix}_{filename}";
                }

                filename = UrlHelper.Combine(Folder, filename);

                var alt = Path.GetFileNameWithoutExtension(filename);
                var imageModel = storage.UploadMediaFile(filename, item.Bytes, alt);

                result.Add(imageModel);
            }

            return result;
        }

        [Permission(Feature.FILE, Action = Data.Permission.Action.EDIT)]
        public bool File(ApiCall call)
        {
            string Folder = call.Context.Request.Forms.Get("Folder")?.ToString() ?? string.Empty;

            if (!Folder.EndsWith("/") && !Folder.EndsWith("\\"))
            {
                Folder = Folder + "/";
            }

            var storage = GetStorageProvider(call);
            foreach (var item in call.Context.Request.Files)
            {
                string filename = FormatKey(item.FileName);

                filename = UrlHelper.Combine(Folder, filename);

                storage.UploadCmsFile(filename, item.Bytes);
            }

            return true;
        }


        [Permission(Feature.SITE, Action = Data.Permission.Action.EDIT)]
        public bool Package(ApiCall call)
        {
            var files = call.Context.Request.Files;

            if (files != null && files.Count > 0)
            {
                foreach (var f in files)
                {
                    var bytes = f.Bytes;
                    string filename = f.FileName;

                    string extension = System.IO.Path.GetExtension(filename);
                    if (!string.IsNullOrEmpty(extension))
                    {
                        extension = extension.ToLower();
                    }

                    if (extension == ".zip" || extension == ".rar")
                    {


                        var packageId = call.GetValue("packageId");

                        if (packageId != default)
                        {
                            var path = Path.Combine(GetPackageHistoryFolder(call.WebSite), $"{packageId}.json");
                            if (System.IO.File.Exists(path))
                            {
                                var fileNames = JsonSerializer.Deserialize<string[]>(System.IO.File.ReadAllText(path));
                                Sites.Sync.ImportExport.RemoveFiles(fileNames, call.WebSite);
                            }
                        }

                        var memory = new MemoryStream(bytes);
                        Sites.Sync.ImportExport.ImportZip(memory, call.WebSite, call.Context.User.Id);

                        if (packageId != default)
                        {
                            var path = Path.Combine(GetPackageHistoryFolder(call.WebSite), $"{packageId}.json");
                            if (packageId != default)
                            {
                                memory = new MemoryStream(bytes);
                                using var archive = new ZipArchive(memory, ZipArchiveMode.Read);

                                var fileNames = archive.Entries
                                    .Select(s => s.FullName)
                                    .Where(w => !string.IsNullOrWhiteSpace(w))
                                    .ToArray();

                                System.IO.File.WriteAllText(path, JsonSerializer.Serialize(fileNames));
                            }
                        }
                    }
                }
            }

            return true;
        }

        [Permission(Feature.DATABASE, Action = Kooboo.Data.Permission.Action.VIEW)]
        public PagedListViewModel<Dictionary<string, string>, BaseColumnViewModel> ReadCsv(ApiCall call)
        {
            var file = call.Context.Request.Files.FirstOrDefault();
            if (file?.Bytes == null)
            {
                throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("invalid data", call.Context));
            }

            var rowsToSkip = call.GetIntValue("rowsToSkip");
            var bytes = file.Bytes;
            var encoding = EncodingDetector.GetEncoding(ref bytes);
            var csvText = encoding.GetString(file.Bytes);
            var records = CsvReader.ReadFromText(csvText, new CsvOptions
            {
                RowsToSkip = rowsToSkip,
                HeaderMode = HeaderMode.HeaderAbsent,
                ValidateColumnCount = false,
                ReturnEmptyForMissingColumn = true,
                AllowNewLineInEnclosedFieldValues = true,
            });

            var headerRow = records.FirstOrDefault() ?? throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("Header row not found", call.Context));

            if (records.Count() < 2)
            {
                throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("No records to import", call.Context));
            }

            var columns = headerRow.Headers
                .Select(it => new BaseColumnViewModel
                {
                    Name = it.Trim(),
                    DisplayName = headerRow[it]?.Trim() ?? string.Empty,
                })
                //.Where(it => !string.IsNullOrWhiteSpace(it.DisplayName))
                .ToList();

            var rows = new List<Dictionary<string, string>>();
            var errors = new StringBuilder();
            foreach (var item in records.Skip(1))
            {
                var row = new Dictionary<string, string>();
                try
                {
                    foreach (var column in columns)
                    {
                        if (item.HasColumn(column.Name))
                        {
                            row[column.Name] = item[column.Name]?.Trim();
                        }
                    }
                    rows.Add(row);
                }
                catch (Exception ex)
                {
                    errors.AppendLine(ex.Message);
                }
            }

            var response = new PagedListViewModel<Dictionary<string, string>, BaseColumnViewModel>
            {
                Columns = columns,
                List = rows
            };
            return response;
        }

        public bool IsUniqueKey(ApiCall call)
        {
            string name = FormatKey(call.NameOrId);
            var oldName = call.GetValue("oldName");
            var isMedia = call.GetValue("type") == "media";
            var storage = GetStorageProvider(call);
            return isMedia ? storage.IsUniqueMediaKey(KoobooStorageProvider.HandleCustomSettingPrefix(call.WebSite, name), oldName) : storage.IsUniqueCmsFileKey(name, oldName);
        }

        private string FormatKey(string key)
        {
            return string.Join('/', key.Split('/').Select(StringHelper.ToValidFileName));
        }

        public static string GetPackageHistoryFolder(WebSite website)
        {
            var orgid = website.OrganizationId;

            string orgfolder = AppSettings.GetOrganizationFolder(orgid);

            string websitefolder = Path.Combine(orgfolder, website.Name);

            string fileiofolder = Path.Combine(websitefolder, "PackageHistory");

            IOHelper.EnsureDirectoryExists(fileiofolder);

            return fileiofolder;
        }
    }
}

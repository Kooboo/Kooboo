//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Sync;
using Kooboo.Sites.Sync.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class ReceiverApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "receiver";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;
            }
        }

        //item push by remote client... 
        public ReceiverFeedback Push(ApiCall call)
        {
            Guid Hash = call.GetValue<Guid>("hash");

            if (Hash != default(Guid))
            {
                var computeHash = Kooboo.Lib.Security.Hash.ComputeGuid(call.Context.Request.PostData);

                if (computeHash != Hash)
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("Hash validation failed", call.Context));
                }
            }
            var converter = new IndexedDB.Serializer.Simple.SimpleConverter<SyncObject>();
            SyncObject sync = converter.FromBytes(call.Context.Request.PostData);

            var website = Data.Config.AppHost.SiteRepo.Get(sync.RemoteSiteId);
            var sitedb = website.SiteDb();

            Guid userId = sync.UserId;
            if (userId == default(Guid) && call.Context.User != null)
            {
                userId = call.Context.User.Id;
            }

            // 
            var check = Kooboo.Sites.Sync.ConflictService.instance.CheckPushIn(sync, sitedb);
            if (check.HasConflict)
            {
                ReceiverFeedback feedback = new ReceiverFeedback() { HasConflict = true, };

                if (check.log != null)
                {
                    feedback.ReceiverVersion = check.log.Id;
                    feedback.UserName = Kooboo.Data.GlobalDb.Users.GetUserName(check.log.UserId);
                    feedback.LastModified = check.log.UpdateTime;
                    feedback.EditType = check.log.EditType;
                }

                if (check.IsTable)
                {
                    feedback.DisplayBody = Kooboo.Sites.Service.ObjectService.GetSummaryText(check.TableData);
                }
                else
                {
                    if (check.SiteObject is Kooboo.Sites.Models.Image)
                    {
                        var siteImage = check.SiteObject as Kooboo.Sites.Models.Image;
                        feedback.IsImage = true;
                        feedback.DisplayBody = Convert.ToBase64String(siteImage.ContentBytes);

                    }
                    else
                    {
                        feedback.DisplayBody = Kooboo.Sites.Service.ObjectService.GetSummaryText(check.SiteObject);
                    }
                }
                return feedback;

            }
            else
            {
                var version = SyncService.Receive(sitedb, sync, null, userId);
                return new ReceiverFeedback() { Success = true, ReceiverVersion = version };
            }
        }

        //item push by remote client... 
        public ReceiverFeedback ForcePush(ApiCall call)
        {
            Guid Hash = call.GetValue<Guid>("hash");

            if (Hash != default(Guid))
            {
                var computeHash = Kooboo.Lib.Security.Hash.ComputeGuid(call.Context.Request.PostData);

                if (computeHash != Hash)
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("Hash validation failed", call.Context));
                }
            }
            var converter = new IndexedDB.Serializer.Simple.SimpleConverter<SyncObject>();
            SyncObject sync = converter.FromBytes(call.Context.Request.PostData);

            var website = Data.Config.AppHost.SiteRepo.Get(sync.RemoteSiteId);
            var sitedb = website.SiteDb();

            Guid userId = sync.UserId;
            if (userId == default(Guid) && call.Context.User != null)
            {
                userId = call.Context.User.Id;
            }

            var logId = SyncService.Receive(sitedb, sync, null, userId);

            return new ReceiverFeedback() { ReceiverVersion = logId, HasConflict = false };
        }

        public void Zip(ApiCall call)
        {
            var isSpa = call.GetBoolValue("iSSpa");

            if (call.Context.WebSite == null)
            {
                throw new Exception("site not found");
            }

            var SiteId = call.Context.WebSite.Id;
            // verify login.  
            if (call.Context.User == null)
            {
                throw new Exception("Access denied");
            }
            else
            {
                var userSites = Kooboo.Sites.Service.WebSiteService.ListByUser(call.Context.User);

                var find = userSites.Find(o => o.Id == SiteId);
                if (find == null)
                {
                    throw new Exception("Access denied");
                }
            }


            var website = Data.Config.AppHost.SiteRepo.Get(SiteId);
            var sitedb = website.SiteDb();
            //var zip = call.Context.Request.PostData; 

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
                        MemoryStream memory = new MemoryStream(bytes);

                        Kooboo.Sites.Sync.ImportExport.ImportZip(memory, call.WebSite, call.Context.User.Id);
                    }

                }
            }


        }

    }
}
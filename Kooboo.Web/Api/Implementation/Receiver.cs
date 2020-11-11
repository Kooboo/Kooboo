//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Template;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Sync;
using System;
using System.IO;

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
        public void Push(Guid SiteId, ApiCall call)
        {
            Guid Hash = call.GetValue<Guid>("hash");

            Guid userid = default(Guid);
            if (call.Context.User != null)
            {
                userid = call.Context.User.Id;
            }

            if (Hash != default(Guid))
            {
                var hashback = Kooboo.Lib.Security.Hash.ComputeGuid(call.Context.Request.PostData);

                if (hashback != Hash)
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("Hash validation failed", call.Context));
                }
            } 

            var website = Kooboo.Data.GlobalDb.WebSites.Get(SiteId);
            var sitedb = website.SiteDb();

            var converter = new IndexedDB.Serializer.Simple.SimpleConverter<SyncObject>();

            SyncObject sync = converter.FromBytes(call.Context.Request.PostData);

            SyncService.Receive(sitedb, sync, null, userid);

        }

   
        public void Zip(Guid SiteId, ApiCall call)
        {
           // verify login.  
           if (call.Context.User == null)
            {
                throw new Exception("Access denied"); 
            }
           else
            {
                var usersites = Kooboo.Sites.Service.WebSiteService.ListByUser(call.Context.User);

                var find = usersites.Find(o => o.Id == SiteId); 
                if (find == null)
                {
                    throw new Exception("Access denied");
                }
            }

            // Guid Hash = call.GetValue<Guid>("hash");
             
            //if (Hash != default(Guid))
            //{
            //    var hashback = Kooboo.Lib.Security.Hash.ComputeGuid(call.Context.Request.PostData);

            //    if (hashback != Hash)
            //    {
            //        throw new Exception(Data.Language.Hardcoded.GetValue("Hash validation failed", call.Context));
            //    }
            //}

            var website = Kooboo.Data.GlobalDb.WebSites.Get(SiteId);
            var sitedb = website.SiteDb();

            //  var zip = call.Context.Request.PostData; 

            var files = Kooboo.Lib.NETMultiplePart.FormReader.ReadFile(call.Context.Request.PostData);

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
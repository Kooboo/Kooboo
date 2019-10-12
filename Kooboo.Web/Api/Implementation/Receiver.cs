//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Template;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Sync;
using System;

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
    }
}
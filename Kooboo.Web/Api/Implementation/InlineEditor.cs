//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Diagnostics;
using System.Linq;
using Kooboo.Api;
using Kooboo.Lib.Helper;
using Kooboo.Sites.DataTraceAndModify;
using Kooboo.Sites.DataTraceAndModify.Modifiers;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.SiteTransfer;

namespace Kooboo.Web.Api.Implementation
{
    public class InlineEditor : IApi
    {
        public Type ModelType
        {
            get; set;
        }

        public string ModelName
        {
            get { return "InlineEditor"; }
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

        public void Update(string url, ApiCall call)
        {
            var page = call.WebSite.SiteDb().Pages.GetByUrl(url);
            if (page == null)
            {
                return;
            }

            call.Context.SetItem<Page>(page);

            var data = Lib.Helper.JsonHelper.Deserialize<dynamic>(call.Context.Request.Body);
            var changedList = new List<ModifierBase>();

            foreach (var item in data)
            {
                var source = item.GetValue("source").ToString();

                try
                {
                    var type = ModifyExecutor.GetModifierType(source);
                    changedList.Add((ModifierBase)item.ToObject(type));
                }
                catch (Exception e)
                {
                    Debug.Fail(e.Message);
                    continue;
                }
            }

            ModifyExecutor.Execute(call.Context, changedList);
        }


        public UrlCheckResult CanEnter(string url, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();
            url = url.ToLower().Trim();

            if (url.StartsWith("//") || url.StartsWith("http"))
            {
                url = url.TrimStart('/');
                if (url.StartsWith("http://")) url = url[7..];
                if (url.StartsWith("https://")) url = url[8..];
                var list = Data.Config.AppHost.BindingService.GetBySiteId(call.Context.WebSite.Id);
                var binding = list.FirstOrDefault(a => url.StartsWith(a.FullDomain));
                url = url.Substring(binding.FullDomain.Length);
            }


            var route = Kooboo.Sites.Routing.ObjectRoute.GetRoute(sitedb, url);

            if (route?.DestinationConstType == ConstObjectType.Page)
            {
                return new UrlCheckResult() { Url = url, CanEnter = true };
            }
            else if (call.WebSite.EnableSitePath)
            {
                route = Kooboo.Sites.Routing.ObjectRoute.GetRoute(sitedb, url[3..]);
                if (route?.DestinationConstType == ConstObjectType.Page)
                {
                    return new UrlCheckResult() { Url = url, CanEnter = true };
                }

            }

            _ = TransferManager.continueDownload(sitedb, url, call.Context).Result;
            route = Kooboo.Sites.Routing.ObjectRoute.GetRoute(sitedb, url);
            if (route?.DestinationConstType == ConstObjectType.Page)
            {
                return new UrlCheckResult() { Url = url, CanEnter = true };
            }

            return new UrlCheckResult() { Message = "Inline editor only support editing on page" };
        }

        public class UrlCheckResult
        {
            public bool CanEnter { get; set; }

            public string Url { get; set; }

            public string Message { get; set; }
        }


    }




}

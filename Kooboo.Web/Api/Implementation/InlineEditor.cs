//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Sites.InlineEditor;
using Newtonsoft.Json.Linq;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Lib.Utilities;
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.DataTraceAndModify;
using System.Linq;

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

        [Kooboo.Attributes.RequireParameters("updates")]
        public void Update(Guid PageId, ApiCall call)
        {
            var page = call.WebSite.SiteDb().Pages.Get(PageId);
            if (page == null)
            {
                return;
            }

            call.Context.SetItem<Page>(page);

            var data = Lib.Helper.JsonHelper.Deserialize<dynamic>(call.Context.Request.Body).updates;
            var changedList = new List<ModifierBase>();

            foreach (var item in data)
            {
                var source = item.GetValue("source").ToString();
                var type = ModifyExecutor.GetModifierType(source);
                changedList.Add((ModifierBase)item.ToObject(type));
            }

            ModifyExecutor.Execute(call.Context, changedList);
        }
    }
}

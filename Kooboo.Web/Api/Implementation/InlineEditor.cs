//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.DataTraceAndModify;
using Kooboo.Sites.DataTraceAndModify.Modifiers;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
    }
}

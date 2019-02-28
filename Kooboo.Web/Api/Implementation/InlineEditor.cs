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

            // pageid,...updates...  
            if (string.IsNullOrEmpty(call.Context.Request.Body))
            {
                return;
            }
            var model = Lib.Helper.JsonHelper.Deserialize<dynamic>(call.Context.Request.Body);

            List<IInlineModel> updatemodels = new List<IInlineModel>();

            foreach (var item in model.updates)
            {
                string editortype = item.editorType;
                var modeltype = EditorContainer.GetModelType(editortype);
                var updatemodel = ((JObject)item).ToObject(modeltype) as IInlineModel;
                if (updatemodel != null)
                {
                    updatemodels.Add(updatemodel);
                }
            }

            UpdateManager.Execute(call.Context, updatemodels);

        }
   

    }
    

}

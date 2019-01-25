//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.ViewModel;
using System;

namespace Kooboo.Sites.DataSources
{
    public class ContentItem : SiteDataSource
    {
        public bool IsDefault
        {
            get
            {
                return this.Context.RenderContext.Request.Channel == Data.Context.RequestChannel.Default;
            }
        }
         
        [Kooboo.Attributes.RequireFolder]
        public TextContentViewModel  ById(Guid Id)
        {
            var textcontent = Context.SiteDb.TextContent.Get(Id);
            if (textcontent != null)
            {  
                var view = Helper.ContentHelper.ToView(textcontent, Context.RenderContext.Culture, Context.SiteDb.ContentTypes.GetColumns(textcontent.ContentTypeId));
                if (this.IsDefault && (view != null && view.Online == false))
                { return null; }
                return view;
            }
            return null; 
       
        }

        [Kooboo.Attributes.RequireFolder]
        public TextContentViewModel  ByUserKey(string UserKey)
        {
            Guid id = Lib.Security.Hash.ComputeGuidIgnoreCase(UserKey);
            return  ById(id);
        } 
    }
}

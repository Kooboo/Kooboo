//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.ViewModel;

namespace Kooboo.Sites.Render
{
    public class BindingTextContentItemRenderTask : BindingObjectRenderTask
    {
      
        private string ItemAliasKey; 
        public BindingTextContentItemRenderTask(string ItemAliasKey, string boundary, bool isEnd)
        {
            this.ItemAliasKey = ItemAliasKey;
            this.Boundary = boundary;
            this.IsEndBinding = isEnd;  
        }
         

        public override string Render(RenderContext context)
        {
            var value = context.DataContext.GetValue(this.ItemAliasKey); 
           
            if (value is Kooboo.Data.Models.DataMethodResult)
            {
                var item = value as Data.Models.DataMethodResult; 
                if (item.HasValue && item.Value is TextContentViewModel)
                {
                    return RenderTextContent(context, item.Value as TextContentViewModel); 
                }
            } 
            else
            {
                if (value is TextContentViewModel)
                {
                    return RenderTextContent(context, value as TextContentViewModel); 
                }
            }
            return null; 
        }

        private string RenderTextContent(RenderContext context, TextContentViewModel contentitem)
        {
            if (contentitem == null)
            {
                return null;
            }
            string result = "\r\n<!--#kooboo";
            if (this.IsEndBinding)
            {
                result += "--end=true"; 
                result += "--objecttype='contentrepeater'";

                if (!string.IsNullOrEmpty(this.Boundary))
                {
                    result += "--boundary='" + this.Boundary + "'";
                }
                result += "-->\r\n";
                return result;
            }
            else
            {

                result += "--objecttype='contentrepeater'";

                result += "--nameorid='" + contentitem.Id.ToString() + "'";
                result += "--folderid='" + contentitem.FolderId.ToString() + "'";

                result += "--bindingvalue='" + this.ItemAliasKey + "'";
                

                if (!string.IsNullOrEmpty(this.Boundary))
                {
                    result += "--boundary='" + this.Boundary + "'";
                }
                result += "-->\r\n";
                return result;
            }
        }
    }

     


}

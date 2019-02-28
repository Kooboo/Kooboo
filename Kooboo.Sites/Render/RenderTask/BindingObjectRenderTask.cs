//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.ViewModel;

namespace Kooboo.Sites.Render
{
    public class BindingObjectRenderTask : IRenderTask
    {
        public string ObjectType { get; set; }
        public string NameOrId { get; set; }

        public string AttributeName { get; set; }

        public string KoobooId { get; set; }

        public string BindingValue { get; set; }

        //for text content. 
        public string FieldName { get; set; }

        public bool IsEndBinding { get; set; }

        public string Boundary { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false; 
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            string renderresult = this.Render(context);
            result.Add(new RenderResult() {  Value = renderresult }); 
        }
         
        public virtual string Render(RenderContext context)
        {
            string result = "\r\n<!--#kooboo";
            if (this.IsEndBinding)
            {
                result += "--end='true'";

                if (!string.IsNullOrEmpty(this.ObjectType))
                {
                    result += "--objecttype='" + this.ObjectType + "'";
                }
                if (!string.IsNullOrEmpty(this.Boundary))
                {
                    result += "--boundary='" + this.Boundary + "'";
                }
                result += "-->\r\n";
                return result;
            }
            else
            {

                if (!string.IsNullOrEmpty(this.ObjectType))
                {
                    result += "--objecttype='" + this.ObjectType + "'";
                }

                string nameorid = this.NameOrId; 
                if (string.IsNullOrEmpty(nameorid))
                {
                    nameorid = GetNameOrId(context); 
                }

                if (!string.IsNullOrEmpty(nameorid))
                {
                    result += "--nameorid='" + nameorid + "'";
                }

                if (!string.IsNullOrEmpty(this.AttributeName))
                {
                    result += "--attributename='" + this.AttributeName + "'";
                }
                if (!string.IsNullOrEmpty(this.BindingValue))
                {
                    result += "--bindingvalue='" + this.BindingValue + "'";
                }

                if (!string.IsNullOrEmpty(this.KoobooId))
                {
                    result += "--koobooid='" + this.KoobooId + "'";
                }

                if (!string.IsNullOrEmpty(this.Boundary))
                {
                    result += "--boundary='" + this.Boundary + "'";
                }
                result += "-->\r\n";
                return result;
            }
        }


        private string GetNameOrId(RenderContext context)
        {
            if (string.IsNullOrEmpty(this.BindingValue))
            {
                return null;
            }

            string objectkey = this.BindingValue;
           
            int lastDotIndex = this.BindingValue.LastIndexOf(".");
            if (lastDotIndex > -1)
            {
                objectkey = this.BindingValue.Substring(0, lastDotIndex);
                //Modify by cz 
                if (objectkey.IndexOf("{") > -1)
                {
                    objectkey = objectkey.TrimStart('{');
                }
            }
           
            object value = context.DataContext.GetValue(objectkey);
            if (value == null)
            {
                return null;
            }
            if (value is DataMethodResult)
            {
                value = ((DataMethodResult)value).Value;
            }
            if (value is TextContentViewModel)
            {
                TextContentViewModel textcontent = value as TextContentViewModel;

                return textcontent.Id.ToString();
            }
            return null; 
        }

    }
}

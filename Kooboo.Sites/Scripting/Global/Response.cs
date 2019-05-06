//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic; 
using System.Collections; 

namespace Kooboo.Sites.Scripting.Global
{
    public class Response
    {
        private RenderContext context { get; set; }
        public Response(RenderContext context)
        {
            this.context = context;
        }

        public void write(object value)
        {
            if (value == null)
            {
                return;
            }  
            string output = null; 
            if (!(value is string) && value.GetType().IsClass)
            {
                if (value is Kooboo.Data.Interface.IDynamic)
                {
                    var dynamic = value as Kooboo.Data.Interface.IDynamic;
                    output = Lib.Helper.JsonHelper.SerializeCaseSensitive(dynamic.Values); 
                } 
   
                else
                {
                    output = Lib.Helper.JsonHelper.SerializeCaseSensitive(value);
                } 
            }
            else
            {
                output = value.ToString();
            }

            var item = this.context.GetItem<string>(Constants.OutputName);
            if (item == null)
            {
                this.context.SetItem<string>(output, Constants.OutputName);
            }
            else
            {
                item += output;
                this.context.SetItem<string>(item, Constants.OutputName);
            }
        }

        public void setHeader(string key, string value)
        {
            this.context.Response.Headers[key] = value;
        }

        public void redirect(string url)
        {
            this.context.Response.Redirect(302, url);
        }

        public void Json(object value)
        {
            // write method default is Json already...
            write(value); 
        }       
    
    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Kooboo.Lib;

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
            string output = ToJson(value); 

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

        public string ToJson(object value)
        {
            string output;
            if (!(value is string) && value.GetType().IsClass)
            {
                if (value is Kooboo.Data.Interface.IDynamic)
                {
                    var dynamic = value as Kooboo.Data.Interface.IDynamic;
                    output = Lib.Helper.JsonHelper.SerializeCaseSensitive(dynamic.Values);
                }
                else if (IsDynamicArray(value))
                {
                    var dynamicValues = value as ICollection;

                    List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                    foreach (var v in dynamicValues)
                    {
                        var dynamicvalue = v as Kooboo.Data.Interface.IDynamic;
                        if (dynamicvalue != null)
                        {
                            result.Add(dynamicvalue.Values);
                        }
                    }
                    output = Lib.Helper.JsonHelper.SerializeCaseSensitive(result);
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
            return output;
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

        public bool IsDynamicArray(object value)
        {
            var type = value.GetType();

            if (Lib.Reflection.TypeHelper.IsCollection(type))
            {
                if (type.IsGenericType)
                {
                    if (Lib.Reflection.TypeHelper.GetGenericType(value.GetType()) == typeof(Kooboo.Data.Interface.IDynamic))
                    {
                        return true;
                    }
                    else
                    {
                        return false; 
                    } 
                }
                else
                {
                    var oneitem = GetOneItem(value as ICollection);
                    var IDynamicItem = oneitem as Kooboo.Data.Interface.IDynamic;

                    return IDynamicItem != null;  
                } 

            }

            return false; 
        }

        public object GetOneItem(ICollection col)
        {
            if (col == null)
            {
                return null; 
            }
            foreach (var item in col)
            {
                return item; 
            }
            return null; 
        }

    }
}

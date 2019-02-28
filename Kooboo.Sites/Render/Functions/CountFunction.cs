//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Kooboo.Sites.Render.Functions
{
    public class CountFunction : IFunction
    {
        public string Name
        {
            get
            {
                return "Count"; 
            }
        }

        public List<IFunction> Parameters
        {
            get;set;
        }

        public object Render(RenderContext context)
        {
            var paras = FunctionHelper.RenderParameter(context, this.Parameters);

            if (paras != null && paras.Count() > 0)
            { 
                var value = paras[0];
                if (value != null)
                {
                    if (value is ICollection)
                    {
                        var col = (ICollection)value;
                        if (col != null)
                        {
                            return col.Count;
                        }

                    }
                    else if (value is string)
                    {
                        try
                        {
                            var jarray = Lib.Helper.JsonHelper.DeserialzeBaseObject(value.ToString()); 
                            if (jarray !=null && jarray is JArray)
                            {
                                var jr = jarray as JArray; 
                                if (jr !=null)
                                {
                                    return jr.Count; 
                                }
                            }
                        }
                        catch (Exception)
                        { 
                        } 
                      
                    }
                   
                }
            }
            return 0; 
        }
           
    }
}

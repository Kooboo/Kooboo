//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            get; set;
        }

        public object Render(RenderContext context)
        {
            var paras = FunctionHelper.RenderParameter(context, this.Parameters);

            if (paras != null && paras.Any())
            {
                var value = paras[0];
                if (value != null)
                {
                    if (value is ICollection col)
                    {
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
                            if (jarray is JArray jr)
                            {
                                if (jr != null)
                                {
                                    return jr.Count;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }
            }
            return 0;
        }
    }
}
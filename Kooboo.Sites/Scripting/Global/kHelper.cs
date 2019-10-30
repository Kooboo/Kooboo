//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.Sites.Scripting.Global
{
    public static class kHelper
    {
        public static object PrepareData(object dataobj, Type modelType)
        {
            Dictionary<string, object> data = GetData(dataobj);

            var result = Lib.Reflection.TypeHelper.ToObject(data, modelType);

            return result;
        }

        public static Dictionary<string, object> GetData(object dataobj)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (dataobj is IDictionary idict)
            {
                foreach (var item in idict.Keys)
                {
                    var value = idict[item];
                    if (value != null)
                    {
                        data.Add(item.ToString(), value.ToString());
                    }
                }
            }
            else
            {
                if (dataobj is IDictionary<string, object> dynamicobj)
                {
                    foreach (var item in dynamicobj.Keys)
                    {
                        var value = dynamicobj[item];
                        if (value != null)
                        {
                            data.Add(item.ToString(), value.ToString());
                        }
                    }
                }
            }

            return data;
        }
    }
}
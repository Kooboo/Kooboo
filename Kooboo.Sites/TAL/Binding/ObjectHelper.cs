//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections;

using Kooboo.Data.Models;

namespace Kooboo.TAL.Binding
{
    public static class ObjectHelper
    {
        
       

        static object Unwarp(object obj)
        {
            var result = obj as DataMethodResult;
            return result != null ? result.Value : obj;
        }

     
        static object GetPropertyValue(object obj, string[] segments, int start, out object container, out string propertyName)
        {
            container = null;
            propertyName = null;

            if (start >= segments.Length)
            {
                return null;
            }

            object result = null;

            var unwrappedObj = Unwarp(obj);
            if (unwrappedObj != null)
            {
                if (unwrappedObj is DataMethodResult)
                {
                    result = GetPropertyValue(unwrappedObj, segments, start, out container, out propertyName);
                }
                else if (unwrappedObj is IDictionary)
                {
                    var dic = unwrappedObj as IDictionary;
                    if (dic.Contains(segments[start]))
                    {
                        if (start == segments.Length - 1)
                        {
                            container = dic;
                            propertyName = segments[start];
                            return dic[segments[start]];
                        }

                        return GetPropertyValue(dic[segments[start]], segments, start + 1, out container, out propertyName);
                    }

                    return null;
                }
                else
                {
                    container = unwrappedObj;
                    result = container;

                    for (var i = start; i < segments.Length; i++)
                    {
                        var propName = segments[i];
                        propertyName = propName;

                        try
                        {
                            result = Lib.Reflection.Dynamic.GetObjectMember(container, propName);
                        }
                        catch
                        {
                            result = null;
                            // It's by designed to catch all exceptions.
                            // If the property is not found on the container object, we should return null indicating the expression evaluates to null
                        }

                        if (result == null)
                        {
                            break;
                        }

                        if (i != segments.Length - 1)
                        {
                            container = result;
                        }
                    }
                }
            }

            if (result == null)
            {
                var dataResult = obj as DataMethodResult;
                if (dataResult != null && dataResult.Children.ContainsKey(segments[start]))
                {
                    var child = dataResult.Children[segments[start]];
                    if (start == segments.Length - 1)
                    {
                        result = child;
                    }
                    else
                    {
                        result = GetPropertyValue(child, segments, start + 1, out container, out propertyName);
                    }
                }
            }

            return result;
        }
    }
}

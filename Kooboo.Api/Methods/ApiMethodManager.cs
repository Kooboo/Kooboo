//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kooboo.Api.Methods
{
    public static class ApiMethodManager
    {
        private static readonly object locker = new object();
        private static Dictionary<string, ApiMethod> _list;

        public static Dictionary<string, ApiMethod> List
        {
            get
            {
                if (_list == null)
                {
                    lock (locker)
                    {
                        if (_list == null)
                        {
                            _list = new Dictionary<string, ApiMethod>(StringComparer.OrdinalIgnoreCase);
                        }
                    }
                }
                return _list;
            }
        }

        public static ApiMethod Get(IApi instance, string methodName)
        {
            var currentlist = List;

            if (string.IsNullOrEmpty(methodName))
            {
                return null;
            }
            string key = instance.ModelName + "." + methodName;
            if (currentlist.ContainsKey(key))
            {
                return currentlist[key];
            }

            if (instance is IDynamicApi)
            {
                var dynamic = instance as IDynamicApi;
                return GetDynamicApiMethod(dynamic, methodName);
            }

            var methodinfo = Lib.Reflection.TypeHelper.GetMethodInfo(instance.GetType(), methodName);

            if (methodinfo == null)
            {
                currentlist[key] = null;
                return null;
            }

            var newmethod = new ApiMethod
            {
                MethodName = methodinfo.Name,
                ClassInstance = instance,
                ReturnType = methodinfo.ReturnType,
                DeclareType = instance.GetType(),
                IsVoid = methodinfo.ReturnType == typeof(void)
            };

            var acceptmodel = methodinfo.GetCustomAttribute(typeof(Attributes.RequireModel), true);
            if (acceptmodel != null)
            {
                var accepted = acceptmodel as Attributes.RequireModel;
                newmethod.RequireModelType = accepted.ModelType;
            }

            var requirepara = methodinfo.GetCustomAttribute(typeof(Attributes.RequireParameters));
            if (requirepara != null)
            {
                var requiredParas = requirepara as Attributes.RequireParameters;
                newmethod.RequireParas = requiredParas.Parameters;
            }

            if (newmethod.IsVoid)
            {
                newmethod.Void = Lib.Reflection.TypeHelper.CompileAction(methodinfo);
            }
            else
            {
                newmethod.Func = Lib.Reflection.TypeHelper.CompileFunc(methodinfo);
            }

            var paras = methodinfo.GetParameters();
            foreach (var item in paras)
            {
                newmethod.Parameters.Add(new Parameter() { Name = item.Name, ClrType = item.ParameterType });
            }

            //set set call function.
            var apicallName = GetApiCallName(newmethod.DeclareType);
            if (!string.IsNullOrEmpty(apicallName))
            {
                newmethod.SetCall = Lib.Reflection.TypeHelper.GetSetFieldValue<ApiCall>(apicallName, newmethod.DeclareType);
            }

            try
            {
                lock (locker)
                {
                    currentlist[key] = newmethod;
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            return newmethod;
        }

        public static ApiMethod GetDynamicApiMethod(IDynamicApi instance, string methodName)
        {
            var api = instance as IApi;
            if (api == null)
            {
                return null;
            }

            var currentlist = List;

            if (string.IsNullOrEmpty(methodName))
            {
                return null;
            }

            string key = api.ModelName + "." + methodName;
            if (currentlist.ContainsKey(key))
            {
                return currentlist[key];
            }

            var info = instance.GetMethod(methodName);

            //var methodinfo = GetMethodInfo(instance.GetType(), MethodName);

            if (info == null || info.Method == null)
            {
                currentlist[key] = null;
                return null;
            }

            var newmethod = new ApiMethod
            {
                MethodName = info.Method.Name,
                ClassInstance = Activator.CreateInstance(info.Type),
                ReturnType = info.Method.ReturnType,
                DeclareType = info.Type,
                IsVoid = info.Method.ReturnType == typeof(void)
            };

            var acceptmodel = info.Method.GetCustomAttribute(typeof(Attributes.RequireModel), true);
            if (acceptmodel != null)
            {
                var accepted = acceptmodel as Attributes.RequireModel;
                newmethod.RequireModelType = accepted.ModelType;
            }

            var requirepara = info.Method.GetCustomAttribute(typeof(Attributes.RequireParameters));
            if (requirepara != null)
            {
                var requiredParas = requirepara as Attributes.RequireParameters;
                newmethod.RequireParas = requiredParas.Parameters;
            }

            if (newmethod.IsVoid)
            {
                newmethod.Void = Lib.Reflection.TypeHelper.CompileAction(info.Method);
            }
            else
            {
                newmethod.Func = Lib.Reflection.TypeHelper.CompileFunc(info.Method);
            }

            var paras = info.Method.GetParameters();
            foreach (var item in paras)
            {
                newmethod.Parameters.Add(new Parameter() { Name = item.Name, ClrType = item.ParameterType });
            }

            //set set call function.
            var apicallName = GetApiCallName(newmethod.DeclareType);
            if (!string.IsNullOrEmpty(apicallName))
            {
                newmethod.SetCall = Lib.Reflection.TypeHelper.GetSetFieldValue<ApiCall>(apicallName, newmethod.DeclareType);
            }

            try
            {
                lock (locker)
                {
                    currentlist[key] = newmethod;
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            return newmethod;
        }

        public static string GetApiCallName(Type apiType)
        {
            foreach (var item in apiType.GetProperties())
            {
                if (item.CanRead && item.CanWrite && item.PropertyType.IsPublic && item.PropertyType == typeof(ApiCall))
                {
                    return item.Name;
                }
            }

            foreach (FieldInfo info in apiType.GetFields())
            {
                if (info.IsPublic && info.FieldType == typeof(ApiCall))
                {
                    return info.Name;
                }
            }
            return null;
        }

        public static object Execute(ApiMethod method, ApiCall request)
        {
            //  List<object> paras = new List<object>();
            //  paras.Add(request);

            var paras = BindParameters(method, request);

            if (method.SetCall == null)
            {
                if (method.IsVoid)
                {
                    method.Void(method.ClassInstance, paras.ToArray());
                    return true;
                }

                return method.Func(method.ClassInstance, paras.ToArray());
            }

            var cls = Activator.CreateInstance(method.DeclareType) as IApi;
            method.SetCall(cls, request);

            if (method.IsVoid)
            {
                method.Void(cls, paras.ToArray());
                return true;
            }

            return method.Func(cls, paras.ToArray());
        }

        public static List<object> BindParameters(ApiMethod method, ApiCall call)
        {
            var result = new List<object>();

            foreach (var item in method.Parameters)
            {
                if (item.ClrType == typeof(ApiCall))
                {
                    result.Add(call);
                }
                else if (item.ClrType.IsValueType || item.ClrType == typeof(string))
                {
                    var value = call.GetValue(item.Name);
                    if (value != null)
                    {
                        var rightvalue = Lib.Reflection.TypeHelper.ChangeType(value, item.ClrType);
                        result.Add(rightvalue);
                    }
                    else
                    {
                        result.Add(null);
                    }
                }
                else if (item.ClrType.IsClass)
                {
                    // try to assign the model.
                    result.Add(AssignModel(call, item.ClrType, item.Name));
                }
                else
                {
                    result.Add(null);
                }
            }

            return result;
        }

        public static object AssignModel(ApiCall call, Type modelType, string fieldname)
        {
            object model = null;

            var fieldvalue = call.Context.Request.GetValue(fieldname);

            if (!string.IsNullOrEmpty(fieldvalue))
            {
                try
                {
                    model = Lib.Helper.JsonHelper.Deserialize(fieldvalue, modelType);

                    return model;
                }
                catch (Exception)
                {
                }
            }

            var json = call.Context.Request.Body;

            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    model = Lib.Helper.JsonHelper.Deserialize(json, modelType);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                var req = call.Context.Request;
                Dictionary<string, string> values = new Dictionary<string, string>();

                foreach (var item in req.Forms.AllKeys)
                {
                    var value = req.Forms[item];
                    values[item] = value;
                }

                foreach (var item in req.QueryString.AllKeys)
                {
                    var value = req.QueryString[item];
                    values[item] = value;
                }

                if (values.Count() >= 0)
                {
                    var dictjson = Lib.Helper.JsonHelper.Serialize(values);
                    try
                    {
                        model = Lib.Helper.JsonHelper.Deserialize(dictjson, modelType);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return model;
        }
    }
}
//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Kooboo.Data.Language;

namespace Kooboo.Api.Methods
{
    public static class ApiMethodManager
    {
        private static object locker = new object();
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

        public static ApiMethod Get(IApi instance, string MethodName)
        {
            var currentlist = List;

            if (string.IsNullOrEmpty(MethodName))
            {
                return null;
            }
            string key = instance.ModelName + "." + MethodName;
            if (currentlist.ContainsKey(key))
            {
                return currentlist[key];
            }

            if (instance is IDynamicApi)
            {
                var dynamic = instance as IDynamicApi;
                return GetDynamicApiMethod(dynamic, MethodName); 
            }

            var methodinfo = Lib.Reflection.TypeHelper.GetMethodInfo(instance.GetType(), MethodName);

            if (methodinfo == null)
            {
                currentlist[key] = null;
                return null;
            }

            ApiMethod newmethod = new ApiMethod();
            newmethod.MethodName = methodinfo.Name;
            newmethod.ClassInstance = instance;
            newmethod.ReturnType = methodinfo.ReturnType;
            newmethod.DeclareType = instance.GetType();
            newmethod.IsVoid = methodinfo.ReturnType == typeof(void);

            var acceptmodel = methodinfo.GetCustomAttribute(typeof(Kooboo.Attributes.RequireModel), true);
            if (acceptmodel != null)
            {
                var Accepted = acceptmodel as Attributes.RequireModel;
                newmethod.RequireModelType = Accepted.ModelType;
            }

            var requirepara = methodinfo.GetCustomAttribute(typeof(Kooboo.Attributes.RequireParameters));
            if (requirepara != null)
            {
                var RequiredParas = requirepara as Attributes.RequireParameters;
                newmethod.RequireParas = RequiredParas.Parameters;
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
            string apicallName = GetApiCallName(newmethod.DeclareType);
            if (!string.IsNullOrEmpty(apicallName))
            {

                newmethod.SetCall = Kooboo.Lib.Reflection.TypeHelper.GetSetFieldValue<ApiCall>(apicallName, newmethod.DeclareType);
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
                string abc = ex.Message;
            }
            return newmethod;
        }


        public static ApiMethod GetDynamicApiMethod(IDynamicApi instance, string MethodName)
        {
            var api = instance as IApi;
            if (api == null)
            {
                return null;
            }

            var currentlist = List;

            if (string.IsNullOrEmpty(MethodName))
            {
                return null;
            }

            string key = api.ModelName + "." + MethodName;
            if (currentlist.ContainsKey(key))
            {
                return currentlist[key];
            }
              
            var info = instance.GetMethod(MethodName);

            //var methodinfo = GetMethodInfo(instance.GetType(), MethodName);

            if (info == null || info.Method == null)
            {
                currentlist[key] = null;
                return null;
            }

            ApiMethod newmethod = new ApiMethod();
            newmethod.MethodName = info.Method.Name;
            newmethod.ClassInstance = Activator.CreateInstance(info.Type);
            newmethod.ReturnType = info.Method.ReturnType;
            newmethod.DeclareType = info.Type;
            newmethod.IsVoid = info.Method.ReturnType == typeof(void);

            var acceptmodel = info.Method.GetCustomAttribute(typeof(Kooboo.Attributes.RequireModel), true);
            if (acceptmodel != null)
            {
                var Accepted = acceptmodel as Attributes.RequireModel;
                newmethod.RequireModelType = Accepted.ModelType;
            }

            var requirepara = info.Method.GetCustomAttribute(typeof(Kooboo.Attributes.RequireParameters));
            if (requirepara != null)
            {
                var RequiredParas = requirepara as Attributes.RequireParameters;
                newmethod.RequireParas = RequiredParas.Parameters;
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
            string apicallName = GetApiCallName(newmethod.DeclareType);
            if (!string.IsNullOrEmpty(apicallName))
            {

                newmethod.SetCall = Kooboo.Lib.Reflection.TypeHelper.GetSetFieldValue<ApiCall>(apicallName, newmethod.DeclareType);
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
                string abc = ex.Message;
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
                else
                {
                    return method.Func(method.ClassInstance, paras.ToArray());
                }
            }
            else
            {
                var cls = Activator.CreateInstance(method.DeclareType) as IApi;
                method.SetCall(cls, request);

                if (method.IsVoid)
                {
                    method.Void(cls, paras.ToArray());
                    return true;
                }
                else
                {
                    return method.Func(cls, paras.ToArray());
                }
            }
        }
         
        public static List<object> BindParameters(ApiMethod method, ApiCall call)
        {
            List<object> result = new List<object>();

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

        public static object AssignModel(ApiCall call, Type ModelType, string fieldname)
        {
            object model = null;

            var fieldvalue = call.Context.Request.GetValue(fieldname);

            if (!string.IsNullOrEmpty(fieldvalue))
            {
                try
                {
                    model = Lib.Helper.JsonHelper.Deserialize(fieldvalue, ModelType);

                    return model;
                }
                catch (Exception ex)
                {

                }
            }


            string json = call.Context.Request.Body;

            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    model = Lib.Helper.JsonHelper.Deserialize(json, ModelType);
                }
                catch (Exception ex)
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
                    string dictjson = Lib.Helper.JsonHelper.Serialize(values);
                    try
                    {
                        model = Lib.Helper.JsonHelper.Deserialize(dictjson, ModelType);
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }

            return model;
        }
         
    }
}

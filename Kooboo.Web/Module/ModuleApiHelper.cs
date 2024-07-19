//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using System.Reflection;
using Kooboo.Api;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;

namespace Kooboo.Module
{
    public static class ModuleApiHelper
    {
        public static T GetModuleByName<T>(string name)
        {
            var type = ModuleApiContainer.GetType(name);
            if (type != null)
            {
                object obj = Activator.CreateInstance(type, true);//根据类型创建实例
                return (T)obj;//类型转换并返回
            }
            return default(T);
        }

        public static ApiMethod GetApiMethod(ApiCall call)
        {
            // this is only for the sitemodule. 
            if (call.Context.WebSite == null)
            {
                return null;
            }

            ApiMethod method = null;

            var type = ModuleApiContainer.GetType(call.Command.ObjectType);

            if (type != null)
            {
                var instance = CreateInstance(type, call.Context) as ISiteModuleApi;
                if (instance != null)
                {
                    method = GetMethod(instance, call.Command.Method, call);
                    if (method != null)
                    {
                        return method;
                    }
                }
            }
            else
            {
                // try command and value. 
                var commandtype = ModuleApiContainer.GetType(call.Command.Method);

                if (commandtype != null)
                {
                    var instance = CreateInstance(commandtype, call.Context) as ISiteModuleApi;
                    if (instance != null)
                    {
                        method = GetMethod(instance, call.Command.Value, call);
                        if (method != null)
                        {
                            return method;
                        }
                    }

                }

            }

            return null;
        }

        public static System.Reflection.ConstructorInfo GetConstrutor(Type type)
        {
            var consturcs = type.GetConstructors();
            if (!consturcs.Any())
            {
                return null;
            }
            foreach (var item in consturcs)
            {
                var para = item.GetParameters();
                if (para.Any())
                {
                    return item;
                }
            }

            return consturcs.First();

        }

        public static object[] InitConstructors(ParameterInfo[] parainfos, RenderContext context)
        {
            List<object> para = new List<object>();
            foreach (var item in parainfos)
            {
                var value = CreateInstance(item.ParameterType, context);
                para.Add(value);
            }
            return para.ToArray();
        }

        public static object CreateInstance(Type clrtype, RenderContext context)
        {
            if (clrtype == typeof(string))
            {
                return "";
            }
            else if (clrtype == typeof(byte) || clrtype == typeof(int) || clrtype == typeof(Int16) || clrtype == typeof(long) || clrtype == typeof(decimal) || clrtype == typeof(double) || clrtype == typeof(float))
            {
                return Convert.ChangeType(0, clrtype);
            }
            else if (clrtype == typeof(Guid))
            {
                return default(Guid);
            }

            else if (clrtype == typeof(bool))
            {
                return false;
            }
            else if (clrtype == typeof(DateTime))
            {
                return DateTime.Now;
            }

            else if (clrtype.IsClass)
            {
                var constructor = GetConstrutor(clrtype);
                var paras = constructor.GetParameters();
                if (paras.Any())
                {
                    var paravalues = InitConstructors(paras, context);
                    var obj = Activator.CreateInstance(clrtype, paravalues);
                    FixSpecialObj(obj, context);
                    return obj;
                }
                else
                {
                    var obj = Activator.CreateInstance(clrtype);
                    FixSpecialObj(obj, context);
                    return obj;
                }
            }
            return null;
        }

        public static void FixSpecialObj(object obj, RenderContext context)
        {
            if (obj == null)
            {
                return;
            }
            var objtype = obj.GetType();
            /// fix SiteRepositoryBase.
            if (IsSubclassSiteRepository(objtype))
            {
                //assign the site DB and init method...
                PropertyInfo propertyInfo = objtype.GetProperty("WebSite");
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(obj, context.WebSite);
                }
                // invoke init...  
                MethodInfo methodInfo = objtype.GetMethod("init");

                if (methodInfo != null)
                {
                    methodInfo.Invoke(obj, null);
                }
            }
        }

        public static bool IsSubclassSiteRepository(Type toCheck)
        {
            Type generic = typeof(Kooboo.Sites.Repository.SiteRepositoryBase<>);

            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }


        public static ApiMethod GetMethod(ISiteModuleApi instance, string MethodName, ApiCall call)
        {
            if (instance == null)
            {
                return null;
            }

            if (instance is SiteModuleApiBase)
            {
                var basemodule = instance as SiteModuleApiBase;
                basemodule.Context = call.Context;
                basemodule.SiteDb = call.Context.WebSite.SiteDb();
            }

            var methodinfo = GetMethodInfo(instance.GetType(), MethodName);

            if (methodinfo == null)
            {
                return null;
            }

            ApiMethod newmethod = new ApiMethod();
            newmethod.MethodName = methodinfo.Name;
            newmethod.ClassInstance = instance;
            newmethod.ReturnType = methodinfo.ReturnType;
            newmethod.DeclareType = instance.GetType();
            newmethod.IsVoid = methodinfo.ReturnType == typeof(void);

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

            return newmethod;
        }

        private static MethodInfo GetMethodInfo(Type type, string methodname)
        {
            var method = type.GetMethod(methodname);
            if (method != null)
            {
                return method;
            }

            var lowername = methodname.ToLower();

            var allmethods = Kooboo.Lib.Reflection.TypeHelper.GetPublicMethods(type);
            foreach (var item in allmethods)
            {
                if (item.Name.ToLower() == lowername)
                {
                    return item;
                }
            }

            return null;
        }



    }
}

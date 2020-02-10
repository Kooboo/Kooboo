using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Lib.Reflection;
using KScript.Sites;

namespace KScript.KscriptConfig
{
    public static class KscriptContextExtension
    {
        public static object CopyTo(this RenderContext renderContext, Type type)
        {
            var instance = Activator.CreateInstance(type);
            instance = Copy(renderContext,instance);
            //sepcial setting method
            var getSetting = type.GetField("GetSetting");
            if (getSetting != null)
            {
                Func<Type, object> getSettingFunc = renderContext.GetSetting;
                getSetting.SetValue(instance, getSettingFunc);
            }
            var findContent = type.GetField("FindTextContext");
            if (findContent != null)
            {
                Func<string, object> findContextFunc = renderContext.FindContext;
                findContent.SetValue(instance, findContextFunc);
            }
            return instance;
        }

        public static object GetSetting(this RenderContext renderContext, Type type)
        {
            if (renderContext.WebSite != null && renderContext.WebSite.OrganizationId != default(Guid))
            {
                var sitedb = renderContext.WebSite.SiteDb();
                var setting = sitedb.CoreSetting.GetSiteSetting(type);
                if (setting != null)
                {
                    return setting;
                }
            }
            return Activator.CreateInstance(type);
        }

        public static IDictionary<string,object> FindContext(this RenderContext renderContext, string query)
        {            
            var textcontents = new TextContentObjectRepository(renderContext.WebSite.SiteDb().TextContent, renderContext);
            return textcontents.Find(query);
        }

        /// <summary>
        /// copy object to another object
        /// </summary>
        /// <param name="copyFrom"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private static object Copy(object copyFrom, object instance)
        {
            if (copyFrom == null) return instance;

            #region map method to Action/Func field

            //GetMethods doesn't return extension method.
            var fromMethods = copyFrom.GetType().GetMethods().ToList();
            //var extensionMethods = GetExtensionMethods(typeof(KscriptContextExtension).Assembly,copyFrom.GetType());
            //fromMethods.AddRange(extensionMethods);

            var fields = instance.GetType().GetFields().ToList();
            foreach (var field in fields)
            {
                var matchMethod = fromMethods.Find(m => IsSameMethod(field,m));
                if (matchMethod == null) continue;

                var delegateMethod = matchMethod.CreateDelegate(field.FieldType,copyFrom);
                field.SetValue(instance, delegateMethod);
            }
            #endregion

            #region map properties
            var fromProperties = copyFrom.GetType().GetProperties().ToList();
            var properties = instance.GetType().GetProperties().ToList();
            foreach (var prop in properties)
            {
                var matchProp = fromProperties.Find(p => p.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                if (matchProp == null)
                {
                    continue;
                }
                var value = matchProp.GetValue(copyFrom);
                if (prop.PropertyType == matchProp.PropertyType)
                {
                    prop.SetValue(instance, value);
                }
                else 
                {
                    var matchPropertyInstance = Activator.CreateInstance(prop.PropertyType);
                    matchPropertyInstance= Copy(value,matchPropertyInstance);
                    //need convert type
                    prop.SetValue(instance, TypeHelper.ChangeType(matchPropertyInstance,prop.PropertyType));
                }
            }
            #endregion


            return TypeHelper.ChangeType(instance,instance.GetType());
        }

        //private static List<MethodInfo> GetExtensionMethods(Assembly assembly,Type extensionType)
        //{
        //    var query = from type in assembly.GetTypes()
        //                where !type.IsGenericType && !type.IsNested
        //                from method in type.GetMethods(BindingFlags.Static
        //                    | BindingFlags.Public | BindingFlags.NonPublic)
        //                where method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
        //                where method.GetParameters()[0].ParameterType == extensionType
        //                select method;
        //    return query.ToList();
        //}

        private static bool IsSameMethod(FieldInfo field, MethodInfo methodInfo)
        {
            if (field.FieldType.IsGenericType)
            {
                if (!IsAction(field.FieldType) && !IsFunc(field.FieldType)) return false;

                var fieldParams = field.FieldType.GenericTypeArguments;
                var parameters = methodInfo.GetParameters().Select(p => p.ParameterType).ToList();
                if (IsFunc(field.FieldType))
                {
                    parameters.Add(methodInfo.ReflectedType);
                }

                if (fieldParams.Length != parameters.Count) return false;

                var same = true;
                for (var i = 0; i < fieldParams.Length; i++)
                {
                    if (parameters[i] != fieldParams[i])
                    {
                        same = false;
                        break;
                    }
                }

                return same;

            }

            return false;
        }

        private static bool IsAction(Type type)
        {
            var types = new List<Type> {
                typeof(Action),typeof(Action<>),typeof(Action<,>),typeof(Action<,,>)
                ,typeof(Action<,,,>),typeof(Action<,,,,>)
            };

            return type.IsGenericType && types.Contains(type.GetGenericTypeDefinition());
        }

        private static bool IsFunc(Type type)
        {
            var types = new List<Type> {
                typeof(Func<>),typeof(Func<,>),typeof(Func<,,>),typeof(Func<,,,>)
                ,typeof(Func<,,,,>),typeof(Func<,,,,,>)
            };

            return type.IsGenericType && types.Contains(type.GetGenericTypeDefinition());
        }

    }
}

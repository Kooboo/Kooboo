using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System.Net;
using Kooboo.Lib.Reflection;

namespace Kooboo.Sites.KscriptConfig
{
    public static class KscriptContextExtension
    {
        public static object ToKscriptContext(this RenderContext renderContext, Type kscriptContextType)
        {
            if (kscriptContextType == null) return null;
            var instance = Activator.CreateInstance(kscriptContextType);
            if (instance == null || renderContext == null) return null;

            var requestProp = kscriptContextType.GetProperty("Request");
            if (requestProp != null)
            {
                var requestInstance = Activator.CreateInstance(requestProp.PropertyType);
                
                SetPropertyData(renderContext.Request, requestInstance);
                requestProp.SetValue(instance, TypeHelper.ChangeType(requestInstance, requestProp.PropertyType));
            }

            var responseProp = kscriptContextType.GetProperty("Response");
            if (responseProp != null)
            {
                var responseInstance = Activator.CreateInstance(responseProp.PropertyType);
                responseProp.SetValue(instance, responseInstance);

                //to do chang to set dynamic method
                var addCookie = responseProp.PropertyType.GetField("AddCookie");
                if (addCookie != null)
                {
                    Action<Cookie> action = (Cookie cookie) =>
                     {
                         renderContext.Response.AddCookie(cookie);
                     };
                    addCookie.SetValue(responseInstance, action);
                }
                var appendCookie = responseProp.PropertyType.GetField("AppendCookie");
                if (appendCookie != null)
                {
                    Action<string,string,DateTime> action = (string cookieName, string value, DateTime expire) =>
                    {
                        renderContext.Response.AppendCookie(cookieName,value,expire);
                    };
                    appendCookie.SetValue(responseInstance, action);
                }
                responseProp.SetValue(instance, TypeHelper.ChangeType(responseInstance, responseProp.PropertyType));
                //SetPropertyData(renderContext.Response, responseInstance);
            }

            var getSetting = kscriptContextType.GetField("GetSetting");
            if (getSetting != null)
            {
                Func<Type, object> getSettingFunc = (Type type) =>
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
                  };

                getSetting.SetValue(instance, getSettingFunc);
            }
            return TypeHelper.ChangeType(instance, kscriptContextType);
            //return instance;
        }


        public static void SetPropertyData(object dataFrom, object dataTo)
        {
            var fromProperties = dataFrom.GetType().GetProperties().ToList();
            var toProperties = dataTo.GetType().GetProperties().ToList();

            foreach (var toProp in toProperties)
            {
                var fromProp = fromProperties.Find(p => p.Name == toProp.Name && p.PropertyType == toProp.PropertyType);
                if (fromProp == null) continue;

                var value = fromProp.GetValue(dataFrom);
                toProp.SetValue(dataTo, TypeHelper.ChangeType(value, fromProp.PropertyType));
            }

            //var fromMethods = dataFrom.GetType().GetMethods().ToList();
            //var toMethodFields = dataTo.GetType().GetFields().ToList();

            //foreach (var toMethodField in toMethodFields)
            //{
            //    var fromMethod = fromMethods.Find(m => m.Name == toMethodField.Name && IsSameMethod(toMethodField, m));
            //    if (fromMethod == null) continue;
            //    toMethodField.SetValue(dataTo, fromMethod);

            //}

        }

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

        public static bool IsAction(Type type)
        {
            var types = new List<Type> {
                typeof(Action),typeof(Action<>),typeof(Action<,>),typeof(Action<,,>)
                ,typeof(Action<,,,>),typeof(Action<,,,,>)
            };

            return type.IsGenericType && types.Contains(type.GetGenericTypeDefinition());
        }

        public static bool IsFunc(Type type)
        {
            var types = new List<Type> {
                typeof(Func<>),typeof(Func<,>),typeof(Func<,,>),typeof(Func<,,,>)
                ,typeof(Func<,,,,>),typeof(Func<,,,,,>)
            };

            return type.IsGenericType && types.Contains(type.GetGenericTypeDefinition());
        }

    }
}

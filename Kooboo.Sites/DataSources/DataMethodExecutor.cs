//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Cache;
using System.Collections;
using Kooboo.Data.Context;
using Kooboo.Sites.ViewModel;
using Kooboo.Sites.Scripting;
using Kooboo.Sites.Extensions;

namespace Kooboo.Sites.DataSources
{
    public static class DataMethodExecutor
    {
        public static object ExecuteViewDataMethod(Render.FrontContext context, ViewDataMethod ViewMethod, DataContext parentDataContext = null)
        {
            var CompiledMethod = CompileMethodCache.GetCompiledMethod(context.SiteDb, ViewMethod.MethodId);
            if (CompiledMethod == null)
            {
                return null;
            }

            var ParameterBindings = CompiledMethod.ParameterBindings;

            DataContext dataContext = parentDataContext == null ? context.RenderContext.DataContext : parentDataContext;

            if (CompiledMethod.IsKScript)
            {
                var dictparas = ParameterBinder.BindKScript(CompiledMethod.Parameters, ParameterBindings, dataContext);

                return Kooboo.Sites.Scripting.Manager.ExecuteDataSource(context.RenderContext, CompiledMethod.CodeId, dictparas);
            }

            List<object> paras = ParameterBinder.Bind(CompiledMethod.Parameters, ParameterBindings, dataContext);

            CheckAndAssignDefaultValue(paras, CompiledMethod, context, ViewMethod.MethodId);
              

            var result = Execute(CompiledMethod, paras.ToArray(), context);
            if (result == null)
            {
                return null;
            }
            if (ViewMethod.HasChildren)
            {
                var type = result.GetType();

                if (Kooboo.Lib.Reflection.TypeHelper.IsGenericCollection(type))
                {
                    List<DataMethodResult> results = new List<DataMethodResult>();
                    var itemcollection = ((IEnumerable)result).Cast<object>().ToList();
                    foreach (var item in itemcollection)
                    {
                        var itemresult = ExecuteSubViewDataMethod(context, item, ViewMethod.Children);

                        results.Add(itemresult);
                    }
                    return results;
                }
                else if (result is PagedResult)
                {
                    var pagedresult = result as PagedResult;

                    List<DataMethodResult> results = new List<DataMethodResult>();
                    var itemcollection = ((IEnumerable)pagedresult.DataList).Cast<object>().ToList();
                    foreach (var item in itemcollection)
                    {
                        var itemresult = ExecuteSubViewDataMethod(context, item, ViewMethod.Children);

                        results.Add(itemresult);
                    }

                    pagedresult.DataList = results.ToList<object>();

                    return pagedresult;
                }
                else
                {
                    return ExecuteSubViewDataMethod(context, result, ViewMethod.Children);
                }
            }
            else
            {
                return result;
            }
        }
          
        internal static object Execute(Render.FrontContext context, DataMethodCompiled CompiledMethod)
        {
            List<object> paras = ParameterBinder.Bind(CompiledMethod.Parameters, CompiledMethod.ParameterBindings, context.RenderContext.DataContext);

            CheckAndAssignDefaultValue(paras, CompiledMethod, context, default(Guid));
            return Execute(CompiledMethod, paras.ToArray(), context);
        }

        internal static object Execute(DataMethodCompiled method, object[] Paras, Render.FrontContext Context)
        {
            // assign default values.

            object result;

            if (method.IsStatic)
            {
                if (method.IsVoid)
                {
                    method.StaticVoid(Paras);
                    return null;
                }
                else
                {
                    result = method.StaticFunc(Paras);
                }
            }
            else
            {
                var instance = Activator.CreateInstance(method.DeclareType);
                if (instance is SiteDataSource)
                {
                    var datasource = instance as SiteDataSource;
                    datasource.Context = Context;
                    if (method.IsVoid)
                    {
                        method.Void(datasource, Paras);
                        return null;
                    }
                    else
                    {
                        result = method.Func(datasource, Paras);
                    }
                }
                else
                {
                    if (method.IsVoid)
                    {
                        method.Void(instance, Paras);
                        return null;
                    }
                    else
                    {
                        result = method.Func(instance, Paras);
                    }
                }
            }

            if (result == null)
            { return null; }
            else
            {
                var resultAsTask = result as Task;
                if (resultAsTask != null)
                {
                    if (method.ReturnType == typeof(Task))
                    {
                        ThrowIfWrappedTaskInstance(resultAsTask.GetType(), "Not getted", method.DeclareType);
                        resultAsTask.Wait();
                        return null;
                    }

                    var taskValueType = GetTaskInnerTypeOrNull(method.ReturnType);
                    if (taskValueType != null)
                    {
                        // for: public Task<T> Action()
                        // constructs: return (Task<object>)Convert<T>((Task<T>)result)
                        var genericMethodInfo = _convertOfTMethod.MakeGenericMethod(taskValueType);
                        var convertedResult = genericMethodInfo.Invoke(null, new object[] { result });
                        return convertedResult;
                    }

                    // This will be the case for:
                    // 1. Types which have derived from Task and Task<T>,
                    // 2. Action methods which use dynamic keyword but return a Task or Task<T>.
                    throw new InvalidOperationException(
                        String.Format("The method '{0}' on type '{1}' returned a Task instance even though it is not an asynchronous method.",
                            "Not getted",
                            method.DeclareType
                        )
                    );
                }
                else
                {
                    return result;
                }
            }
        }


        private static DataMethodResult ExecuteSubViewDataMethod(Render.FrontContext Context, object itemvalue, List<ViewDataMethod> Children)
        {
            DataMethodResult dataresult = new DataMethodResult();
            dataresult.Value = itemvalue;

            DataContext parentcontext = new DataContext(Context.RenderContext);
            parentcontext.Push("", itemvalue);

            foreach (var item in Children)
            {
                var subResult = ExecuteViewDataMethod(Context, item, parentcontext);

                if (subResult != null)
                {
                    if (subResult is DataMethodResult)
                    {
                        dataresult.Children.Add(item.AliasName, subResult as DataMethodResult);
                    }
                    else
                    {
                        DataMethodResult subMethodResult = new DataMethodResult();
                        subMethodResult.Value = subResult;
                        dataresult.Children.Add(item.AliasName, subMethodResult);
                    }
                }
            }
            return dataresult;

        }

        private static List<DataMethodResult> ConvertToMethodResultList(object data)
        {
            List<DataMethodResult> result = new List<DataMethodResult>();
            var itemcollection = ((IEnumerable)data).Cast<object>().ToList();
            foreach (var item in itemcollection)
            {
                DataMethodResult oneresult = new DataMethodResult();
                oneresult.Value = item;
            }
            return null;
        }

        internal static void CheckAndAssignDefaultValue(List<object> values, DataMethodCompiled CompiledMethod, Render.FrontContext context, Guid CurrentMethodId)
        {
            if (!values.Where(o => o == null).Any())
            {
                return;
            }

            int count = CompiledMethod.Parameters.Count();

            bool IsTextContentMethod = CompiledMethod.ReturnType == typeof(TextContentViewModel) || Kooboo.Lib.Reflection.TypeHelper.GetGenericType(CompiledMethod.ReturnType) == typeof(TextContentViewModel);

            TextContentViewModel samplecontent = null;
            if (IsTextContentMethod)
            {
                var folderid = TryGetFolderGuid(CompiledMethod.ParameterBindings);
                samplecontent = context.SiteDb.TextContent.GetDefaultContentFromFolder(folderid, context.RenderContext.Culture);
            }

            bool IsByCategory = IsQueryByCategory(CompiledMethod);


            for (int i = 0; i < count; i++)
            {
                if (values[i] == null)
                {
                    var paraname = CompiledMethod.Parameters.Keys.ToList()[i];

                    if (IsByCategory)
                    {
                        if (paraname.ToLower() == "id")
                        {
                            values[i] = default(Guid);
                            continue;
                        }
                        else if (paraname.ToLower() == "userkey")
                        {
                            values[i] = string.Empty;
                            continue;
                        }
                    }

                    if (samplecontent != null)
                    {
                        var key = GetBindingKey(paraname, CompiledMethod.ParameterBindings);
                        var value = Kooboo.Lib.Reflection.Dynamic.GetObjectMember(samplecontent, key);

                        if (value != null)
                        {
                            values[i] = value;
                        }
                    }

                    if (values[i] == null)
                    {
                        var x = CompiledMethod.Parameters.Keys.ToList()[i];
                        var paratype = CompiledMethod.Parameters[x];
                        values[i] = GetDefaultValueForDataType(paratype);
                    }
                }

            }

        }

        private static bool IsQueryByCategory(DataMethodCompiled CompiledMethod)
        {
            if (!string.IsNullOrEmpty(CompiledMethod.OriginalMethodName))
            {
                if (CompiledMethod.OriginalMethodName.ToLower().Contains("bycategory"))
                {
                    return true;
                }
            }
            return false;
        }

        private static object GetDefaultValue(DataMethodCompiled CompiledMethod, Render.FrontContext context, Guid CurrentMethodId, string paraname, Guid folderid)
        {
            object value = null;

            var defaultcontent = GetDefaultContentNew(folderid, context, CurrentMethodId);
            if (defaultcontent != null)
            {
                var key = GetBindingKey(paraname, CompiledMethod.ParameterBindings);
                value = Kooboo.Lib.Reflection.Dynamic.GetObjectMember(defaultcontent, key);
            }

            return value;
        }

        internal static object GetDefaultValueForDataType(string typename)
        {
            if (string.IsNullOrEmpty(typename))
            { return null; }
            var type = Kooboo.Lib.Reflection.TypeHelper.GetType(typename);
            if (type == typeof(System.Guid))
            {
                return default(Guid);
            }
            else if (type == typeof(int) || type == typeof(Int16) || type == typeof(Int64))
            {
                return 0;
            }
            else if (type == typeof(DateTime))
            {
                return DateTime.Now;
            }
            else if (type == typeof(bool))
            {
                return false;
            }

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        private static string GetBindingKey(string Paraname, Dictionary<string, ParameterBinding> bindings)
        {
            Paraname = Paraname.ToLower();
            foreach (var item in bindings)
            {
                if (item.Key.ToLower() == Paraname)
                {
                    var value = item.Value.Binding;

                    if (value.Contains("{") && value.Contains("}"))
                    {
                        value = value.Replace("{", "");
                        value = value.Replace("}", "");
                    }
                    return value;
                }
            }
            return null;
        }


        internal static TextContentViewModel GetDefaultContentNew(Guid FolderId, Render.FrontContext context, Guid CurrentMethodId = default(Guid))
        {
            List<DataMethodSetting> CorrectMethods = new List<DataMethodSetting>();

            var allmethods = context.SiteDb.DataMethodSettings.GetByFolder(FolderId);

            foreach (var item in allmethods)
            {
                if (item.Id != CurrentMethodId)
                {
                    var type = Kooboo.Data.TypeCache.GetType(item.ReturnType);

                    if (type != null && Kooboo.Lib.Reflection.TypeHelper.IsGenericCollection(type))
                    {
                        CorrectMethods.Add(item);
                    }
                }
            }
            if (CorrectMethods.Count > 0)
            {
                // first execute on the same page...
                if (context.Page != null)
                {
                    var pagemethods = context.SiteDb.Pages.GetAllMethodIds(context.Page.Id);

                    var withinmethod = CorrectMethods.FindAll(o => pagemethods.Contains(o.Id));

                    foreach (var item in withinmethod)
                    {
                        var result = DataMethodExecutor.ExecuteDataMethod(context, item.Id);
                        if (result != null)
                        {
                            var itemcollection = ((IEnumerable)result).Cast<object>().ToList();

                            if (itemcollection != null && itemcollection.Count() > 0)
                            {
                                var contentitem = itemcollection[0];

                                if (contentitem is TextContentViewModel)
                                {
                                    return contentitem as TextContentViewModel;
                                }
                            }
                        }
                        CorrectMethods.Remove(item);
                    }

                    ///execute pages that link to current page... 
                    var allotherpages = context.SiteDb.Relations.GetReferredBy(context.Page, ConstObjectType.Page);

                    foreach (var item in allotherpages)
                    {
                        var otherpagemethods = context.SiteDb.Pages.GetAllMethodIds(item.objectXId);

                        var otherwithinmethod = CorrectMethods.FindAll(o => otherpagemethods.Contains(o.Id));

                        foreach (var otheritem in otherwithinmethod)
                        {
                            var result = DataMethodExecutor.ExecuteDataMethod(context, otheritem.Id);
                            if (result != null)
                            {
                                var itemcollection = ((IEnumerable)result).Cast<object>().ToList();

                                if (itemcollection != null && itemcollection.Count() > 0)
                                {
                                    var contentitem = itemcollection[0];

                                    if (contentitem is TextContentViewModel)
                                    {
                                        return contentitem as TextContentViewModel;
                                    }
                                }
                            }
                            CorrectMethods.Remove(otheritem);
                        }
                    }
                }


                foreach (var item in CorrectMethods)
                {
                    var result = DataMethodExecutor.ExecuteDataMethod(context, item.Id);
                    if (result != null)
                    {
                        List<object> itemcollection; 

                        if (result is PagedResult)
                        {
                            var paged = result as PagedResult;
                            itemcollection = ((IEnumerable)paged.DataList).Cast<object>().ToList();

                            if (itemcollection != null && itemcollection.Count() > 0)
                            {
                                var contentitem = itemcollection[0];

                                if (contentitem is TextContentViewModel)
                                {
                                    return contentitem as TextContentViewModel;
                                }

                            }
                        }
                        else
                        { 
                             itemcollection = ((IEnumerable)result).Cast<object>().ToList();

                            if (itemcollection != null && itemcollection.Count() > 0)
                            {
                                var contentitem = itemcollection[0];

                                if (contentitem is TextContentViewModel)
                                {
                                    return contentitem as TextContentViewModel;
                                }

                            }
                        }

                    }
                }
            }

            return context.SiteDb.TextContent.GetDefaultContentFromFolder(FolderId, context.RenderContext.Culture);
        }


        private static Guid TryGetFolderGuid(Dictionary<string, ParameterBinding> bindings)
        {
            Guid folderid = default(Guid);
            foreach (var item in bindings)
            {
                var lower = item.Key.ToLower();
                if (lower == "folderid" || lower == "folder.id" || lower == "folder" || lower.Contains(".folderid"))
                {
                    var value = item.Value;
                    Guid.TryParse(value.Binding, out folderid);
                }
            }
            return folderid;
        }

        private static object ExecuteDataMethod(Render.FrontContext context, Guid MethodId)
        {
            var CompiledMethod = CompileMethodCache.GetCompiledMethod(context.SiteDb, MethodId);
            if (CompiledMethod == null)
            {
                return null;
            }

            var ParameterBindings = CompiledMethod.ParameterBindings;

            List<object> paras = ParameterBinder.Bind(CompiledMethod.Parameters, ParameterBindings, context.RenderContext.DataContext);

            CheckAndAssignDefaultValue(paras, CompiledMethod, context, MethodId);

            return Execute(CompiledMethod, paras.ToArray(), context);

        }

        private static void ThrowIfWrappedTaskInstance(Type actualTypeReturned, string methodName, Type declaringType)
        {
            // Throw if a method declares a return type of Task and returns an instance of Task<Task> or Task<Task<T>>
            // This most likely indicates that the developer forgot to call Unwrap() somewhere.
            if (actualTypeReturned != typeof(Task))
            {
                var innerTaskType = GetTaskInnerTypeOrNull(actualTypeReturned);
                if (innerTaskType != null && typeof(Task).IsAssignableFrom(innerTaskType))
                {
                    throw new InvalidOperationException(
                        String.Format("The method '{0}' on type '{1}' returned an instance of '{2}'.Make sure to call Unwrap on the returned value to avoid unobserved faulted Task.",
                            methodName,
                            declaringType,
                            actualTypeReturned.FullName
                        )
                    );
                }
            }
        }

        /// <summary>
        /// Cast Task to Task of object
        /// </summary>
        private static async Task<object> CastToObject(Task task)
        {
            await task;
            return null;
        }

        /// <summary>
        /// Cast Task of T to Task of object
        /// </summary>
        private static async Task<object> CastToObject<T>(Task<T> task)
        {
            return (object)await task;
        }

        private static Type GetTaskInnerTypeOrNull(Type type)
        {
            var genericType = ExtractGenericInterface(type, typeof(Task<>));

            return genericType?.GenericTypeArguments[0];
        }

        private static Type ExtractGenericInterface(Type queryType, Type interfaceType)
        {
            if (queryType == null)
            {
                throw new ArgumentNullException(nameof(queryType));
            }

            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            Func<Type, bool> matchesInterface =
                type => type.IsGenericType && type.GetGenericTypeDefinition() == interfaceType;
            if (matchesInterface(queryType))
            {
                // Checked type matches (i.e. is a closed generic type created from) the open generic type.
                return queryType;
            }

            // Otherwise check all interfaces the type implements for a match.
            return queryType.GetInterfaces().FirstOrDefault(matchesInterface);
        }

        private static readonly MethodInfo _convertOfTMethod =
            typeof(DataMethodExecutor).GetRuntimeMethods().Single(methodInfo => methodInfo.Name == "Convert");


        // Method called via reflection.
        public static object Convert<T>(object taskAsObject)
        {
            var task = (Task<T>)taskAsObject;
            return task.Result;
        }
    }


    public class DataMethodCompiled
    {
        public Action<object[]> StaticVoid;
        public Func<object[], object> StaticFunc;

        public Action<object, object[]> Void;
        public Func<object, object[], object> Func;

        public Guid MethodId { get; set; }
        public bool IsStatic { get; set; }
        public bool IsVoid { get; set; }

        public bool IsKScript { get; set; }

        public Guid CodeId { get; set; }

        public Type DeclareType { get; set; }
        public Type ReturnType { get; set; }

        public string OriginalMethodName { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public Dictionary<string, ParameterBinding> ParameterBindings { get; set; }

        public DataMethodCompiled(IDataMethodSetting methodsetting)
        {
            Type type = Kooboo.Data.TypeCache.GetType(methodsetting.DeclareType);
            this.DeclareType = type;

            if (methodsetting.IsKScript)
            {
                this.IsKScript = true;
                this.CodeId = methodsetting.CodeId;
                this.Parameters = methodsetting.Parameters;
                this.ParameterBindings = new Dictionary<string, ParameterBinding>();

                if (methodsetting.ParameterBinding != null)
                {
                    foreach (var item in methodsetting.ParameterBinding)
                    {
                        if (item.Key.ToLower() != Kooboo.Sites.DataSources.ScriptSourceManager.SampleResponseFieldName.ToLower())
                        {
                            this.ParameterBindings.Add(item.Key, item.Value); 
                        }
                    }
                }

            }
            else
            {

                Type returntype = Kooboo.Data.TypeCache.GetType(methodsetting.ReturnType);
                this.ReturnType = returntype;
                this.OriginalMethodName = methodsetting.OriginalMethodName;

                Parameters = methodsetting.Parameters;
                ParameterBindings = methodsetting.ParameterBinding;
                this.IsStatic = methodsetting.IsStatic;
                this.IsVoid = methodsetting.IsVoid;
                List<Type> paratypes = new List<Type>();

                foreach (var item in methodsetting.Parameters.Values)
                {
                    Type paratype = Kooboo.Data.TypeCache.GetType(item);
                    paratypes.Add(paratype);
                }

                if (methodsetting.IsStatic)
                {
                    if (methodsetting.IsVoid)
                    {
                        StaticVoid = Lib.Reflection.TypeHelper.CompileStaticAction(type, methodsetting.OriginalMethodName, paratypes);
                    }
                    else
                    {
                        StaticFunc = Lib.Reflection.TypeHelper.CompileStaticFunc(type, methodsetting.OriginalMethodName, paratypes);
                    }
                }
                else
                {
                    if (methodsetting.IsVoid)
                    {
                        Void = Lib.Reflection.TypeHelper.CompileAction(type, methodsetting.OriginalMethodName, paratypes);
                    }
                    else
                    {
                        Func = Lib.Reflection.TypeHelper.CompileFunc(type, methodsetting.OriginalMethodName, paratypes);
                    }
                }


            }


        }

    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.Cache;
using Kooboo.Sites.Models;
using Kooboo.Sites.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Kooboo.Sites.DataSources
{
    public static class DataMethodExecutor
    {
        public static object ExecuteViewDataMethod(Render.FrontContext context, ViewDataMethod viewMethod, DataContext parentDataContext = null)
        {
            var compiledMethod = CompileMethodCache.GetCompiledMethod(context.SiteDb, viewMethod.MethodId);
            if (compiledMethod == null)
            {
                return null;
            }

            var parameterBindings = compiledMethod.ParameterBindings;

            DataContext dataContext = parentDataContext ?? context.RenderContext.DataContext;

            if (compiledMethod.IsKScript)
            {
                var dictparas = ParameterBinder.BindKScript(compiledMethod.Parameters, parameterBindings, dataContext);

                return Kooboo.Sites.Scripting.Manager.ExecuteDataSource(context.RenderContext, compiledMethod.CodeId, dictparas);
            }

            List<object> paras = ParameterBinder.Bind(compiledMethod.Parameters, parameterBindings, dataContext);

            CheckAndAssignDefaultValue(paras, compiledMethod, context, viewMethod.MethodId);

            var result = Execute(compiledMethod, paras.ToArray(), context);
            if (result == null)
            {
                return null;
            }
            if (viewMethod.HasChildren)
            {
                var type = result.GetType();

                if (Kooboo.Lib.Reflection.TypeHelper.IsGenericCollection(type))
                {
                    List<DataMethodResult> results = new List<DataMethodResult>();
                    var itemcollection = ((IEnumerable)result).Cast<object>().ToList();
                    foreach (var item in itemcollection)
                    {
                        var itemresult = ExecuteSubViewDataMethod(context, item, viewMethod.Children);

                        results.Add(itemresult);
                    }
                    return results;
                }
                else if (result is PagedResult pagedresult)
                {
                    List<DataMethodResult> results = new List<DataMethodResult>();
                    var itemcollection = ((IEnumerable)pagedresult.DataList).Cast<object>().ToList();
                    foreach (var item in itemcollection)
                    {
                        var itemresult = ExecuteSubViewDataMethod(context, item, viewMethod.Children);

                        results.Add(itemresult);
                    }

                    pagedresult.DataList = results.ToList<object>();

                    return pagedresult;
                }
                else
                {
                    return ExecuteSubViewDataMethod(context, result, viewMethod.Children);
                }
            }
            else
            {
                return result;
            }
        }

        internal static object Execute(Render.FrontContext context, DataMethodCompiled compiledMethod)
        {
            List<object> paras = ParameterBinder.Bind(compiledMethod.Parameters, compiledMethod.ParameterBindings, context.RenderContext.DataContext);

            CheckAndAssignDefaultValue(paras, compiledMethod, context, default(Guid));
            return Execute(compiledMethod, paras.ToArray(), context);
        }

        internal static object Execute(DataMethodCompiled method, object[] paras, Render.FrontContext context)
        {
            // assign default values.

            object result;

            if (method.IsStatic)
            {
                if (method.IsVoid)
                {
                    method.StaticVoid(paras);
                    return null;
                }
                else
                {
                    result = method.StaticFunc(paras);
                }
            }
            else
            {
                var instance = Activator.CreateInstance(method.DeclareType);
                if (instance is SiteDataSource datasource)
                {
                    datasource.Context = context;
                    if (method.IsVoid)
                    {
                        method.Void(datasource, paras);
                        return null;
                    }
                    else
                    {
                        result = method.Func(datasource, paras);
                    }
                }
                else
                {
                    if (method.IsVoid)
                    {
                        method.Void(instance, paras);
                        return null;
                    }
                    else
                    {
                        result = method.Func(instance, paras);
                    }
                }
            }

            if (result == null)
            { return null; }
            else
            {
                if (result is Task resultAsTask)
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
                        var genericMethodInfo = ConvertOfTMethod.MakeGenericMethod(taskValueType);
                        var convertedResult = genericMethodInfo.Invoke(null, new object[] { result });
                        return convertedResult;
                    }

                    // This will be the case for:
                    // 1. Types which have derived from Task and Task<T>,
                    // 2. Action methods which use dynamic keyword but return a Task or Task<T>.
                    throw new InvalidOperationException(
                        $"The method '{"Not getted"}' on type '{method.DeclareType}' returned a Task instance even though it is not an asynchronous method."
                    );
                }
                else
                {
                    return result;
                }
            }
        }

        private static DataMethodResult ExecuteSubViewDataMethod(Render.FrontContext context, object itemvalue, List<ViewDataMethod> children)
        {
            DataMethodResult dataresult = new DataMethodResult {Value = itemvalue};

            DataContext parentcontext = new DataContext(context.RenderContext);
            parentcontext.Push("", itemvalue);

            foreach (var item in children)
            {
                var subResult = ExecuteViewDataMethod(context, item, parentcontext);

                if (subResult != null)
                {
                    if (subResult is DataMethodResult result)
                    {
                        dataresult.Children.Add(item.AliasName, result);
                    }
                    else
                    {
                        DataMethodResult subMethodResult = new DataMethodResult {Value = subResult};
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
                DataMethodResult oneresult = new DataMethodResult {Value = item};
            }
            return null;
        }

        internal static void CheckAndAssignDefaultValue(List<object> values, DataMethodCompiled compiledMethod, Render.FrontContext context, Guid currentMethodId)
        {
            if (!values.Any(o => o == null))
            {
                return;
            }

            int count = compiledMethod.Parameters.Count();
            var keylist = compiledMethod.Parameters.Keys.ToList();

            bool isContentList = compiledMethod.DeclareType == typeof(Kooboo.Sites.DataSources.ContentList);

            bool isContentQueried = false;

            bool isTextContentMethod = compiledMethod.ReturnType == typeof(TextContentViewModel) || Kooboo.Lib.Reflection.TypeHelper.GetGenericType(compiledMethod.ReturnType) == typeof(TextContentViewModel);

            TextContentViewModel samplecontent = null;

            bool isByCategory = IsQueryByCategory(compiledMethod);

            for (int i = 0; i < count; i++)
            {
                if (values[i] == null)
                {
                    var paraname = keylist[i];

                    if (isContentList)
                    {
                        //int PageSize, int PageNumber, string SortField, Boolean IsAscending
                        if (paraname == "PageSize" || paraname == "PageNumber" || paraname == "SortField" || paraname == "IsAscending")
                        {
                            var x = keylist[i];
                            var paratype = compiledMethod.Parameters[x];
                            values[i] = GetDefaultValueForDataType(paratype);
                        }
                    }

                    if (isByCategory)
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

                    if (!isContentQueried)
                    {
                        if (isTextContentMethod)
                        {
                            var folderid = TryGetFolderGuid(compiledMethod.ParameterBindings);
                            samplecontent = context.SiteDb.TextContent.GetDefaultContentFromFolder(folderid, context.RenderContext.Culture);
                        }
                        isContentQueried = true;
                    }

                    if (samplecontent != null)
                    {
                        var key = GetBindingKey(paraname, compiledMethod.ParameterBindings);
                        var value = Kooboo.Lib.Reflection.Dynamic.GetObjectMember(samplecontent, key);

                        if (value != null)
                        {
                            values[i] = value;
                        }
                    }

                    if (values[i] == null)
                    {
                        var x = keylist[i];
                        var paratype = compiledMethod.Parameters[x];
                        values[i] = GetDefaultValueForDataType(paratype);
                    }
                }
            }
        }

        private static bool IsQueryByCategory(DataMethodCompiled compiledMethod)
        {
            if (!string.IsNullOrEmpty(compiledMethod.OriginalMethodName))
            {
                if (compiledMethod.OriginalMethodName.ToLower().Contains("bycategory"))
                {
                    return true;
                }
            }
            return false;
        }

        private static object GetDefaultValue(DataMethodCompiled compiledMethod, Render.FrontContext context, Guid currentMethodId, string paraname, Guid folderid)
        {
            object value = null;

            var defaultcontent = GetDefaultContentNew(folderid, context, currentMethodId);
            if (defaultcontent != null)
            {
                var key = GetBindingKey(paraname, compiledMethod.ParameterBindings);
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

        private static string GetBindingKey(string paraname, Dictionary<string, ParameterBinding> bindings)
        {
            paraname = paraname.ToLower();
            foreach (var item in bindings)
            {
                if (item.Key.ToLower() == paraname)
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

        internal static TextContentViewModel GetDefaultContentNew(Guid folderId, Render.FrontContext context, Guid currentMethodId = default(Guid))
        {
            List<DataMethodSetting> correctMethods = new List<DataMethodSetting>();

            var allmethods = context.SiteDb.DataMethodSettings.GetByFolder(folderId);

            foreach (var item in allmethods)
            {
                if (item.Id != currentMethodId)
                {
                    var type = Kooboo.Data.TypeCache.GetType(item.ReturnType);

                    if (type != null && Kooboo.Lib.Reflection.TypeHelper.IsGenericCollection(type))
                    {
                        correctMethods.Add(item);
                    }
                }
            }
            if (correctMethods.Count > 0)
            {
                // first execute on the same page...
                if (context.Page != null)
                {
                    var pagemethods = context.SiteDb.Pages.GetAllMethodIds(context.Page.Id);

                    var withinmethod = correctMethods.FindAll(o => pagemethods.Contains(o.Id));

                    foreach (var item in withinmethod)
                    {
                        var result = DataMethodExecutor.ExecuteDataMethod(context, item.Id);
                        var itemcollection = ((IEnumerable) result)?.Cast<object>().ToList();

                        if (itemcollection != null && itemcollection.Any())
                        {
                            var contentitem = itemcollection[0];

                            if (contentitem is TextContentViewModel model)
                            {
                                return model;
                            }
                        }
                        correctMethods.Remove(item);
                    }

                    //execute pages that link to current page...
                    var allotherpages = context.SiteDb.Relations.GetReferredBy(context.Page, ConstObjectType.Page);

                    foreach (var item in allotherpages)
                    {
                        var otherpagemethods = context.SiteDb.Pages.GetAllMethodIds(item.objectXId);

                        var otherwithinmethod = correctMethods.FindAll(o => otherpagemethods.Contains(o.Id));

                        foreach (var otheritem in otherwithinmethod)
                        {
                            var result = DataMethodExecutor.ExecuteDataMethod(context, otheritem.Id);
                            var itemcollection = ((IEnumerable) result)?.Cast<object>().ToList();

                            if (itemcollection != null && itemcollection.Any())
                            {
                                var contentitem = itemcollection[0];

                                if (contentitem is TextContentViewModel model)
                                {
                                    return model;
                                }
                            }
                            correctMethods.Remove(otheritem);
                        }
                    }
                }

                foreach (var item in correctMethods)
                {
                    var result = DataMethodExecutor.ExecuteDataMethod(context, item.Id);
                    if (result != null)
                    {
                        List<object> itemcollection;

                        if (result is PagedResult)
                        {
                            var paged = result as PagedResult;
                            itemcollection = ((IEnumerable)paged.DataList).Cast<object>().ToList();

                            if (itemcollection != null && itemcollection.Any())
                            {
                                var contentitem = itemcollection[0];

                                if (contentitem is TextContentViewModel textContentViewModel)
                                {
                                    return textContentViewModel;
                                }
                            }
                        }
                        else
                        {
                            itemcollection = ((IEnumerable)result).Cast<object>().ToList();

                            if (itemcollection != null && itemcollection.Any())
                            {
                                var contentitem = itemcollection[0];

                                if (contentitem is TextContentViewModel textContentViewModel)
                                {
                                    return textContentViewModel;
                                }
                            }
                        }
                    }
                }
            }

            return context.SiteDb.TextContent.GetDefaultContentFromFolder(folderId, context.RenderContext.Culture);
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
                    if (Guid.TryParse(value.Binding, out folderid))
                    {
                        return folderid;
                    }
                }
            }
            return folderid;
        }

        private static object ExecuteDataMethod(Render.FrontContext context, Guid methodId)
        {
            var compiledMethod = CompileMethodCache.GetCompiledMethod(context.SiteDb, methodId);
            if (compiledMethod == null)
            {
                return null;
            }

            var parameterBindings = compiledMethod.ParameterBindings;

            List<object> paras = ParameterBinder.Bind(compiledMethod.Parameters, parameterBindings, context.RenderContext.DataContext);

            CheckAndAssignDefaultValue(paras, compiledMethod, context, methodId);

            return Execute(compiledMethod, paras.ToArray(), context);
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
                        $"The method '{methodName}' on type '{declaringType}' returned an instance of '{actualTypeReturned.FullName}'.Make sure to call Unwrap on the returned value to avoid unobserved faulted Task."
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

        private static readonly MethodInfo ConvertOfTMethod =
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
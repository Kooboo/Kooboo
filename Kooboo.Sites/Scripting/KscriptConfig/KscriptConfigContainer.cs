using System;
using System.Collections.Generic;
using Kooboo.Data;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Lib.Reflection;

namespace KScript.KscriptConfig
{
    public static class KscriptConfigContainer
    {
        private static object _lockObj = new object();

        private static Type kscriptContextType;
        public static Type KscriptContextType
        {
            get
            {
                var config = AppSettings.KscriptConfig;
                if (config == null && string.IsNullOrEmpty(config.KscriptContext)) return null;

                if (kscriptContextType == null)
                {
                    lock (_lockObj)
                    {
                        if (kscriptContextType == null)
                        {
                            LoadExtensionDll(config.ExtensionDlls);
                            kscriptContextType = Kooboo.Lib.Reflection.AssemblyLoader.LoadTypeByFullClassName(config.KscriptContext);
                        }
                    }
                }

                return kscriptContextType;
            }
        }

        private static Dictionary<string, Type> kscriptSetting;
        public static Dictionary<string, Type> KscriptSettings
        {
            get
            {
                var config = AppSettings.KscriptConfig;
                if (config == null || string.IsNullOrEmpty(config.KscriptSetting)) return new Dictionary<string, Type>();

                if (kscriptSetting == null)
                {
                    lock (_lockObj)
                    {
                        if (kscriptSetting == null)
                        {
                            LoadExtensionDll(config.ExtensionDlls);
                            kscriptSetting = GetKscriptSetting(config.KscriptSetting);
                        }
                    }
                }

                return kscriptSetting;
            }
        }
        /// <summary>
        /// data 
        /// key:koobootest
        /// value:typeof(KNamespaceTypeManager)
        /// </summary>
        private static Dictionary<string, Type> _list;
        public static Dictionary<string, Type> List
        {
            get
            {
                if (_list == null)
                {
                    lock (_lockObj)
                    {
                        if (_list == null)
                        {
                            _list = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                            var config = AppSettings.KscriptConfig;
                            if (config == null) return List;

                            //dll in base directory,need be loaded
                            LoadExtensionDll(config.ExtensionDlls);

                            foreach (var item in config.Kscripts)
                            {
                                try
                                {
                                    var kscriptType = Kooboo.Lib.Reflection.AssemblyLoader.LoadTypeByFullClassName(item.Value);
                                    if (kscriptType == null) continue;

                                    var ns = GetTopLevelNamespace(item.NameSpace);
                                    if (!_list.ContainsKey(ns))
                                    {
                                        //get ksript toplevel namespace
                                        _list[ns] = typeof(KScriptConfigType);
                                    }

                                    //get type with namespace
                                    var kscriptTypes = GetTypes(kscriptType);

                                    foreach (var type in kscriptTypes)
                                    {
                                        var name = string.IsNullOrEmpty(item.Name) ? type.Name : item.Name;
                                        var fullname = string.Format("{0}.{1}", item.NameSpace, name);
                                        NamespaceTypeDic[fullname] = type;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Kooboo.Data.Log.Instance.Exception.Write(ex.Message + ex.Source); 
                                }
                            }
                        }
                    }
                }
                return _list;
            }
        }

        private static Dictionary<string, Type[]> kscriptConfigTypes;
        public static Dictionary<string, Type[]> KscriptConfigTypes
        {
            get
            {
                if (kscriptConfigTypes == null)
                {
                    lock (_lockObj)
                    {
                        if (kscriptConfigTypes == null)
                        {
                            kscriptConfigTypes = new Dictionary<string, Type[]>(StringComparer.OrdinalIgnoreCase);
                            var config = AppSettings.KscriptConfig;
                            if (config == null) return kscriptConfigTypes;

                            //dll in base directory,need be loaded
                            LoadExtensionDll(config.ExtensionDlls);

                            foreach (var item in config.Kscripts)
                            {
                                try
                                {
                                    var kscriptType = AssemblyLoader.LoadTypeByFullClassName(item.Value);
                                    if (kscriptType == null) continue;
                                    var kscriptTypes = GetTypes(kscriptType);
                                    if (kscriptTypes == null || kscriptTypes.Count == 0) continue;

                                    kscriptConfigTypes[item.NameSpace] = kscriptTypes.ToArray();
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                }
                return kscriptConfigTypes;
            }
        }
        /// <summary>
        /// data 
        /// key:koobootest.service
        /// value:typeof(koobootest.service.product)
        /// </summary>
        public static Dictionary<string, Type> NamespaceTypeDic { get; set; } = new Dictionary<string, Type>();

        public static void Clear()
        {
            _list = null;
            kscriptSetting = null;
            kscriptContextType = null;
        }

        private static Dictionary<string, Type> GetKscriptSetting(string kscriptSetting)
        {
            var kscriptSettings = new Dictionary<string, Type>();

            var settingType = Kooboo.Lib.Reflection.AssemblyLoader.LoadTypeByFullClassName(kscriptSetting);
            if (settingType != null)
            {
                var types = GetTypes(settingType);

                foreach (var type in types)
                {
                    var name = type.Name;
                    var nameProp = type.GetProperty("Name");

                    if (nameProp != null)
                    {
                        var instance = Activator.CreateInstance(type);
                        if (instance != null)
                        {
                            var instanceName = nameProp.GetValue(instance) as string;
                            if (!string.IsNullOrEmpty(instanceName))
                                name = instanceName;
                        }
                    }
                    kscriptSettings[name] = type;
                }
            }

            return kscriptSettings;
        }

        private static List<Type> GetTypes(Type type)
        {
            var types = new List<Type> { type };
            if (type.IsInterface)
            {
                types = Kooboo.Lib.Reflection.AssemblyLoader.LoadTypeByInterface(type);
            }
            else if (type.IsClass)
            {
                types = Kooboo.Lib.Reflection.AssemblyLoader.LoadTypeByBaseClass(type);
            }

            return types;
        }
        private static string GetTopLevelNamespace(string ns)
        {
            if (string.IsNullOrEmpty(ns)) return string.Empty;
            var parts = ns.Split('.');
            var topLevelNamespace = parts[0];

            return topLevelNamespace;
        }

        private static void LoadExtensionDll(string extensionDlls)
        {
            if (!string.IsNullOrEmpty(extensionDlls))
                ExtensionAssemblyLoader.Instance.LoadSpecificDlls(extensionDlls);
        }

    }

    /// <summary>
    /// use this type to get kscript object by kscript namepace
    /// </summary>
    public class KScriptConfigType
    {
        private string kscriptname;
        private object kscriptContext;
        /// <summary>
        /// kscript name and kscript object dictionary
        /// </summary>
        public Dictionary<string, object> Types { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public KScriptConfigType() { }
        public KScriptConfigType(string name, RenderContext context)
        {
            SetKscriptnameNContext(name, context, kscriptContext);
        }

        /// <summary>
        /// pass kscriptContext and don't create context every time
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="kscriptContext"></param>
        KScriptConfigType(string name, RenderContext context, object kscriptContext)
        {
            SetKscriptnameNContext(name, context, kscriptContext);
        }

        public object this[string key]
        {
            get
            {
                if (Types.ContainsKey(key))
                    return Types[key];

                return null;
            }
        }

        public void SetKscriptnameNContext(string kscriptname, RenderContext context, object kscriptContext = null)
        {
            this.kscriptname = kscriptname;
            this.kscriptContext = kscriptContext;
            CreateInstanceByNamespace(kscriptname, context);
        }
        private void CreateInstanceByNamespace(string kscriptname, RenderContext context)
        {
            var keys = KscriptConfigContainer.NamespaceTypeDic.Keys;
            var matchNS = keys.ToList().FindAll(k => k.StartsWith(kscriptname + ".", StringComparison.OrdinalIgnoreCase));

            foreach (var ns in matchNS)
            {
                CreateInstanceByNamespace(kscriptname, ns, context);
            }
        }
        private void CreateInstanceByNamespace(string startNS, string ns, RenderContext rendercontext)
        {
            var endPrefix = ns.Remove(0, startNS.Length + 1);

            var parts = endPrefix.Split('.').ToList();
            var isClassName = parts.Count == 1;
            if (isClassName)
            {
                var type = KscriptConfigContainer.NamespaceTypeDic[ns];
                var instance = Activator.CreateInstance(type);

                if (KscriptConfigContainer.KscriptContextType != null)
                {
                    //set kscript context
                    var properties = type.GetProperties().ToList();
                    var datacontextProperty = properties.Find(p => p.PropertyType == KscriptConfigContainer.KscriptContextType || p.PropertyType.FullName == KscriptConfigContainer.KscriptContextType.FullName);
                    if (datacontextProperty != null)
                    {
                        datacontextProperty.SetValue(instance, GetOrCreateContext(rendercontext, datacontextProperty.PropertyType));
                    }
                }

                Types[parts[0]] = instance;
            }
            else
            {
                startNS = string.Format("{0}.{1}", startNS, parts[0]);
                var kScriptConfigType = new KScriptConfigType(startNS, rendercontext, kscriptContext);
                Types[parts[0]] = kScriptConfigType;
            }
        }

        //private object GetOrCreateContext(RenderContext context)
        //{
        //    var kscriptContextType = KscriptConfigContainer.KscriptContextType;
        //    if (kscriptContextType == null || context == null)
        //        return null;
        //    if (kscriptContext != null)
        //        return kscriptContext;

        //    var instance = context.ToKscriptContext(kscriptContextType);

        //    return instance;
        //}

        private object GetOrCreateContext(RenderContext renderContext, Type kscriptContextType)
        {
            if (kscriptContextType == null || renderContext == null)
                return null;
            if (kscriptContext != null)
                return kscriptContext;

            var instance = renderContext.CopyTo(kscriptContextType);
            this.kscriptContext = instance;
            return instance;
        }
    }

}

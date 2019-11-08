using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Kooboo.Data;
using System.Xml.Linq;
using System.Reflection;

namespace Kooboo.Sites.Scripting.KscriptConfig
{
    public class ExtensionKscriptConfigContainer
    {
        internal static ExtensionKscriptConfigReader kscriptConfigReader = new ExtensionKscriptConfigReader();
        private static object _lockObj = new object();

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
                            _list = new Dictionary<string, Type>();
                            var config = kscriptConfigReader.ReadConfig();
                            //walkaround cz
                            //dll in base directory,need be loaded
                            if (!string.IsNullOrEmpty(config.ExtensionDlls))
                            {
                                Lib.Reflection.AssemblyLoader.LoadExtensionDll(config.ExtensionDlls);
                            }
                            foreach (var item in config.Kscripts)
                            {
                                try
                                {
                                    var kscriptType = Lib.Reflection.AssemblyLoader.LoadTypeByFullClassName(item.Value);
                                    if (kscriptType == null) continue;

                                    if (kscriptType.IsInterface)
                                    {
                                        var alltypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(kscriptType);
                                        foreach (var type in alltypes)
                                        {
                                            _list[type.Name] = type;
                                        }
                                    }
                                    else
                                    {
                                        _list[item.Key] = kscriptType;
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                }
                return _list;
            }
        }
    }

 


}

using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Module
{ 
    public static class ModuleApiContainer
    {
        private static object _locker = new object();

        private static Dictionary<string, Type> _list;
        public static Dictionary<string, Type> List
        {
            get
            {
                if (_list == null)
                {
                    lock (_locker)
                    {
                        if (_list == null)
                        {
                            _list = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                             
                            var alldefinedTypes = AssemblyLoader.LoadTypeByInterface(typeof(ISiteModuleApi));
                            foreach (var item in alldefinedTypes)
                            { 
                             
                             // Activator.CreateInstance(item, )
                              // AddApi(_list, instance);
                            } 
                        }
                    }
                }
                return _list;
            }
        }
  
     

        // this seems like only for unit test now. 
        public static void AddApi(Type apitype)
        {
            //lock (_locker)
            //{
            //    var instance = Activator.CreateInstance(apitype) as IApi;
            //    if (instance != null)
            //    {
            //        var currentlist = List;
            //        AddApi(currentlist, instance);
            //    }
            //}
        }

        public static Kooboo.Api.ApiMethod GetApi(string ModelName)
        { 
            return null;
        }

    }


}

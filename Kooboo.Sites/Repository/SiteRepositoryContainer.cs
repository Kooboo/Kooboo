using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Repository
{
  
    public static class SiteRepositoryContainer
    {
        private static object _locker = new object();

        private static Dictionary<string, Type> _repos;
        public static Dictionary<string, Type> Repos
        {
            get
            {
                if (_repos == null)
                {
                    lock (_locker)
                    {
                        if (_repos == null)
                        {
                            _repos = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase); 

                            var allSiteObjectTypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IRepository));

                            foreach (var item in allSiteObjectTypes)
                            {
                                if (IsSubclassSiteRepository(item))
                                { 
                                    var genetype = Lib.Reflection.TypeHelper.GetGenericType(item); 
                                   
                                    if (genetype !=null)
                                    {
                                        var name = genetype.Name;
                                        _repos[name] = item;
                                    } 
                                }                                
                            }
                        }
                    }
                }
                return _repos;
            }
        }
          
        public static Type GetRepoTypeInfo(string ModelName)
        {
            if (ModelName !=null && Repos.ContainsKey(ModelName))
            {
                return Repos[ModelName]; 
            }
            return null; 
        }

        public static Type GetRepoTypeInfo(Type SiteModel)
        {
            return GetRepoTypeInfo(SiteModel.Name); 
        } 

        public static  bool IsSubclassSiteRepository(Type toCheck)
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
    }


}

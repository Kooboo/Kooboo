using System;
using System.Collections.Generic;

namespace Kooboo.Api
{
    public class ApiProvider : IApiProvider
    {
        public ApiProvider()
        {
            List = new Dictionary<string, IApi>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IApi)))
            {
                Set(item);
            }

        }


        public Dictionary<string, IApi> List
        {
            get; set;
        }

        public string ApiPrefix { get; set; } = "/_api";

        public void Set(Type apitype)
        {
            var instance = Activator.CreateInstance(apitype) as IApi;
            if (instance != null)
            {   
                List[instance.ModelName] = instance;
            }    
        }

        public IApi Get(string ModelName)
        {
            if (string.IsNullOrEmpty(ModelName))
            {
                return null;
            }
            if (List.ContainsKey(ModelName))
            {
                return List[ModelName];
            }

            return null;
        }
    }
}

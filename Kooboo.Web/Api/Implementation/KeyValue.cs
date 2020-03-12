//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Scripting.Global;
using KScript;
using System.Collections.Generic; 

namespace Kooboo.Web.Api.Implementation
{ 
    public class KeyValueApi : IApi
    {
        public string ModelName
        {
            get { return "KeyValue"; }
        }

        public bool RequireSite
        {
            get { return true; }
        }

        public bool RequireUser
        {
            get { return true; }
        }

        public Dictionary<string, string>  List(ApiCall call)
        {
            kKeyValue store = new kKeyValue(call.Context);

            Dictionary<string, string> allvalues = new Dictionary<string, string>();

            foreach (var item in store)
            {
                allvalues.Add(item.Key, item.Value?.ToString()); 
            }
            return allvalues;  
        }
         
        public string Get(string key, ApiCall call)
        {
            kKeyValue store = new kKeyValue(call.Context); 
            return store.get(key)?.ToString(); 
        }

        public void Update(string key, string value, ApiCall call)
        {
            kKeyValue store = new kKeyValue(call.Context);
            store.set(key, value); 
        }

        public void Deletes(List<string> ids, ApiCall call)
        {
            kKeyValue store = new kKeyValue(call.Context);

            foreach (var item in ids)
            {
                store.Remove(item); 
            }
        }

        public bool IsUniqueName(string name, ApiCall call)
        {
            kKeyValue store = new kKeyValue(call.Context);
            var obj = store.get(name);

            return obj == null; 

        }

    }
}

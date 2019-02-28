//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Render.Components
{
    public static class Container
    {
        static Container()
        {   
            List = new Dictionary<string, IComponent>(StringComparer.OrdinalIgnoreCase);

            var typelist = AssemblyLoader.LoadTypeByInterface(typeof(IComponent));

            foreach (var item in typelist)
            {
                var instance = Activator.CreateInstance(item) as IComponent;
                if (!List.ContainsKey(instance.TagName))
                {
                    List.Add(instance.TagName, instance);
                }
            }
        }
        /// <summary>
        /// Return the list of components. 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, IComponent> List
        {
            get; set;
        }
            
        public static IComponent Get(string ComponentTagName)
        {
            if (List.ContainsKey(ComponentTagName))
            {
                return List[ComponentTagName];
            }
            return null;
        }


        public static IComponent GetByConstType(byte constObjectType)
        {
            return List.Values.ToList().Find(o => o.StoreConstType == constObjectType);

        }

    }
}

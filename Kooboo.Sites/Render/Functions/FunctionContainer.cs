//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Render.Functions
{
    public static class FunctionContainer
    {
        static FunctionContainer()
        {
            FunctionList = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            var types = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IFunction));

            foreach (var item in types)
            {
                var onefunction = (IFunction)Activator.CreateInstance(item);

                string name = onefunction.Name;
                FunctionList.Add(name, item);
            }
        }

        private static Dictionary<string, Type> FunctionList
        {
            get; set;
        }

        public static IFunction GetFunction(string name)
        {
            if (FunctionList.ContainsKey(name))
            {
                var type = FunctionList[name];
                return (IFunction)Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
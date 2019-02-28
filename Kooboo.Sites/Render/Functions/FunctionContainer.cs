//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Functions
{
   public static class FunctionContainer
    { 
        static FunctionContainer()
        {
            functionList = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            var types = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IFunction));

            foreach (var item in types)
            {
                var onefunction = (IFunction)Activator.CreateInstance(item);

                string name = onefunction.Name;
                functionList.Add(name, item);
            }
        }

    

        private static Dictionary<string, Type> functionList
        {
            get;set;
          
        }

        public static IFunction GetFunction(string name)
        { 
            if (functionList.ContainsKey(name))
            {
                var type = functionList[name];
                return (IFunction)Activator.CreateInstance(type);
            }

            return null;
        }
         
    }
}


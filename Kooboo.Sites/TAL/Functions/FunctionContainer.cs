using Kooboo.TAL.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.TAL.Binding
{
   public class FunctionContainer
    {
        private static Dictionary<string, IFunction> _listoffunction;

        private static Dictionary<string, IFunction> getFunctions()
        {
            if (_listoffunction == null)
            {
                _listoffunction = new Dictionary<string, IFunction>();
                foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in item.GetTypes())
                    {
                        if (!type.IsAbstract && !type.IsInterface && !type.IsGenericType && typeof(IFunction).IsAssignableFrom(type))
                        {
                            var onefunction = (IFunction)Activator.CreateInstance(type);

                            _listoffunction.Add(onefunction.Name.ToLower(),onefunction);
                        }
                    }
                }
            }

            return _listoffunction;
        }

        public static IFunction getFunction(string name)
        {
            name = name.ToLower();

            if (getFunctions().ContainsKey(name))
            {
                return getFunctions()[name];
            }

            return null;
        } 

    }
}

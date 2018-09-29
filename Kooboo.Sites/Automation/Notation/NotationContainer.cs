//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib;
using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Automation.Notation
{
 public static   class NotationContainer
    {      

        private static Dictionary<string, INotation> _List;

        public static Dictionary<string, INotation> GetNotaions()
        {
            if (_List == null)
            {
                _List = new Dictionary<string, INotation>(); 

                foreach (var type in AssemblyLoader.LoadTypeByInterface(typeof(INotation)))
                {
                    var instance = (INotation)Activator.CreateInstance(type);

                    _List.Add(instance.Name, instance); 
                }
            }

            return _List;
        }

        public static INotation getNataitonByName(string Name)
        {
            if (GetNotaions().ContainsKey(Name))
            {
                return GetNotaions()[Name]; 
            }
            return null;
        }

        public static List<string> GetNotationNames()
        {
            List<string> names = new List<string>();

            foreach (var item in GetNotaions())
            {
                names.Add(item.Key); 
            }
            return names; 
        }

    }
}

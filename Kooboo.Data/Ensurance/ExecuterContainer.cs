using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Ensurance
{
      
    public class ExecuterContainer
    {
        private static object _locker = new object();

        private static Dictionary<string, IExecutor> _list;

        public static Dictionary<string, IExecutor> List
        {
            get
            {
                if (_list == null)
                {
                    lock (_locker)
                    {
                        if (_list == null)
                        {
                            _list = new Dictionary<string, IExecutor>();

                            var alltypes = AssemblyLoader.LoadTypeByGenericInterface(typeof(IExecutor<>));
                            foreach (var item in alltypes)
                            {
                                var iteminstance = Activator.CreateInstance(item) as IExecutor;
                                if (iteminstance != null)
                                {
                                    string FullTypeName = TypeHelper.GetGenericType(item).FullName;

                                    _list[FullTypeName] = iteminstance;
                                }
                            }
                        }
                    }
                }

                return _list;
            }
        }

        public static IExecutor GetExecutor(string FullTypeName)
        {
            if (List.ContainsKey(FullTypeName))
            {
                return List[FullTypeName];
            }
            return null;
        }
    }


}

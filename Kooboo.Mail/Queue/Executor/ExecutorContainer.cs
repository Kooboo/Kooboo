//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Lib.Reflection;

namespace Kooboo.Mail.Queue.Executor
{
    public class TaskExecutorContainer
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
                            var list = new Dictionary<string, IExecutor>();

                            var alltypes = AssemblyLoader.LoadTypeByGenericInterface(typeof(IExecutor<>));
                            foreach (var item in alltypes)
                            {
                                var iteminstance = Activator.CreateInstance(item) as IExecutor;
                                if (iteminstance != null)
                                {
                                    string FullTypeName = TypeHelper.GetGenericType(item).FullName;

                                    list[FullTypeName] = iteminstance;
                                }
                            }
                            _list = list;
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

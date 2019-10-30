//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.TaskQueue.TaskExecutor
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
                            _list = new Dictionary<string, IExecutor>();

                            var alltypes = AssemblyLoader.LoadTypeByGenericInterface(typeof(ITaskExecutor<>));
                            foreach (var item in alltypes)
                            {
                                if (Activator.CreateInstance(item) is IExecutor iteminstance)
                                {
                                    string fullTypeName = TypeHelper.GetGenericType(item).FullName;

                                    _list[fullTypeName] = iteminstance;
                                }
                            }
                        }
                    }
                }

                return _list;
            }
        }

        public static IExecutor GetExecutor(string fullTypeName)
        {
            if (List.ContainsKey(fullTypeName))
            {
                return List[fullTypeName];
            }
            return null;
        }
    }
}
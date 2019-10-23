//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Render.ServerSide
{
    public static class TaskContainer
    {
        static TaskContainer()
        {
            _list = new Dictionary<string, Type>();

            List<Type> types = new List<Type>
            {
                typeof(LoadJs),
                typeof(SetHtml),
                typeof(LoadJsFolder),
                typeof(LoadFolder),
                typeof(SetMethods)
            };

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type) as IServerTask;

                var name = instance.name;
                name = "k." + name;

                _list.Add(name, type);
            }
        }

        private static object _locker = new object();

        private static Dictionary<string, Type> _list;

        public static Dictionary<string, Type> list
        {
            get
            {
                if (_list == null)
                {
                    lock (_locker)
                    {
                        if (_list == null)
                        {
                            _list = new Dictionary<string, Type>();

                            List<Type> types = new List<Type>
                            {
                                typeof(LoadJs),
                                typeof(SetHtml),
                                typeof(LoadJsFolder),
                                typeof(LoadFolder),
                                typeof(SetMethods)
                            };

                            foreach (var type in types)
                            {
                                var instance = Activator.CreateInstance(type) as IServerTask;

                                var name = instance?.name;
                                name = "k." + name;

                                _list.Add(name, type);
                            }
                        }
                    }
                }
                return _list;
            }
        }
    }
}
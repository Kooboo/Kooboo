//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic; 
using System.Reflection;
  
namespace Kooboo.Data.Events
{
    public static class EventBus
    {
        private static Guid GetTypeHash(Type type)
        {
            string typename = type.FullName;
            return Lib.Security.Hash.ComputeGuidIgnoreCase(typename);
        }

        private static object _locker = new object();

        public static void Raise(IEvent theEvent, RenderContext context =null)
        {
            var type = theEvent.GetType();

            List<HandlerInstance> handlers = new List<HandlerInstance>(); 

            // execute handler. 
            var directhandlers = List.FindAll(o => o.EventType == type);
            if (directhandlers != null)
            {
                handlers.AddRange(directhandlers); 
            }

            // get base class 
            var basetype = type.BaseType;
            while (basetype != null && basetype != typeof(object))
            {
                var basehandlers = List.FindAll(o => o.EventType == basetype);
                if (basehandlers != null)
                {
                    handlers.AddRange(basehandlers);
                }
                basetype = basetype.BaseType; 
            }

            var allinterfaces = type.GetInterfaces();
            if (allinterfaces != null)
            {
                foreach (var item in allinterfaces)
                {
                    var interfacehandlers = List.FindAll(o => o.EventType == item);
                    if (interfacehandlers != null)
                    {
                        handlers.AddRange(interfacehandlers);
                    }
                }
            }

            foreach (var item in handlers)
            {
                List<object> paras = new List<object>();
                paras.Add(theEvent);
                paras.Add(context); 
                item.Handle.Invoke(item.ClassInstance, paras.ToArray());
            }
             
        }

        private static List<HandlerInstance> _List;
        public static List<HandlerInstance> List
        {
            get
            {
                if (_List == null)
                {
                    lock (_locker)
                    {
                        if (_List == null)
                        {
                            _List = new List<HandlerInstance>();

                            var alltypes = AssemblyLoader.LoadTypeByGenericInterface(typeof(IHandler<>));

                            foreach (var item in alltypes)
                            {
                                HandlerInstance instance = new HandlerInstance();

                                var iteminstance = Activator.CreateInstance(item);
                                instance.ClassInstance = iteminstance;

                                instance.EventType = TypeHelper.GetGenericType(item);

                                instance.Handle = item.GetMethod("Handle");

                                _List.Add(instance);
                            }
                        }
                    }
                }

                return _List;
            }
        }
         
    }
     
    public class HandlerInstance
    {
        public object ClassInstance { get; set; }

        public Type EventType { get; set; }

        public MethodInfo Handle { get; set; }

    }

}
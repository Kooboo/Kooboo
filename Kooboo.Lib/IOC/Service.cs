using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Kooboo.Lib.IOC
{
    public static class Service
    {
        static Service()
        {
            InterfaceTypes = new Dictionary<string, List<Type>>();
            SingleTons = new Dictionary<string, object>();
            Transients = new Dictionary<string, Type>();
            Instances = new Dictionary<string, List<object>>();
            ImplementationType = new Dictionary<Type, Type>();
            PriorityType = new Dictionary<string, Type>();
            _lock = new object();
        }

        private static object _lock { get; set; }

        public static Dictionary<string, List<Type>> InterfaceTypes { get; set; }

        public static Dictionary<string, List<Object>> Instances { get; set; }

        public static Dictionary<string, Type> PriorityType { get; set; }

        public static Dictionary<Type, Type> ImplementationType { get; set; }


        public static Dictionary<string, Object> SingleTons { get; set; }

        public static Dictionary<string, Type> Transients { get; set; }

        // get the Singleton instance. 
        public static T GetSingleTon<T>(bool SearchImplemention = false)
        {
            var result = GetSingleTon(typeof(T), SearchImplemention);
            if (result == null)
            {
                return default(T);
            }
            else
            {
                return (T)result;
            }
        }

        // get the Singleton instance. 
        public static T GetSingleTon<T>(Type DefaultImplementation)
        {
            var result = GetSingleTon(typeof(T), false);
            if (result == null)
            {
                var newinstance = (T)Activator.CreateInstance(DefaultImplementation);
                AddSingleton<T>(newinstance);
                return newinstance;
            }
            else
            {
                return (T)result;
            }
        }

        public static Object GetSingleTon(Type objType, bool SearchImplementation)
        {
            string name = objType.Name;

            if (!SingleTons.ContainsKey(name))
            {
                if (SearchImplementation)
                {
                    lock (_lock)
                    {
                        if (!SingleTons.ContainsKey(name))
                        {

                            var types = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(objType);

                            if (Lib.Reflection.TypeHelper.HasInterface(objType, typeof(IPriority)))
                            {
                                List<IPriority> list = new List<IPriority>();
                                foreach (var item in types)
                                {
                                    var obj = Activator.CreateInstance(item) as IPriority;
                                    if (obj != null)
                                    {
                                        list.Add(obj);
                                    }
                                }

                                if (list.Any())
                                {
                                    SingleTons[name] = list.OrderByDescending(o => o.Priority).First();
                                }
                                else
                                {
                                    SingleTons[name] = null;
                                }
                            }
                            else
                            {
                                if (types != null && types.Any())
                                {
                                    var type = types[0];
                                    var obj = Activator.CreateInstance(type);
                                    SingleTons[name] = obj;
                                }
                                else
                                {
                                    SingleTons[name] = null;
                                }

                            }


                        }
                    }
                }
                else
                {
                    return null;
                }
            }

            return SingleTons[name];
        }

        public static void AddSingleton<T>(T instance)
        {
            var name = typeof(T).Name;
            SingleTons[name] = instance;
        }


        public static void AddInstance<T>(T instance)
        {
            var list = GetInstances<T>();
        }

        public static List<Type> GetImplementationTypes<T>()
        {
            return GetImplementationTypes(typeof(T));
        }


        //create intance based on priority.  
        public static T CreateInstanceByPriority<T>() where T : IPriority
        {
            var type = typeof(T);
            string name = type.Name;

            if (!PriorityType.ContainsKey(name))
            {
                lock (_lock)
                {
                    if (!PriorityType.ContainsKey(name))
                    {
                        Type SelectedType = null;

                        var types = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(type);

                        long current = long.MinValue;

                        if (types != null && types.Any())
                        {
                            foreach (var item in types)
                            {
                                var prio = GetTypePriority(item);

                                if (SelectedType == null)
                                {
                                    SelectedType = item;
                                    current = prio;
                                }
                                else
                                {
                                    if (prio > current)
                                    {
                                        prio = current;
                                        SelectedType = item;
                                    }

                                }
                            }
                        }

                        PriorityType[name] = SelectedType;


                    }
                }
            }

            var righttype = PriorityType[name];
            if (righttype != null)
            {
                var instance = Activator.CreateInstance(righttype);
                return (T)instance;
            }
            return default(T);
        }

        public static long GetTypePriority(Type objType)
        {
            var method = objType.GetProperty("Priority").GetGetMethod();
            var dynamicMethod = new DynamicMethod("meide", typeof(long),
                                                  Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Call, method);
            generator.Emit(OpCodes.Ret);
            var silly = (Func<long>)dynamicMethod.CreateDelegate(
                           typeof(Func<long>));
            return silly();
        }

        public static List<Type> GetImplementationTypes(Type InterfaceType)
        {
            string name = InterfaceType.FullName;
            if (!InterfaceTypes.ContainsKey(name))
            {
                lock (_lock)
                {
                    if (!InterfaceTypes.ContainsKey(name))
                    {
                        var types = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(InterfaceType);
                        InterfaceTypes[name] = types;
                    }
                }
            }
            return InterfaceTypes[name];
        }

        //Get instance that implement the TInterface. 
        public static List<T> GetInstances<T>()
        {
            var instances = GetInstances(typeof(T));
            List<T> result = new List<T>();
            foreach (var item in instances)
            {
                var Tinstance = (T)item;
                if (Tinstance != null)
                {
                    result.Add(Tinstance);
                }
            }
            return result;
        }

        public static List<object> GetInstances(Type InterfaceType)
        {
            string name = InterfaceType.FullName;

            if (!Instances.ContainsKey(name))
            {
                lock (_lock)
                {
                    if (!Instances.ContainsKey(name))
                    {
                        List<object> Result = new List<object>();
                        var types = GetImplementationTypes(InterfaceType);
                        foreach (var item in types)
                        {
                            var instance = Activator.CreateInstance(item);
                            if (instance != null)
                            {
                                Result.Add(instance);
                            }
                        }

                        Instances[name] = Result;
                    }
                }
            }

            return Instances[name];
        }
    }
}

//Transient – Created every time they are requested
//Scoped – Created once per scope. Most of the time, scope refers to a web request. But this can also be used for any unit of work, such as the execution of an Azure Function.
//Singleton – Created only for the first request.If a particular instance is specified at registration time, this instance will be provided to all consumers of the registration type.

//services.AddScoped<IMyDependency, MyDependency>();
//services.AddTransient<IOperationTransient, Operation>();
// services.AddScoped<IOperationScoped, Operation>();
// services.AddSingleton<IOperationSingleton, Operation>();
// services.AddSingleton<IOperationSingletonInstance>(new Operation(Guid.Empty));

// // OperationService depends on each of the other Operation types.
// services.AddTransient<OperationService, OperationService>();

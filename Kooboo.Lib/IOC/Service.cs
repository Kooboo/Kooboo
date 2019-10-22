using System;
using System.Collections.Generic;
using System.Linq;

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
            _lock = new object();
        }

        private static object _lock { get; set; }

        public static Dictionary<string, List<Type>> InterfaceTypes { get; set; }

        public static Dictionary<string, List<Object>> Instances { get; set; }

        public static Dictionary<Type, Type> ImplementationType { get; set; }

        public static Dictionary<string, Object> SingleTons { get; set; }

        public static Dictionary<string, Type> Transients { get; set; }

        // get the Singleton instance.
        public static T GetSingleTon<T>(bool searchImplemention = false)
        {
            var result = GetSingleTon(typeof(T), searchImplemention);
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
        public static T GetSingleTon<T>(Type defaultImplementation)
        {
            var result = GetSingleTon(typeof(T), false);
            if (result == null)
            {
                var newinstance = (T)Activator.CreateInstance(defaultImplementation);
                AddSingleton<T>(newinstance);
                return newinstance;
            }
            else
            {
                return (T)result;
            }
        }

        public static Object GetSingleTon(Type objType, bool searchImplementation)
        {
            string name = objType.Name;

            if (!SingleTons.ContainsKey(name))
            {
                if (searchImplementation)
                {
                    lock (_lock)
                    {
                        if (!SingleTons.ContainsKey(name))
                        {
                            var types = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(objType);

                            //TODO: add rule to get the right intance.
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

        public static List<Type> GetImplementationTypes<T>()
        {
            return GetImplementationTypes(typeof(T));
        }

        public static List<Type> GetImplementationTypes(Type interfaceType)
        {
            string name = interfaceType.Name;
            if (!InterfaceTypes.ContainsKey(name))
            {
                lock (_lock)
                {
                    if (!InterfaceTypes.ContainsKey(name))
                    {
                        var types = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(interfaceType);
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
                var tinstance = (T)item;
                if (tinstance != null)
                {
                    result.Add(tinstance);
                }
            }
            return result;
        }

        public static List<object> GetInstances(Type interfaceType)
        {
            string name = interfaceType.Name;

            if (!Instances.ContainsKey(name))
            {
                lock (_lock)
                {
                    if (!Instances.ContainsKey(name))
                    {
                        List<object> result = new List<object>();
                        var types = GetImplementationTypes(interfaceType);
                        foreach (var item in types)
                        {
                            var instance = Activator.CreateInstance(item);
                            if (instance != null)
                            {
                                result.Add(instance);
                            }
                        }

                        Instances[name] = result;
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
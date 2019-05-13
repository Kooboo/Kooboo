using System;
using System.Collections.Generic;
using System.Text;
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
            _lock = new object();
        }

        private static object _lock { get; set; }

        public static Dictionary<string, List<Type>> InterfaceTypes { get; set; }

        public static Dictionary<string, List<Object>> Instances { get; set; }


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

        public static List<Type> GetImplementationTypes(Type InterfaceType)
        {
            string name = InterfaceType.Name;
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
                if (Tinstance !=null)
                {
                    result.Add(Tinstance); 
                }
            }
            return result; 
        }

        public static List<object> GetInstances(Type InterfaceType)
        {
            string name = InterfaceType.Name;

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

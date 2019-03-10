using Kooboo.Data.Interface;
using System;
using System.Collections.Generic; 

namespace Kooboo
{
    // TOOD: improve performance.  
    public static class ConstTypeContainer
    {
        private static object _locker = new object();

        private static Dictionary<byte, Type> _ByteTypes;
        public static Dictionary<byte, Type> ByteTypes
        {
            get
            {
                if (_ByteTypes == null)
                {
                    lock (_locker)
                    {
                        if (_ByteTypes == null)
                        {
                            _ByteTypes = new Dictionary<byte, Type>();

                            var allSiteObjectTypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(ISiteObject));

                            foreach (var item in allSiteObjectTypes)
                            {
                                // TODO: find a way to improve performance.
                                var instance = Activator.CreateInstance(item) as ISiteObject; 
                                if (instance !=null && instance.ConstType >0)
                                {
                                    if (_ByteTypes.ContainsKey(instance.ConstType))
                                    {
                                        var current = _ByteTypes[instance.ConstType]; 
                                        if (Lib.Reflection.TypeHelper.HasBaseType(current, item))
                                        {
                                            _ByteTypes[instance.ConstType] = item; 
                                        }
                                    }
                                    else
                                    {
                                        _ByteTypes.Add(instance.ConstType, item);
                                    }
                                   
                                }  
                            } 
                        }
                    }
                }
                return _ByteTypes;
            }
        }

         
        private static Dictionary<string, Type> _nametypes;
        private static object _namelocker = new object(); 
        public static Dictionary<string, Type> nameTypes
        {
            get
            {
                if (_nametypes == null)
                {
                    lock(_namelocker)
                    {
                        _nametypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                        foreach (var item in ByteTypes)
                        {
                            var name = item.Value.Name;
                            _nametypes.Add(name, item.Value); 
                        }
                    }
                }
                return _nametypes;
            }
        }
         
        private static Dictionary<string, byte> _namebytes;
        private static object _namebytelocker = new object(); 

        public static Dictionary<string, byte> NameBytes
        {
            get
            {
                if (_namebytes== null)
                {
                    lock(_namebytelocker)
                    {
                        if (_namebytes == null)
                        {
                            _namebytes = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
                            foreach (var item in ByteTypes)
                            {
                                _namebytes.Add(item.Value.Name, item.Key); 
                            }
                        }
                    } 
                } 
                return _namebytes; 
            }
        }
         
        public static Type GetModelType(byte constType)
        {
            if (ByteTypes.ContainsKey(constType))
            {
                return ByteTypes[constType];
            }
            return null;
        }

        public static Type GetModelType(string modelname)
        {
            if (nameTypes.ContainsKey(modelname))
            {
                return nameTypes[modelname];
            }
            return null;
        }
         
        public static byte GetConstType(Type modeltype)
        {
            var name = modeltype.Name;
            if (NameBytes.ContainsKey(name))
            {
                return NameBytes[name];
            }
            return 0;
        }

        public static byte GetConstType(string ModelName)
        { 
            if (NameBytes.ContainsKey(ModelName))
            {
                return NameBytes[ModelName];
            }
            return 0;
        }


        public static string GetName(byte constType)
        {

            if (ByteTypes.ContainsKey(constType))
            {
                var type = ByteTypes[constType]; 
                if (type !=null)
                {
                    return type.Name; 
                }
            }

            return string.Empty;  
        }

    } 
}

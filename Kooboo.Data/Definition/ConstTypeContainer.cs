using Kooboo.Data.Interface;
using System;
using System.Collections.Generic; 

namespace Kooboo
{
    // TOOD: improve performance.  
    public static class ConstTypeContainer
    {
        private static object _locker = new object();

        private static Dictionary<byte, Type> _byteTypes;
        public static Dictionary<byte, Type> ByteTypes
        {
            get
            {
                if (_byteTypes == null)
                {
                    lock (_locker)
                    {
                        if (_byteTypes == null)
                        {
                            _byteTypes = new Dictionary<byte, Type>();

                            var allSiteObjectTypes = Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(ISiteObject));

                            foreach (var item in allSiteObjectTypes)
                            {
                                // TODO: find a way to improve performance.
                                if (Activator.CreateInstance(item) is ISiteObject instance && instance.ConstType >0)
                                {
                                    if (_byteTypes.ContainsKey(instance.ConstType))
                                    {
                                        var current = _byteTypes[instance.ConstType]; 
                                        if (Lib.Reflection.TypeHelper.HasBaseType(current, item))
                                        {
                                            _byteTypes[instance.ConstType] = item; 
                                        }
                                    }
                                    else
                                    {
                                        _byteTypes.Add(instance.ConstType, item);
                                    }
                                   
                                }  
                            } 
                        }
                    }
                }
                return _byteTypes;
            }
        }

         
        private static Dictionary<string, Type> _nametypes;
        private static object _namelocker = new object(); 
        public static Dictionary<string, Type> NameTypes
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
            return ByteTypes.ContainsKey(constType) ? ByteTypes[constType] : null;
        }

        public static Type GetModelType(string modelname)
        {
            return NameTypes.ContainsKey(modelname) ? NameTypes[modelname] : null;
        }
         
        public static byte GetConstType(Type modeltype)
        {
            var name = modeltype.Name;
            return NameBytes.ContainsKey(name) ? NameBytes[name] : (byte) 0;
        }

        public static byte GetConstType(string modelName)
        {
            return NameBytes.ContainsKey(modelName) ? NameBytes[modelName] : (byte) 0;
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

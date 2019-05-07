using System;
using System.Collections.Generic;
using System.Text;
using Jint.Native.Object;
using Jint.Native.Array;
using Jint.Runtime.Interop;
namespace Jint
{
    public static class ArrayInstanceExtension
    {
        public static ObjectInstance Construct(this ArrayConstructor arrayConstructor, byte[] bytes)
        {
            return new ObjectWrapper(arrayConstructor.Engine, bytes);
        }
    }

    public class JintEngineHelper
    {
        public static void AddTypeMapper()
        {
            if (!Engine.TypeMappers.ContainsKey(typeof(byte[])))
            {
                Engine.TypeMappers.Add(typeof(byte[]), (Engine en, object v) => en.Array.Construct((byte[])v));
            }
            
        }
    }


}

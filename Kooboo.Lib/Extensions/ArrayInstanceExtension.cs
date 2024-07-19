using Jint.Native.Array;
using Jint.Native.Object;
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
#warning todo
            //if (!Engine.TypeMappers.ContainsKey(typeof(byte[])))
            //{
            //    Engine.TypeMappers.Add(typeof(byte[]), (Engine en, object v) => en.Array.Construct((byte[])v));
            //}

        }
    }


}

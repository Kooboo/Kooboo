using System;
using System.Reflection.Emit;

namespace Kooboo.IndexedDB.WORM.MetaObject
{
    public static class MetaObjectHelper
    {

        public static TField GetPropertyValue<TField, TValue>(string PropertyName)
        {
            var type = typeof(TValue);

            var method = type.GetProperty(PropertyName).GetGetMethod();
            var dynamicMethod = new DynamicMethod("meide", typeof(TField),
                                                  Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Call, method);
            generator.Emit(OpCodes.Ret);
            var funcCall = (Func<TField>)dynamicMethod.CreateDelegate(
                           typeof(Func<TField>));
            return funcCall();
        }

    }
}

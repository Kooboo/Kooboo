using System;
using LibSass.Compiler.Options;
using LibSass.Types;
using static LibSass.Compiler.SassExterns;

namespace LibSass.Compiler.Context
{
    internal abstract partial class SassSafeContextHandle
    {
        private IntPtr GetCustomFunctionsHeadPtr(SassFunctionCollection customFunctions)
        {
            int length = customFunctions.Count;
            IntPtr cFunctions = sass_make_function_list(customFunctions.Count);

            for (int i = 0; i < length; ++i)
            {
                SassFunction customFunction = customFunctions[i];
                IntPtr pointer = customFunction.CustomFunctionDelegate.Method.MethodHandle.GetFunctionPointer();

                _functionsCallbackDictionary.Add(pointer, customFunction.CustomFunctionDelegate);

                var cb = sass_make_function(new SassSafeStringHandle(customFunction.Signature), SassFunctionCallback, pointer);
                sass_function_set_list_entry(cFunctions, i, cb);
            }

            return cFunctions;
        }

        private IntPtr SassFunctionCallback(IntPtr sassValues, IntPtr callback, IntPtr compiler)
        {
            ISassType[] convertedValues = TypeFactory.GetSassArguments(sassValues);

            IntPtr signaturePtr = sass_function_get_signature(callback);
            string signature = PtrToString(signaturePtr);

            IntPtr cookiePtr = sass_function_get_cookie(callback);
            CustomFunctionDelegate customFunctionCallback = _functionsCallbackDictionary[cookiePtr];

            ISassType returnedValue = customFunctionCallback(_sassOptions, signature, convertedValues);

            var ptr = TypeFactory.GetRawPointer(returnedValue, ValidityEvent);

            ValidityEvent.Invoke();

            return ptr;
        }
    }
}

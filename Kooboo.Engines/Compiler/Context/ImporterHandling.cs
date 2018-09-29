using System;
using LibSass.Compiler.Options;
using static LibSass.Compiler.SassExterns;

namespace LibSass.Compiler.Context
{
    internal abstract partial class SassSafeContextHandle
    {
        private IntPtr GetCustomImportersHeadPtr(CustomImportDelegate[] customImporters)
        {
            int length = customImporters.Length;
            IntPtr cImporters = sass_make_importer_list(customImporters.Length);

            for (int i = 0; i < length; ++i)
            {
                CustomImportDelegate customImporter = customImporters[i];
                IntPtr pointer = customImporter.Method.MethodHandle.GetFunctionPointer();

                _importersCallbackDictionary.Add(pointer, customImporter);

                var entry = sass_make_importer(SassImporterCallback, length - i - 1, pointer);
                sass_importer_set_list_entry(cImporters, i, entry);
            }

            return cImporters;
        }

        private IntPtr SassImporterCallback(IntPtr url, IntPtr callback, IntPtr compiler)
        {
            IntPtr cookiePtr = sass_importer_get_cookie(callback);
            CustomImportDelegate customImportCallback = _importersCallbackDictionary[cookiePtr];

            IntPtr parentImporterPtr = sass_compiler_get_last_import(compiler);
            IntPtr absolutePathPtr = sass_import_get_abs_path(parentImporterPtr);
            string parentImport = PtrToString(absolutePathPtr);
            string currrentImport = PtrToString(url);
            SassImport[] importsArray = customImportCallback(currrentImport, parentImport, _sassOptions);

            if (importsArray == null)
                return IntPtr.Zero;

            IntPtr cImportsList = sass_make_import_list(importsArray.Length);

            for (int i = 0; i < importsArray.Length; ++i)
            {
                IntPtr entry;
                if (string.IsNullOrEmpty(importsArray[i].Error))
                {
                    entry = sass_make_import_entry(new SassSafeStringHandle(importsArray[i].Path),
                                                   EncodeAsUtf8IntPtr(importsArray[i].Data),
                                                   EncodeAsUtf8IntPtr(importsArray[i].Map));
                }
                else
                {
                    entry = sass_make_import_entry(new SassSafeStringHandle(string.Empty), IntPtr.Zero, IntPtr.Zero);
                    sass_import_set_error(entry, EncodeAsUtf8IntPtr(importsArray[i].Error), -1, -1);
                }

                sass_import_set_list_entry(cImportsList, i, entry);
            }

            return cImportsList;
        }
    }
}

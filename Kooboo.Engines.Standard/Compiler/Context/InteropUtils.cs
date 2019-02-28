using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LibSass.Compiler.Context
{
    internal abstract partial class SassSafeContextHandle
    {
        internal static string[] PtrToStringArray(IntPtr stringArray)
        {
            if (stringArray == IntPtr.Zero)
                return new string[0];

            List<string> members = new List<string>();

            for (int count = 0; Marshal.ReadIntPtr(stringArray, count * IntPtr.Size) != IntPtr.Zero; ++count)
            {
                members.Add(PtrToString(Marshal.ReadIntPtr(stringArray, count * IntPtr.Size)));
            }

            return members.ToArray();
        }

        internal static string PtrToString(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return string.Empty;

            var data = new List<byte>();
            var offset = 0;

            while (true)
            {
                var ch = Marshal.ReadByte(handle, offset++);

                if (ch == 0)
                    break;

                data.Add(ch);
            }

            return Encoding.UTF8.GetString(data.ToArray());
        }

        internal static string EncodeAsUtf8String(string utf16String)
        {
            if (string.IsNullOrEmpty(utf16String))
            {
                return string.Empty;
            }

            // Get UTF-8 bytes from UTF-16 string
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(utf16String);

            // Return UTF-8 bytes as ANSI string
            return Encoding.Default.GetString(utf8Bytes);
        }

        internal static IntPtr EncodeAsUtf8IntPtr(string utf16String)
        {
            if (string.IsNullOrEmpty(utf16String))
            {
                return IntPtr.Zero;
            }

            return SassExterns.sass_copy_c_string(new SassSafeStringHandle(utf16String));
        }

    }
}

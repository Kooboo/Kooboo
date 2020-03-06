//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;

namespace Kooboo.Data.Interface
{
    public interface ISiteSetting
    {
        string Name { get; }
    }

    public class SettingFile
    {
        /// <param name="nameAndBase64Data">format is "fileName|base64String"</param>
        public SettingFile(string nameAndBase64Data)
        {
            if (nameAndBase64Data != null)
            {
                var idx = nameAndBase64Data.IndexOf("|", StringComparison.OrdinalIgnoreCase);
                idx = idx > 0 ? idx : 0;
                Name = nameAndBase64Data.Substring(0, idx);
                Base64 = nameAndBase64Data.Substring(idx + 1, nameAndBase64Data.Length - idx - 1);
            }
        }

        public string Name { get; set; }

        public string Base64 { get; set; }

        public byte[] Bytes => string.IsNullOrEmpty(Base64) ? null : Convert.FromBase64String(Base64);

        public Stream Stream => Bytes == null ? null : new MemoryStream(Bytes);
    }
}

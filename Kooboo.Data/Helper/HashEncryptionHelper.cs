//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Data.Helper
{
    public static class HashEncryptionHelper
    {
        // backward compatible.
        public static string GetKey(string keyname)
        {
            if (string.IsNullOrEmpty(keyname))
            {
                return null;
            }

            return keyname.Length >= RSAHelper.MinEncryptedLenth ? RSAHelper.Decrypt(keyname) : keyname;
        }
    }
}
//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if (keyname.Length >= RSAHelper.MinEncryptedLenth)
            {
                return RSAHelper.Decrypt(keyname);
            }
            return keyname;
        }     
   
    }
}

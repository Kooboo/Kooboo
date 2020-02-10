using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Service
{
    public static class PasswordService
    {
        private static IPasswordHash hasher { get; set; }
        static PasswordService()
        {
            hasher = Lib.IOC.Service.GetSingleTon<IPasswordHash>(true);
        }
        public static string Hash(string password)
        {
            return hasher.Hash(password);
        } 

        public static bool Verify(string password, string StoredHashValue)
        {
            return hasher.Verify(password, StoredHashValue);
        }

    }
}

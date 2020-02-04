using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Interface
{
   public interface IPasswordHash : Kooboo.Lib.IOC.ISingleTonPriority
    {
        string Hash(string password);

        bool Verify(string password, string StoredHashValue); 
    } 
}

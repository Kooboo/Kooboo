using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Interface
{
    public interface ILoginBlock
    {
        bool IsIpBlocked(string IP); 

        void AddIpFailure(string IP);

        void AddUserNameFailure(string UserName);

        bool IsUserNameBlocked(string name);

        void AddLoginOK(string username, string ip); 
    }
} 
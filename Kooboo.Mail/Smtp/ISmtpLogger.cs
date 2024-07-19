//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Net;

namespace Kooboo.Mail.Smtp
{
    public interface ISmtpLogger
    {
        void LogRead(IPEndPoint local, IPEndPoint remote, string message);

        void LogWrite(IPEndPoint local, IPEndPoint remote, string message);

        void LogTryConnect(string host);

        void LogConnected(IPEndPoint local, IPEndPoint remote);

        void LogConnectionFailed();
    }
}

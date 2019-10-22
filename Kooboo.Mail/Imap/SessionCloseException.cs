//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Mail.Imap
{
    public class SessionCloseException : Exception
    {
        public SessionCloseException(List<ImapResponse> list)
        {
            Response = list;
        }

        public List<ImapResponse> Response { get; set; }
    }
}
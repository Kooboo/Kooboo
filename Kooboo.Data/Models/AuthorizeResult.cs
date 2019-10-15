//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Models
{
    public class AuthorizeResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Thumbprint { get; set; }

        public AuthorizeResult()
        {
        }

        public AuthorizeResult(Exception ex) : this()
        {
            Success = false;
            ErrorMessage = ex.Message;
        }
    }

    public enum SSLType
    {
        CreateOrImport = 0,
        Renew = 1
    }
}
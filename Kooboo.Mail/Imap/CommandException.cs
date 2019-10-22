//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Mail.Imap
{
    public class CommandException : Exception
    {
        public CommandException(string tag, string errorMessage)
        {
            this.Tag = tag;
            this.ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; }

        public string Tag { get; set; }
    }
}
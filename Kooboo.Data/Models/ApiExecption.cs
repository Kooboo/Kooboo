//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Data.Models
{
    public class ApiExecption : Exception
    {
        public ApiExecption(string message, bool translated = false) : base(message)
        {
            this.Translated = translated;
        }

        public bool Translated { get; set; }
    }
}
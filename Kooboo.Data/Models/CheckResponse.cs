//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Data.Models
{
    // instead of return bool, return a model can contains more information..
    public class CheckResponse
    {
        public bool CheckResult { get; set; }

        // When failed.
        public string Message { get; set; }

        public bool Translated { get; set; }
    }
}
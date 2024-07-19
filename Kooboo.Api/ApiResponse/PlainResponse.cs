//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Api.ApiResponse
{
    public class PlainResponse : IResponse
    {
        public object Model { get; set; }
        public bool DataChange { get; set; }
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public List<FieldError> FieldErrors { get; set; }

        public string ContentType { get; set; } = "Application/Json";
        public string Content { get; set; }

        public int statusCode { get; set; } = 200;
    }
}

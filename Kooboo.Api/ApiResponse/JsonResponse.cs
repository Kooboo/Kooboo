//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;


namespace Kooboo.Api.ApiResponse
{
    public class JsonResponse : IResponse
    {
        public JsonResponse(object model)
        {
            this.Model = model;
        }

        public JsonResponse()
        {

        }

        public object Model { get; set; }

        public bool DataChange { get; set; }

        public bool Success { get; set; }

        public int? HttpCode { get; set; }

        public List<string> Messages { get; set; } = new List<string>();

        public List<FieldError> FieldErrors { get; set; } = new List<FieldError>();

    }
}

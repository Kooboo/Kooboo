//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;


namespace Kooboo.Api.ApiResponse
{
    public class ResponseBase : IResponse
    {
        public ResponseBase()
        {
            this.FieldErrors = new List<FieldError>();
            this.Messages = new List<string>();
        }
        public bool DataChange
        {
            get; set;
        }

        public List<FieldError> FieldErrors
        {
            get; set;
        }

        public List<string> Messages
        {
            get; set;
        }

        public object Model
        {
            get; set;
        }

        public bool Success
        {
            get; set;
        }
    }
}

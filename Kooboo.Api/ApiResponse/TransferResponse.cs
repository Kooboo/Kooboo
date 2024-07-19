//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Api.ApiResponse
{
    public class TransferResponse : IResponse
    {
        public TransferResponse()
        {
            this.Messages = new List<string>();
            this.FieldErrors = new List<FieldError>();
        }

        public Guid TaskId { get; set; }

        public Guid SiteId { get; set; }

        public bool DataChange
        { get; set; }

        public List<FieldError> FieldErrors { get; set; }

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


    public class SingleResponse : TransferResponse
    {
        public bool Finish { get; set; }
        public Object Page { get; set; }
    }
}

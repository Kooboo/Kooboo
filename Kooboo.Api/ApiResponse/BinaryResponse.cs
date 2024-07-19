//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using Kooboo.Data.Context;

namespace Kooboo.Api.ApiResponse
{
    public class BinaryResponse : ResponseBase
    {
        public BinaryResponse()
        {
            this.Headers = new HttpStringCollection();
            this.Success = true;
        }
        public byte[] BinaryBytes { get; set; }

        public HttpStringCollection Headers { get; set; }

        public string ContentType { get; set; }

        public Stream Stream { get; set; }
    }
}

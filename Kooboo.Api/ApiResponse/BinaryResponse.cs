//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Api.ApiResponse
{
    public class BinaryResponse : ResponseBase
    {
        public BinaryResponse()
        {
            Headers = new HttpStringCollection();
            Success = true;
        }

        public byte[] BinaryBytes { get; set; }

        public HttpStringCollection Headers { get; set; }

        public string ContentType { get; set; }
    }
}
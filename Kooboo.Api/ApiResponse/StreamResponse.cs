using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kooboo.Api.ApiResponse
{
    public class StreamResponse : IResponse
    {

        public StreamResponse()
        {
            this.Headers = new HttpStringCollection();
            this.Success = true;
        }

        public object Model { get; set; }
        public bool DataChange { get; set; }
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public List<FieldError> FieldErrors { get; set; }

        public Stream Stream { get; set; }

        public HttpStringCollection Headers { get; set; }

        public string ContentType { get; set; }
    }
}

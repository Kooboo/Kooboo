using System;
using System.Collections.Generic;
using System.Text;

namespace KScript.Api
{
    public class KApiResponse
    {
        public KApiResponse(int code, string data = null)
        {
            Code = code;
            Data = data;
        }

        public int Code { get; set; }
        public string Data { get; set; }
    }
}

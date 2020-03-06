using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Callback
{
    public class CallbackResponse
    {
        public string ContentType { get; set; } = "text/plain";

        public string Content { get; set; } = "";

        public int StatusCode { get; set; } = 200;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace KScript.Parameter
{
    public interface MailMessage
    {
        string To { get; set; }
        string From { get; set; }
        string Subject { get; set; }
        string TextBody { get; set; }
        string HtmlBody { get; set; }
    }

    public interface  RoutableText
    {
        string name { get; set; }
        string body { get; set; }
        string url { get; set; }  
    }

}

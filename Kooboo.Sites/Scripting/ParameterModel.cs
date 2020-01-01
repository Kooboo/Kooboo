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



}

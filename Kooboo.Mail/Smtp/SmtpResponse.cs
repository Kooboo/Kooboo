//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Smtp
{
    public class SmtpResponse
    {
        public bool Close { get; set; }

        public bool SessionCompleted { get; set; }

        public bool SendResponse { get; set; } = true;

        public bool StartTls { get; set; }

        public string Message { get; set; }

        public int Code { get; set; }

        public char Seperator { get; set; } = ' ';

        public string Render()
        {
            return this.Code.ToString() + this.Seperator + this.Message;
        }

    }

}

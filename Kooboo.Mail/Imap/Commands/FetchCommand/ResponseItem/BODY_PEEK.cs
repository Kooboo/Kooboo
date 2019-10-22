//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class BODY_PEEK : BODY
    {
        public override string Name
        {
            get
            {
                return "BODY.PEEK";
            }
        }

        protected override void BeforeRender(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
        }
    }
}
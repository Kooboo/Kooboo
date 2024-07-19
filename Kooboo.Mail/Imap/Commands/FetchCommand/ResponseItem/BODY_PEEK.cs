//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;


namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{

    //BODY.PEEK[<section>]<<partial>>: An alternate form of BODY[<section>] that does not implicitly set the \Seen flag.

    public class BODY_PEEK : BODY
    {
        public override string Name
        {
            get
            {
                return "BODY.PEEK";
            }
        }

        protected override void SetSeen(MailDb maildb, FetchMessage Message, DataItem dataItem)
        {
            //  An alternate form of BODY[<section>] that does not implicitly set the \Seen flag.
        }
    }
}

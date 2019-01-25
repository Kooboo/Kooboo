//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

using LumiSoft.Net.MIME;

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

        protected override void BeforeRender(MailDb maildb, FetchMessage Message, DataItem dataItem)
        {
        }
    }
}

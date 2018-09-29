//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand
{
 public  interface ICommandResponse
    {
        string Name { get; }

        List<ImapResponse> Render(MailDb maildb, FetchMessage Message, DataItem dataItem); 
    }
}

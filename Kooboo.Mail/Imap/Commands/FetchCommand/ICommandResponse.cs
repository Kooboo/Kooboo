//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand
{
    public interface ICommandResponse
    {
        string Name { get; }

        List<ImapResponse> Render(MailDb maildb, FetchMessage Message, DataItem dataItem);
    }
}

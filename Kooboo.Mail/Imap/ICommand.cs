//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap.Commands
{
    public interface ICommand
    {
        string CommandName { get; }

        bool RequireAuth { get; }
        bool RequireFolder { get; }

        // at lumisoft,some commands has additional check of arg contains two parts. Like Fetch.. 
        bool RequireTwoPartCommand { get; }

        Task<List<ImapResponse>> Execute(ImapSession session, string args);

        string AdditionalResponse { get; set; }

    }


    public static class ICommandExtensions
    {
        public static Task<List<ImapResponse>> NullResult(this ICommand command)
        {
            return Task.FromResult<List<ImapResponse>>(null);
        }
    }
}

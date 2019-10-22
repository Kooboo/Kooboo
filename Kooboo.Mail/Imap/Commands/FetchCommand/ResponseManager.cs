//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;

namespace Kooboo.Mail.Imap.Commands.FetchCommand
{
    public static class ResponseManager
    {
        private static List<ICommandResponse> _list;

        public static List<ICommandResponse> List
        {
            get
            {
                return _list ?? (_list = new List<ICommandResponse>
                {
                    new ResponseItem.FLAGS(),
                    new ResponseItem.INTERNALDATE(),
                    new ResponseItem.UID(),
                    new ResponseItem.BODY(),
                    new ResponseItem.BODY_PEEK(),
                    new ResponseItem.BODYSTRUCTURE(),
                    new ResponseItem.ENVELOPE(),
                    new ResponseItem.RFC822_SIZE(),
                    new ResponseItem.RFC822_HEADER(),
                    new ResponseItem.RFC822_TEXT(),
                    new ResponseItem.RFC822()
                });
            }
        }

        public static List<ImapResponse> GetResponse(DataItem dataItem, MailDb maildb, FetchMessage message)
        {
            string name = dataItem.Name.ToUpper();
            var item = List.Find(o => o.Name == name);

            return item.Render(maildb, message, dataItem);
        }
    }
}
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
                if (_list == null)
                {
                    _list = new List<ICommandResponse>();
                    _list.Add(new ResponseItem.FLAGS());
                    _list.Add(new ResponseItem.INTERNALDATE());
                    _list.Add(new ResponseItem.UID());
                    _list.Add(new ResponseItem.BODY());
                    _list.Add(new ResponseItem.BODY_PEEK());
                    _list.Add(new ResponseItem.BODYSTRUCTURE());
                    _list.Add(new ResponseItem.ENVELOPE());
                    _list.Add(new ResponseItem.RFC822_SIZE());
                    _list.Add(new ResponseItem.RFC822_HEADER());
                    _list.Add(new ResponseItem.RFC822_TEXT());
                    _list.Add(new ResponseItem.RFC822());
                }
                return _list;
            }
        }

        public static List<ImapResponse> GetResponse(DataItem dataItem, MailDb maildb, FetchMessage message)
        {
            string Name = dataItem.Name.ToUpper();
            var item = List.Find(o => o.Name == Name);
            if (item != null)
            {
                return item.Render(maildb, message, dataItem);
            }
            else
            {
                return new List<ImapResponse>();
            }
        }
    }
}

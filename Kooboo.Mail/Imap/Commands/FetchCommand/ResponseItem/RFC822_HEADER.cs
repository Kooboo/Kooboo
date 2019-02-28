//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Kooboo.Mail.Imap.Commands.FetchCommand.CommandReader;
using LumiSoft.Net.MIME;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class RFC822_HEADER : ICommandResponse
    {
        public virtual string Name
        {
            get
            {
                return "RFC822.HEADER"; 
            } 
        }

        public List<ImapResponse> Render(MailDb maildb, FetchMessage message, DataItem dataItem)
        {
            var parsed = LumiSoft.Net.Mail.Mail_Message.ParseFromByte(message.Bytes);
            var bytes = parsed.Header.ToByte(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
            bytes = bytes.Concat(new byte[] { 0x0D, 0x0A }).ToArray();

            var builder = new StringBuilder()
                .Append(dataItem.FullItemName).Append(" {").Append(bytes.Length).Append("}");

            return new List<ImapResponse>
            {
                new ImapResponse(builder.ToString()),
                new ImapResponse(bytes)
            };
        }
    }
}

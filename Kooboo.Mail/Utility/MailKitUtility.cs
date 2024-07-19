using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kooboo.Mail.Models;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace Kooboo.Mail.Utility
{
    public class MailKitUtility
    {
        public static ActionResponse Send(string MailFrom, string RCPTTO, string MsgBody, string remoteServer, int port, bool UseKooboo, string username, string password)
        {
            var msg = LoadMessage(MsgBody);
            if (msg != null)
            {
                var log = new SmtpOutLog();
                log.SetLogHeader(msg, RCPTTO);

                try
                {
                    SmtpClient client = new SmtpClient(log);
                    client.Connect(remoteServer, port);

                    if (username != null && password != null)
                    {
                        client.Authenticate(username, password);
                    }

                    var from = MailboxAddress.Parse(MailFrom);
                    var tos = GetMailKitAddressList(RCPTTO);
                    client.Send(msg, from, tos);

                }
                catch (Exception ex)
                {
                    log.LogItem.IsSuccess = false;
                    log.LogItem.LogLines.Add(new LogLine() { CMD = ex.Message, IsServer = true });
                }

                log.Finish();

                if (log.LogItem.IsSuccess)
                {
                    return new ActionResponse() { Success = true };
                }

                var logText = GetLog(log.LogItem);

                if (!UseKooboo)
                {
                    //this is a local sending, add sent log.
                    var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(log.LogItem.HeaderFrom);

                    if (orgDb != null)
                    {
                        var address = orgDb.Email.Find(log.LogItem.HeaderFrom);
                        if (address != null)
                        {
                            var mailDb = Kooboo.Mail.Factory.DBFactory.UserMailDb(address.UserId, orgDb.OrganizationId);

                            mailDb.SmtpDelivery.AddReport(new Models.SmtpReportIn { HeaderFrom = log.LogItem.HeaderFrom, RcptTo = RCPTTO, DateTick = DateTime.UtcNow.Ticks, IsSuccess = log.LogItem.IsSuccess, MessageId = log.LogItem.MessageId, Subject = log.LogItem.Subject, Log = logText });
                        }
                    }

                    return new ActionResponse() { Success = false, ShouldRetry = false, Message = logText }; ;
                }
                else
                {
                    return new ActionResponse() { Success = false, ShouldRetry = true, Message = logText };
                }
            }
            return new ActionResponse() { Success = false, ShouldRetry = false, Message = "Message body error" };
        }


        //This is copy from Mail Sender...
        private static string GetLog(LogItem item)
        {
            string log = item.Start.ToString("r") + "\r\n";

            bool InDataModel = false;
            bool IsServerResponse = false;
            int counter = 0;

            foreach (var line in item.LogLines)
            {
                if (line.IsClient)
                {
                    if (InDataModel)
                    {
                        counter += 1;

                        if (counter > 3 && counter < 10)
                        {
                            log += ".\r\n";
                        }

                        if (counter > 3)
                        {
                            continue;
                        }
                    }

                    log += "C: " + line.CMD + "\r\n";

                    if (line.CMD == "DATA" || line.CMD == "DATA\r\n" || line.CMD == "DATA\n")
                    {
                        InDataModel = true;
                        IsServerResponse = false;
                        counter = 0;
                    }
                }
                else if (line.IsServer)
                {
                    if (InDataModel)
                    {
                        if (!IsServerResponse) { IsServerResponse = true; }
                        else
                        {
                            InDataModel = false;
                        }
                    }

                    log += "S: " + line.CMD + "\r\n";
                }
                else
                {
                    log += line.CMD + "\r\n";
                }
            }

            return log.Replace("\r\n\r\n", "\r\n");
        }



        public static List<MailboxAddress> GetMailKitAddressList(string AddLine)
        {
            if (AddLine == null)
            {
                return null;
            }

            InternetAddressList outAddress;

            if (InternetAddressList.TryParse(AddLine, out outAddress))
            {
                List<MailboxAddress> address = new List<MailboxAddress>();
                foreach (var item in outAddress)
                {
                    var add = item as MailboxAddress;

                    if (add != null)
                    {
                        if (string.IsNullOrEmpty(add.Name))
                        {
                            add.Name = add.ToString().Split('@')[0];
                        }
                        address.Add(add);
                    }
                }
                return address;
            }

            return null;
        }

        public static MailboxAddress ParseAddress(string fullAddress)
        {
            if (fullAddress == null)
            {
                return null;
            }
            if (MailboxAddress.TryParse(fullAddress, out MailboxAddress outAddress))
            {
                return outAddress;
            }
            return null;
        }

        public static string GetEmailAddress(InternetAddressList addressList)
        {
            foreach (var item in addressList)
            {
                if (item is MailboxAddress)
                {
                    var MailAdd = item as MailboxAddress;
                    if (MailAdd != null && !string.IsNullOrEmpty(MailAdd.Address))
                    {
                        return MailAdd.Address;
                    }

                }
            }
            return null;
        }


        public static MemoryStream GetAttachmentBody(MimeEntity entity)
        {

            var contentStream = new MemoryStream();

            var part = entity as MimePart;

            if (part != null)
            {
                part.Content.DecodeTo(contentStream);

            }
            else
            {
                var midStream = new MemoryStream();
                entity.WriteToAsync(midStream, true);
                var encoding = GetEncoding(entity);
                MimeContent content = new MimeContent(midStream, encoding);
                content.DecodeTo(contentStream);
            }

            return contentStream;
        }

        private static ContentEncoding GetEncoding(MimeEntity entity)
        {
            foreach (var item in entity.Headers)
            {
                if (item.Field != null && item.Field.ToLower().Contains("transfer-encoding"))
                {
                    if (item.Value != null)
                    {
                        var lower = item.Value.ToLower().Trim();
                        if (lower == "base64")
                        {
                            return ContentEncoding.Base64;
                        }
                        else if (lower == "7bit")
                        {
                            return ContentEncoding.SevenBit;
                        }
                        else if (lower == "8bit")
                        {
                            return ContentEncoding.EightBit;
                        }
                        else if (lower == "uuencode")
                        {
                            return ContentEncoding.UUEncode;
                        }
                        else if (lower == "binary")
                        {
                            return ContentEncoding.Binary;
                        }
                    }
                }
            }

            return ContentEncoding.Default;
        }

        public static string GetFileName(MimeEntity entity)
        {

            if (entity.ContentDisposition != null && !string.IsNullOrWhiteSpace(entity.ContentDisposition.FileName))
            {
                return entity.ContentDisposition.FileName.Trim();
            }

            if (entity.ContentType != null && !string.IsNullOrWhiteSpace(entity.ContentType.Name))
            {
                return entity.ContentType.Name.Trim();
            }

            var part = entity as MimePart;

            if (part != null)
            {
                return part.FileName;
            }

            return null;
        }


        public static MimeMessage LoadMessage(string MessageBody)
        {
            System.IO.MemoryStream mo = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(MessageBody));
            var msg = MimeMessage.Load(mo);
            MailKitBodyFix.Instance.ToCorrectMessage(msg);
            return msg;
        }

        public static string GetHtmlBody(MimeMessage MimeMsg)
        {
            var HTMLParts = GetTextParts(MimeMsg, true);

            if (HTMLParts.Count > 1)
            {
                string html = null;
                foreach (var item in HTMLParts)
                {
                    var TextPart = item as TextPart;
                    if (TextPart != null)
                    {
                        html += TextPart.Text;
                    }
                }
                return html;
            }
            else
            {
                return MimeMsg.HtmlBody;
            }

        }

        public static string GetTextBody(MimeMessage MimeMsg)
        {
            var textParts = GetTextParts(MimeMsg, false);

            if (textParts.Count > 1)
            {
                string text = null;
                foreach (var item in textParts)
                {
                    var textPart = item as TextPart;
                    if (textPart != null)
                    {
                        text += textPart.Text;
                    }
                }
                return text;
            }
            else
            {
                return MimeMsg.TextBody;
            }
        }

        public static List<MimeEntity> GetTextParts(MimeMessage msg, bool IsHtml)
        {
            List<MimeEntity> result = new List<MimeEntity>();
            processItems(msg.BodyParts);

            return result;

            void processItems(IEnumerable<MimeEntity> parts)
            {
                foreach (var part in parts)
                {
                    if (part.IsAttachment)
                    {
                        continue;
                    }
                    if (part is MimeKit.Multipart)
                    {
                        var multipart = part as MimeKit.Multipart;
                        var subList = multipart.ToList();
                        processItems(subList);
                    }
                    else if (part is TextPart)
                    {
                        var item = part as TextPart;

                        if (IsHtml && item.IsHtml)
                        {
                            result.Add(item);
                        }
                        else if (!IsHtml && (item.IsPlain || item.IsRichText))
                        {
                            result.Add(item);
                        }

                    }

                }
            }


        }


    }


    public class SmtpOutLog : IProtocolLogger
    {

        public SmtpOutLog()
        {
            this.LogItem.Start = DateTime.UtcNow;
        }

        public IAuthenticationSecretDetector AuthenticationSecretDetector { get; set; }

        public LogItem LogItem { get; set; } = new LogItem();


        public void SetLogHeader(MimeMessage msg, string RcptTO)
        {
            var from = msg.From;
            var add = Kooboo.Mail.Utility.MailKitUtility.GetEmailAddress(from);

            this.LogItem.DateTick = msg.Date.ToUniversalTime().Ticks;
            this.LogItem.HeaderFrom = add;
            this.LogItem.MessageId = msg.MessageId;
            this.LogItem.Subject = msg.Subject;
            this.LogItem.RcptTo = RcptTO;
        }


        public void Fail(string error)
        {
            this.LogItem.IsSuccess = false;
            this.LogItem.errors += error;
        }

        public void Dispose()
        {

        }

        public void LogClient(byte[] buffer, int offset, int count)
        {
            var result = new byte[count];

            System.Buffer.BlockCopy(buffer, offset, result, 0, count);

            var text = System.Text.Encoding.UTF8.GetString(result);

            if (text.Length > 150)
            {
                text = text.Substring(0, 145) + "...";
            }

            // string line = "C: " + text; 
            this.LogItem.LogLines.Add(new LogLine() { IsClient = true, CMD = text });
        }

        public void LogConnect(Uri uri)
        {
            if (uri == null)
            {
                return;
            }

            string line = "connected to " + uri;

            this.LogItem.LogLines.Add(new LogLine() { CMD = line });

        }

        public void LogServer(byte[] buffer, int offset, int count)
        {
            var result = new byte[count];

            System.Buffer.BlockCopy(buffer, offset, result, 0, count);

            var text = System.Text.Encoding.ASCII.GetString(result);

            this.LogItem.LogLines.Add(new LogLine() { CMD = text, IsServer = true });
        }

        public void Finish()
        {
            this.LogItem.End = DateTime.Now;

            string log = this.LogItem.IsSuccess.ToString() + " : " + this.LogItem.HeaderFrom + " : " + this.LogItem.Start.ToString("r") + "\r\n";
            foreach (var line in this.LogItem.LogLines)
            {
                if (line.IsClient)
                {
                    log += "C: " + line.CMD + "\r\n";
                }
                else if (line.IsServer)
                {
                    log += "S: " + line.CMD + "\r\n";
                }
                else
                {
                    log += line.CMD + "\r\n";
                }
            }

            log = log.Replace("\r\n\r\n", "\r\n");

            Kooboo.Data.Log.Instance.Email.Write(log);
        }

    }

    public class LogItem
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public List<LogLine> LogLines { get; set; } = new List<LogLine>();

        public string HeaderFrom { get; set; }  // the from or envelop mail from address.   

        public string Json { get; set; }

        public long RefId { get; set; }

        public int ThreadId { get; set; }

        public long DateTick { get; set; }

        public string Subject { get; set; }

        public string RcptTo { get; set; }

        public string MessageId { get; set; }

        public Guid BodyHash { get; set; }

        public DateTime FirstReportFailed { get; set; }    // send failed log.
        public DateTime SecondReportFail { get; set; }

        public string RemoteHostOrIP { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string errors { get; set; }
    }

    public class LogLine
    {
        public bool IsClient { get; set; }  // C:

        public bool IsServer { get; set; }   // S:

        // for connect, there is no C: or S:

        public string CMD { get; set; }
    }
}
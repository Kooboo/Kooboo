//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Kooboo.Data.Models;
using Kooboo.Mail.ViewModel;
using MimeKit;
using NUglify.Helpers;
using static Kooboo.Mail.Utility.ICalendarUtility;

namespace Kooboo.Mail.Utility
{
    public static class MessageUtility
    {
        public static string CheckNFixMessage(string content)
        {
            var message = ParseMessage(content);

            bool fixMsg = false;

            if (String.IsNullOrEmpty(message.MessageId))
            {
                fixMsg = true;
                message.MessageId = GenerateMessageId(message);
            }

            // fixMsg = fixMsg | CheckNFixEntity(message); 
            if (fixMsg)
            {
                return message.ToMessageText();
            }
            else
            {
                return content;
            }
        }


        public static string GenerateMessageId(MimeMessage message)
        {
            var emailaddress = AddressUtility.GetAddress(message.From.First().ToString());
            var referenceData = $"{message.From}{message.To}{message.Date}{message.Subject}";
            if (emailaddress != null)
            {
                var seg = AddressUtility.ParseSegment(emailaddress);
                if (!string.IsNullOrEmpty(seg.Host))
                {
                    return "<" + Folder.ToId(referenceData) + "@" + seg.Host + ">";
                }
            }

            return "<" + Folder.ToId(referenceData) + "@mailprotected.com>";
        }

        //private static bool CheckNFixEntity(MIME_Entity entity)
        //{
        //    var result = false;

        //    if (entity.ContentType != null && !String.IsNullOrEmpty(entity.ContentType.Param_Boundary)
        //        && entity.ContentType.Param_Boundary.Length > 80)
        //    {
        //        result = true;
        //        entity.ContentType.Param_Boundary = Guid.NewGuid().ToString().ToLower();
        //    }

        //    var multipart = entity.Body as MIME_b_Multipart;
        //    if (multipart != null)
        //    {
        //        foreach (MIME_Entity each in multipart.BodyParts)
        //        {
        //            result |= CheckNFixEntity(each);
        //        }
        //    }

        //    return result;
        //}

        public static MimeMessage ParseMessage(string body)
        {
            return MailKitUtility.LoadMessage(body);
            //var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(body));
            //return MimeMessage.Load(stream);
        }

        public static Kooboo.Mail.Message ParseMeta(string body)
        {
            var msg = MailKitUtility.LoadMessage(body);
            if (msg != null)
            {
                return ParseMeta(msg);
            }

            return null;

        }

        public static Kooboo.Mail.Message ParseMeta(MimeMessage msg)
        {
            Kooboo.Mail.Message message = new Message();
            message.From = msg.From.ToString();
            message.To = msg.To.ToString();

            message.Cc = msg.Cc.ToString();
            message.Bcc = msg.Bcc.ToString();

            if (msg.Headers.Contains("Message-Id"))
            {
                message.SmtpMessageId = msg.Headers["Message-Id"];
            }
            else if (msg.MessageId != null)
            {
                message.SmtpMessageId = msg.MessageId;
            }

            message.Subject = msg.Subject;
            message.Date = msg.Date.UtcDateTime;

            message.Attachments = Kooboo.Mail.Models.Attachment.LoadFromMimeMessage(msg);

            /// message.Attachments = ParseAttachment(MimeMsg);
            message.Summary = ParseSummary(msg);
            return message;
        }

        public static string GetHtmlBody(MimeMessage MimeMsg)
        {
            return MailKitUtility.GetHtmlBody(MimeMsg); // MimeMsg.HtmlBody;
        }

        public static string GetTextBody(MimeMessage MimeMsg)
        {
            return MailKitUtility.GetTextBody(MimeMsg); // MimeMsg.TextBody;
        }


        public static string GetHeaderValue(MimeMessage msg, string headername)
        {
            foreach (var item in msg.Headers)
            {
                if (item != null && Lib.Helper.StringHelper.IsSameValue(item.Field, headername))
                {
                    return item.Value;
                    // var rawValue = item.RawValue; 

                    //if (rawValue !=null) 
                    // {
                    //     return System.Text.Encoding.UTF8.GetString(rawValue); 
                    // } 
                }

            }
            return null;
        }


        public static string ParseSummary(MimeMessage msg, int length = 150)
        {
            var htmlbody = msg.HtmlBody;

            if (!string.IsNullOrWhiteSpace(htmlbody))
            {
                htmlbody = Kooboo.Search.Utility.RemoveHtml(htmlbody);
                htmlbody = htmlbody.Replace("\r\n\r\n", "\r\n");
                htmlbody = htmlbody.Replace("\t\t", "\t");
                htmlbody = htmlbody.Replace("  ", " ");
            }

            if (!string.IsNullOrEmpty(htmlbody))
            {
                if (htmlbody.Length > length - 50)
                {
                    return Lib.Helper.StringHelper.SementicSubString(htmlbody, 0, length);
                }
                else
                {
                    return htmlbody;
                }
            }

            string text = msg.TextBody;

            if (!string.IsNullOrEmpty(text))
            {
                if (text.Length > length - 50)
                {
                    return Lib.Helper.StringHelper.SementicSubString(text, 0, length);
                }
                else
                {
                    return text;
                }
            }
            return null;
        }


        public static byte[] GetFileBinary(string MessageBody, string FileName)
        {
            return GetFileBinary(ParseMessage(MessageBody), FileName);
        }

        public static byte[] GetFileBinary(MimeMessage MimeMsg, string FileName, bool IncludeInlineFile = true)
        {
            if (MimeMsg == null)
            {
                return null;
            }

            foreach (var item in MimeMsg.Attachments)
            {
                if (MailKitUtility.GetFileName(item) == FileName)
                {
                    var stream = Utility.MailKitUtility.GetAttachmentBody(item);
                    return stream != null ? stream.ToArray() : null;
                }
            }

            if (IncludeInlineFile)
            {
                foreach (var item in MimeMsg.BodyParts)
                {
                    if (item is MimeEntity)
                    {
                        var entity = item as MimeEntity;
                        if (entity != null)
                        {
                            var entityFileName = MailKitUtility.GetFileName(entity);

                            if (entityFileName != null && entityFileName == FileName || System.Net.WebUtility.UrlDecode(entityFileName) == FileName || System.Net.WebUtility.UrlDecode(entityFileName) == System.Net.WebUtility.UrlDecode(FileName))
                            {
                                var stream = Utility.MailKitUtility.GetAttachmentBody(entity);
                                return stream != null ? stream.ToArray() : null;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static string GetFileNameByContentId(MimeMessage MimeMsg, string ContentId)
        {
            if (MimeMsg == null || ContentId == null)
            {
                return null;
            }

            var id = HeaderUtility.ExtraID(ContentId);
            foreach (var item in MimeMsg.BodyParts)
            {
                if (item != null && item.ContentId != null)
                {
                    if (item.ContentId == id || HeaderUtility.ExtraID(item.ContentId) == id)
                    {
                        return MailKitUtility.GetFileName(item);
                    }
                }
            }
            return null;
        }

        public static (string, byte[]) GetFileNameAndBinaryByContentId(MimeMessage MimeMsg, string ContentId)
        {
            if (MimeMsg == null || ContentId == null)
            {
                return (null, null);
            }

            var id = HeaderUtility.ExtraID(ContentId);
            foreach (var item in MimeMsg.BodyParts)
            {
                if (item != null && item.ContentId != null)
                {
                    if (item.ContentId == id || HeaderUtility.ExtraID(item.ContentId) == id)
                    {
                        var name = MailKitUtility.GetFileName(item);
                        var stream = Utility.MailKitUtility.GetAttachmentBody(item);
                        var bytes = stream != null ? stream.ToArray() : null;

                        return (name, bytes);
                    }
                }
            }
            return (null, null);

        }

        public static byte[] GetAllAttachmentZip(string MessageBody)
        {
            var msg = ParseMessage(MessageBody);
            if (msg == null)
            {
                return null;
            }

            if (msg.Attachments == null || !msg.Attachments.Any())
            {
                return null;
            }

            var memoryStream = new MemoryStream();

            using (var archive = SharpCompress.Archives.Zip.ZipArchive.Create())
            {

                foreach (var item in msg.Attachments)
                {
                    if (item.IsAttachment)
                    {
                        var bytes = MailKitUtility.GetAttachmentBody(item);
                        bytes.Position = 0;

                        var filename = MailKitUtility.GetFileName(item);

                        archive.AddEntry(filename, bytes, true);
                    }
                }

                var opt = new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.Deflate);

                opt.ArchiveEncoding.Default = System.Text.Encoding.UTF8;

                archive.SaveTo(memoryStream, opt);
            }

            memoryStream.Position = 0;

            return memoryStream.ToArray();
        }

        public static Kooboo.Mail.ViewModel.ContentViewModel GetContentViewModel(User user, int MsgId)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(maildb.OrganizationId);

            //var msg = maildb.Msgstore.Get(MsgId);
            var msg = maildb.Message2.Get(MsgId);
            if (msg == null)
            {
                return null;
            }
            var model = new Kooboo.Mail.ViewModel.ContentViewModel
            {
                Id = (int)msg.MsgId
            };
            //model.From = GetFrom(msg.From, msg.AddressId, Factory.DBFactory.OrgDb(maildb.OrganizationId));  
            var mime = ParseMessage(msg.Body);

            var fromMailboxAddress = mime.From.First() as MailboxAddress;
            var mailFromInfo = orgdb.Email.Get(fromMailboxAddress.Address);
            if (mailFromInfo != null && mailFromInfo.AddressType == EmailAddressType.Group)
            {
                var from = GetHeaderValue(mime, "Sender");
                model.From = GetAddressModel(from);
            }
            else
            {
                var from = GetHeaderValue(mime, "From");
                model.From = GetAddressModel(from);
            }

            if (!string.IsNullOrEmpty(msg.To))
            {
                model.To = GetAddressModels(msg.To);
            }
            else
            {
                model.To = new List<AddressModel>();
            }
            model.Cc = GetAddressModels(msg.Cc);
            model.Bcc = GetAddressModels(msg.Bcc);
            model.Subject = msg.Subject;
            model.InviteConfirm = msg.InviteConfirm;
            if (msg.Date == default)
            {
                model.Date = msg.CreationTime;
            }
            else
            {
                model.Date = msg.Date;
            }

            if (Folder.ReservedFolder.TryGetValue(msg.FolderId, out var result))
                model.FolderName = result.ToLower();
            else
            {
                var folder = maildb.Folder.Get(msg.FolderId);
                if (folder is not null)
                    model.FolderName = folder.Name;
            }

            model.Attachments = ViewModel.AttachmentViewModel.FromAttachments(msg.MsgId, msg.Attachments);

            model.Html = Kooboo.Mail.Utility.ComposeUtility.RestoreHtmlOrText(user, MsgId);

            model.DownloadAttachment = "/_api/EmailAttachment/msgfile/" + MsgId.ToString();

            if (msg.FolderId == Folder.ToId(Folder.Inbox))
            {
                byte[] bodyData = Encoding.UTF8.GetBytes(msg.Body);
                MemoryStream ms = new MemoryStream(bodyData);
                MimeMessage mimeMessage = MimeMessage.Load(ms);
                if (mimeMessage.BodyParts.Count() > 0)
                {
                    mimeMessage.BodyParts.ForEach(part =>
                    {
                        if ("text/calendar".Equals(part.ContentType.MimeType, StringComparison.OrdinalIgnoreCase))
                        {
                            if (part.ContentType.Parameters.TryGetValue("method", out string method))
                            {
                                string calendarContent = ((TextPart)(part)).Text;
                                List<ICalendarViewModel> calendarViewModels = ICalendarUtility.AnalysisICalendarContent(calendarContent);
                                model.Calendar = calendarViewModels;

                                // check calendar is unvalid
                                var uid = model.Calendar?[0]?.Uid;
                                if (uid != null)
                                {
                                    var calendar = maildb.Calendar.GetScheduleById(uid);
                                    if (calendar == null)
                                    {
                                        model.InviteConfirm = -1;
                                        return;
                                    }
                                }
                            }
                        }
                    });
                }
            }

            return model;
        }

        public static ViewModel.AddressModel GetAddressModel(string address)
        {
            var model = new Kooboo.Mail.ViewModel.AddressModel();
            if (address == null)
            {
                return model;
            }

            if (InternetAddress.TryParse(address, out var internetAddress))
            {
                if (internetAddress is MailboxAddress)
                {
                    var mailboxAddress = internetAddress as MailboxAddress;
                    if (!string.IsNullOrEmpty(mailboxAddress.Address))
                    {
                        var name = AddressUtility.GetDisplayName(mailboxAddress);
                        model.Name = name;
                        model.Address = mailboxAddress.Address;
                    }

                }
                else if (internetAddress is GroupAddress)
                {
                    var groupadd = internetAddress as GroupAddress;
                    var memberone = groupadd.Members.FirstOrDefault();

                    if (memberone is MailboxAddress)
                    {
                        var mailboxAddress = memberone as MailboxAddress;
                        if (!string.IsNullOrEmpty(mailboxAddress.Address))
                        {
                            var name = AddressUtility.GetDisplayName(mailboxAddress);
                            model.Name = name;
                            model.Address = mailboxAddress.Address;
                        }
                    }

                }


            }
            else
            {
                if (MailboxAddress.TryParse(address, out var mailboxAddress))
                {
                    model.Name = mailboxAddress.Name;
                    model.Address = mailboxAddress.Address;
                }
                else
                {
                    var index = address.LastIndexOf('<');
                    if (index > -1)
                    {
                        model.Name = address.Substring(0, index);
                        model.Address = AddressUtility.GetAddress(address);
                    }
                }
            }

            return model;

        }


        public static List<ViewModel.AddressModel> GetAddressModels(string addressstring)
        {

            if (string.IsNullOrWhiteSpace(addressstring))
            {
                return new List<ViewModel.AddressModel>();
            }

            //if (addressstring.Contains(";"))
            //{
            //    addressstring = addressstring.Replace(";", ",");
            //}

            List<ViewModel.AddressModel> models = new List<ViewModel.AddressModel>();

            var address = MailKitUtility.GetMailKitAddressList(addressstring);

            if (address == null)
            {
                return new List<ViewModel.AddressModel>();
            }

            foreach (var item in address)
            {
                if (!string.IsNullOrEmpty(item.Address))
                {
                    var name = AddressUtility.GetDisplayName(item);

                    models.Add(new ViewModel.AddressModel() { Name = name, Address = item.Address });
                }
            }
            return models;

        }

    }
}

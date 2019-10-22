//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using LumiSoft.Net.MIME;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Kooboo.Mail.Utility
{
    public static class MessageUtility
    {
        public static string CheckNFixMessage(string content)
        {
            var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            var message = LumiSoft.Net.MIME.MIME_Message.ParseFromStream(stream);

            bool needToFix = false;

            var messageIdField = message.Header.GetFirst("Message-ID");
            var messageId = messageIdField?.ValueToString();
            if (String.IsNullOrEmpty(messageId) || messageId.Length > 80)
            {
                needToFix = true;
                var correctField = new MIME_h_Unstructured("Message-ID", $"<{Guid.NewGuid().ToString().ToLower()}@kooboo.com>");
                if (messageIdField == null)
                {
                    message.Header.Add(correctField);
                }
                else
                {
                    message.Header.ReplaceFirst(correctField);
                }
            }

            needToFix |= CheckNFixEntity(message);

            if (needToFix)
            {
                return System.Text.Encoding.UTF8.GetString(message.ToByte());
            }
            else
            {
                return content;
            }
        }

        public static MIME_Message ParseMineMessage(string body)
        {
            var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(body));
            return LumiSoft.Net.MIME.MIME_Message.ParseFromStream(stream);
        }

        public static Kooboo.Mail.Message ParseMeta(string body)
        {
            var mimeMsg = ParseMineMessage(body);
            if (mimeMsg == null)
            {
                return null;
            }
            return ParseMeta(mimeMsg);
        }

        public static string GetHtmlBody(MIME_Message mimeMsg)
        {
            foreach (var item in mimeMsg.AllEntities)
            {
                if (item.ContentType != null)
                {
                    if (Lib.Helper.StringHelper.IsSameValue(item.ContentType.Type, "text") && Lib.Helper.StringHelper.IsSameValue(item.ContentType.SubType, "html"))
                    {
                        if (item.Body is MIME_b_Text btext)
                        {
                            return btext.Text;
                        }
                    }
                }
            }
            return null;
        }

        internal static string GetTextBody(MIME_Message mimeMsg)
        {
            foreach (var item in mimeMsg.AllEntities)
            {
                if (item.ContentType != null)
                {
                    if (Lib.Helper.StringHelper.IsSameValue(item.ContentType.Type, "text") && Lib.Helper.StringHelper.IsSameValue(item.ContentType.SubType, "plain"))
                    {
                        if (item.Body is MIME_b_Text btext)
                        {
                            return btext.Text;
                        }
                    }
                }
            }
            return null;
        }

        internal static string GetAnyTextBody(MIME_Message mimeMsg)
        {
            foreach (var item in mimeMsg.AllEntities)
            {
                if (item.ContentType != null)
                {
                    if (Lib.Helper.StringHelper.IsSameValue(item.ContentType.Type, "text"))
                    {
                        if (item.Body is MIME_b_Text btext)
                        {
                            return btext.Text;
                        }
                    }
                }
            }
            return null;
        }

        public static Kooboo.Mail.Message ParseMeta(MIME_Message mimeMsg)
        {
            Kooboo.Mail.Message message = new Message
            {
                From = GetHeaderValue(mimeMsg, "from"),
                To = GetHeaderValue(mimeMsg, "to"),
                SmtpMessageId = GetHeaderValue(mimeMsg, "Message-Id"),
                Subject = GetHeaderValue(mimeMsg, "subject"),
                Date = GetSentDate(mimeMsg),
                Attachments = ParseAttachment(mimeMsg),
                Summary = ParseSummary(mimeMsg)
            };
            return message;
        }

        private static DateTime GetSentDate(MIME_Message msg)
        {
            var value = GetHeaderValue(msg, "date");
            if (value != null)
            {
                return LumiSoft.Net.MIME.MIME_Utils.ParseRfc2822DateTime(value).ToUniversalTime();
            }
            return DateTime.UtcNow;
        }

        public static string GetHeaderValue(MIME_Message msg, string headername)
        {
            foreach (var item in msg.Header)
            {
                if (item is MIME_h_Unstructured header && Lib.Helper.StringHelper.IsSameValue(header.Name, headername))
                {
                    return header.Value;
                }
            }
            return null;
        }

        public static List<Models.Attachment> ParseAttachment(MIME_Message mineMessage)
        {
            List<Mail.Models.Attachment> result = new List<Models.Attachment>();
            foreach (var item in mineMessage.AllEntities)
            {
                if (item.ContentDisposition?.DispositionType != null && item.ContentDisposition.DispositionType.ToLower() == "attachment")
                {
                    if (item.Body is MIME_b_SinglepartBase attachmentbody)
                    {
                        Models.Attachment attach = new Models.Attachment
                        {
                            FileName = item.ContentDisposition.Param_FileName,
                            Type = item.ContentType.Type,
                            SubType = item.ContentType.SubType
                        };

                        long size = item.ContentDisposition.Param_Size;

                        if (size <= 0)
                        {
                            var x = attachmentbody;
                            size = x.Data.Length;
                        }
                        attach.Size = size;

                        if (!string.IsNullOrEmpty(attach.FileName) && attach.Size > 0)
                        {
                            result.Add(attach);
                        }
                    }
                }
            }
            return result;
        }

        internal static string ParseSummary(MIME_Message msg, int length = 150)
        {
            var htmlbody = GetHtmlBody(msg);

            if (htmlbody != null)
            {
                htmlbody = Kooboo.Search.Utility.RemoveHtml(htmlbody);
                htmlbody = htmlbody.Replace("\r\n\r\n", "\r\n");
                htmlbody = htmlbody.Replace("\t\t", "\t");
                htmlbody = htmlbody.Replace("  ", " ");
            }

            if (!string.IsNullOrEmpty(htmlbody) && htmlbody.Length > length - 50)
            {
                return Lib.Helper.StringHelper.SementicSubString(htmlbody, 0, length);
            }

            string text = GetTextBody(msg);

            int htmllen = 0;
            if (htmlbody != null)
            {
                htmllen = htmlbody.Length;
            }

            if (!string.IsNullOrEmpty(text) && text.Length > htmllen)
            {
                return Lib.Helper.StringHelper.SementicSubString(text, 0, length);
            }
            return null;
        }

        public static byte[] GetFileBinary(string messageBody, string fileName)
        {
            return GetFileBinary(ParseMineMessage(messageBody), fileName);
        }

        public static byte[] GetFileBinary(MIME_Message mimeMsg, string fileName)
        {
            if (mimeMsg == null)
            {
                return null;
            }
            List<Mail.Models.Attachment> result = new List<Models.Attachment>();
            foreach (var item in mimeMsg.AllEntities)
            {
                if ((item.ContentDisposition?.DispositionType != null && item.ContentDisposition.Param_FileName == fileName)
                    || (item.ContentType != null && item.ContentType.Param_Name == fileName))
                {
                    if (item.Body is MIME_b_SinglepartBase attachmentbody)
                    {
                        if (attachmentbody.Data != null)
                        {
                            return attachmentbody.Data;
                        }
                        else if (attachmentbody.EncodedData != null)
                        {
                            return attachmentbody.EncodedData;
                        }
                    }
                }
            }
            return null;
        }

        public static string GetFileNameByContentId(MIME_Message mimeMsg, string contentId)
        {
            if (mimeMsg == null)
            {
                return null;
            }
            List<Mail.Models.Attachment> result = new List<Models.Attachment>();
            foreach (var item in mimeMsg.AllEntities)
            {
                if (item.ContentID != null)
                {
                    if (item.ContentID == contentId || item.ContentID == "<" + contentId + ">")
                    {
                        if (item.ContentDisposition != null)
                        {
                            return item.ContentDisposition.Param_FileName;
                        }
                        else
                        {
                            return item.ContentType.Param_Name;
                        }
                    }
                }
            }
            return null;
        }

        public static byte[] GenerateAllAttachmentZip(string messageBody)
        {
            var mime = ParseMineMessage(messageBody);
            if (mime == null)
            {
                return null;
            }

            var memorystream = new MemoryStream();
            var allattachments = ParseAttachment(mime);

            if (allattachments == null || allattachments.Count == 0)
            {
                return null;
            }

            var archive = new ZipArchive(memorystream, ZipArchiveMode.Create, true);

            foreach (var item in allattachments)
            {
                if (item.FileName != null)
                {
                    var filebinary = GetFileBinary(mime, item.FileName);

                    if (filebinary != null && filebinary.Length > 0)
                    {
                        var zipfile = archive.CreateEntry(item.FileName);
                        var filestream = zipfile.Open();
                        filestream.Write(filebinary, 0, filebinary.Length);
                        filestream.Close();
                    }
                }
            }

            return memorystream.ToArray();
        }

        public static Kooboo.Mail.ViewModel.ContentViewModel GetContentViewModel(User user, int msgId)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);

            var msg = maildb.Messages.Get(msgId);
            if (msg == null)
            {
                return null;
            }

            var model = new Kooboo.Mail.ViewModel.ContentViewModel
            {
                Id = msg.Id,
                From = GetAddressModel(msg.From),
                To = GetAddressModels(msg.To),
                Cc = GetAddressModels(msg.Cc),
                Bcc = GetAddressModels(msg.Bcc),
                Subject = msg.Subject
            };

            model.Date = msg.Date == default(DateTime) ? msg.CreationTime : msg.Date;

            model.Attachments = msg.Attachments;

            model.Html = Kooboo.Mail.Utility.ComposeUtility.RestoreHtmlOrText(user, msgId);

            return model;
        }

        public static Kooboo.Mail.ViewModel.AddressModel GetAddressModel(string address)
        {
            var model = new Kooboo.Mail.ViewModel.AddressModel();
            if (address == null)
            {
                return model;
            }

            model.Address = Kooboo.Mail.Utility.AddressUtility.GetAddress(address);

            int start = address.IndexOf(" < ");

            if (start > 0)
            {
                var part = address.Substring(0, start);

                part = part.Replace("\"", "");
                part = part.Replace("'", "");

                model.Name = part;
            }

            return model;
        }

        private static List<ViewModel.AddressModel> GetAddressModels(List<string> addresses)
        {
            List<ViewModel.AddressModel> result = new List<ViewModel.AddressModel>();
            foreach (var item in addresses)
            {
                var model = GetAddressModel(item);
                result.Add(model);
            }
            return result;
        }

        private static List<ViewModel.AddressModel> GetAddressModels(string addressstring)
        {
            if (addressstring == null)
            {
                return new List<ViewModel.AddressModel>();
            }

            List<char> seprator = new List<char> {';'};
            var list = addressstring.Split(seprator.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            return GetAddressModels(list.ToList());
        }

        private static bool CheckNFixEntity(MIME_Entity entity)
        {
            var result = false;

            if (entity.ContentType != null && !String.IsNullOrEmpty(entity.ContentType.Param_Boundary)
                && entity.ContentType.Param_Boundary.Length > 80)
            {
                result = true;
                entity.ContentType.Param_Boundary = Guid.NewGuid().ToString().ToLower();
            }

            if (entity.Body is MIME_b_Multipart multipart)
            {
                foreach (MIME_Entity each in multipart.BodyParts)
                {
                    result |= CheckNFixEntity(each);
                }
            }

            return result;
        }
    }
}
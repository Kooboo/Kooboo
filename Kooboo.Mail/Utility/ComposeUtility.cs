//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Mime;

namespace Kooboo.Mail.Utility
{
    public static class ComposeUtility
    {
        public static ViewModel.ComposeViewModel ToComposeViewModel(Message message, User user)
        {
            ViewModel.ComposeViewModel model = new ViewModel.ComposeViewModel();

            var fromaddress = AddressUtility.GetAddress(message.From);
            var orgdb = Factory.DBFactory.OrgDb(user.CurrentOrgId);
            var dbaddress = orgdb.EmailAddress.Get(fromaddress);
            if (dbaddress != null)
            {
                model.From = dbaddress.Id;
            }
            model.Cc = GetAddressList(message.Cc);
            model.Bcc = GetAddressList(message.Bcc);
            model.To = GetAddressList(message.To);
            model.Subject = message.Subject;
            model.Attachments = message.Attachments;
            return model;
        }

        public static Message FromComposeViewModel(ViewModel.ComposeViewModel model, User user)
        {
            Message msg = new Message();

            if (model.MessageId != null)
            {
                msg.Id = model.MessageId.Value;
            }
            msg.From = GetFrom(model, user);
            msg.Cc = ToAddress(model.Cc);
            msg.Bcc = ToAddress(model.Bcc);
            msg.To = ToAddress(model.To);
            msg.Subject = model.Subject;
            msg.AddressId = model.From;
            msg.OutGoing = true;

            if (model.Attachments != null && model.Attachments.Count > 0)
            {
                // fill in the attachment info.  
                foreach (var item in model.Attachments)
                {
                    if (item.Size <= 0)
                    {
                        item.Size = Kooboo.Mail.MultiPart.FileService.GetSize(user, item.FileName);
                    }

                    if (string.IsNullOrEmpty(item.Type) || string.IsNullOrEmpty(item.SubType))
                    {
                        var mimetype = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(item.FileName);
                        int index = mimetype.IndexOf("/");
                        if (index == -1)
                        {
                            index = mimetype.IndexOf("\\");
                        }

                        if (index > 0)
                        {
                            item.Type = mimetype.Substring(0, index);
                            item.SubType = mimetype.Substring(index + 1);
                        }
                        else
                        {
                            item.Type = mimetype;
                        }
                    }

                    msg.Attachments.Add(item);
                }
            }

            return msg;
        }

        public static string GetFrom(ViewModel.ComposeViewModel model, User user)
        {
            var orgdb = Factory.DBFactory.OrgDb(user.CurrentOrgId);
            var dbaddress = orgdb.EmailAddress.Get(model.From);
            if (dbaddress != null)
            {
                if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
                {
                    return "\"" + user.FirstName + " " + user.LastName + "\"<" + dbaddress.Address + ">";
                }
                else
                {
                    return dbaddress.Address;
                }
            }
            return null;
        }

        public static string ComposeMessageBody(ViewModel.ComposeViewModel model, User user)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("From", GetFrom(model, user));

            var to = model.To;
            to.AddRange(model.Cc);

            headers.Add("To", ToAddress(to));
            headers.Add("Subject", model.Subject);

            string strHeader = Kooboo.Mail.Multipart.HeaderComposer.Compose(headers);

            var bodycomposer = new Kooboo.Mail.Multipart.BodyComposer(model.Html, model.Attachments, user);

            return strHeader + bodycomposer.Body();
        }
             
        public static string ComposeTextEmailBody(string from, string to, string subject, string textbody)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("From", from);  
            headers.Add("To", to);
            headers.Add("Subject", subject);
            string strHeader = Kooboo.Mail.Multipart.HeaderComposer.Compose(headers);

            string contenttype = "Content-Type:text/plain"; 
                        
            var charset = Lib.Helper.EncodingDetector.GetTextCharset(textbody); 
            if (charset !=null && charset.ToLower() != "ascii")
            {
                contenttype += "; CharSet=" + charset + ";\r\n Content-Transfer-Encoding: 8bit"; 
            }

            strHeader += contenttype + "\r\n"; 
                                                               
            string body = strHeader + "\r\n" + textbody;
            return body; 
        }

        public static string ComposeHtmlTextEmailBody(string from, string to, string subject, string htmlbody, string textbody)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("From", from);
            headers.Add("To", to);
            headers.Add("Subject", subject);
            string strHeader = Kooboo.Mail.Multipart.HeaderComposer.Compose(headers);
                
            var bodycomposer = new Kooboo.Mail.Multipart.BodyComposer(htmlbody, textbody);

            return strHeader + bodycomposer.Body();  
        }


        public static string RestoreHtmlOrText(User user, int MsgId)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);
            string body = maildb.Messages.GetContent(MsgId);

            if (string.IsNullOrEmpty(body))
            {
                return null;
            }

            var mine = MessageUtility.ParseMineMessage(body);

            if (mine == null)
            {
                return null;
            }

            string html = MessageUtility.GetHtmlBody(mine);

            if (html != null)
            {
                return Kooboo.Mail.Multipart.BodyComposer.RestoreInlineImages(html, user, MsgId);
            }

            html = MessageUtility.GetTextBody(mine);
            if (!string.IsNullOrEmpty(html))
            {
                return "<pre>" + html + "</pre>";
            }

            int index = body.IndexOf("\r\n\r\n");

            if (index > 0)
            {
                return "<pre>" + body.Substring(index + 2) + "</pre>";
            }
            else
            {
                return "<pre>" + body + "</pre>";
            }

        }

        internal static string ToAddress(List<string> address)
        {
            return string.Join(";", address);
        }

        internal static List<string> GetAddressList(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return new List<string>();
            }

            var seprator = new List<char>();
            seprator.Add(';');
            return address.Split(seprator.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        // For forward type email address. 
        public static string ComposeForwardAddressMessage(string MessageSource)
        {
            string kforwardheader = "x-kforward";

            var newbody = GetMessageBodyPart(ref MessageSource);
            if (string.IsNullOrEmpty(newbody))
            {
                return null;
            }
            // recompose the email header... 
            var mime = Mail.Utility.MessageUtility.ParseMineMessage(MessageSource);
            var forwardheader = MessageUtility.GetHeaderValue(mime, kforwardheader);
            if (!string.IsNullOrEmpty(forwardheader))
            {
                return null;
            }

            Dictionary<string, string> headers = new Dictionary<string, string>();

            string prefix = "(Forwarded By Kooboo)";

            var From = MessageUtility.GetHeaderValue(mime, "From");

            if (From.Contains(prefix))
            {
                return null;
            }

            var to = MessageUtility.GetHeaderValue(mime, "To");
            var subject = MessageUtility.GetHeaderValue(mime, "Subject");

            string fromaddress = Utility.AddressUtility.GetAddress(From);

            From = "\"" + prefix + "\"<" + fromaddress + ">";

            headers.Add("From", From);
            headers.Add("to", to);
            headers.Add("subject", subject);

            if (mime.ContentType != null && !string.IsNullOrEmpty(mime.ContentType.ValueToString()))
            {
                headers.Add("Content-Type", mime.ContentType.ValueToString());
            }

            if (!string.IsNullOrEmpty(mime.ContentTransferEncoding))
            {
                headers.Add("Content-Transfer-Encoding", mime.ContentTransferEncoding);
            }

            if (!string.IsNullOrEmpty(mime.MimeVersion))
            {
                headers.Add("MIME-Version", mime.MimeVersion);
            }

            List<string> optionalFields = new List<string>();
            //optionalFields.Add("Content-Type");
            //optionalFields.Add("MIME-Version");
            //optionalFields.Add("Content-Transfer-Encoding");
            optionalFields.Add("Charset");

            foreach (var item in optionalFields)
            {
                var optionValue = MessageUtility.GetHeaderValue(mime, item);
                if (!string.IsNullOrEmpty(optionValue))
                {
                    headers.Add(item, optionValue);
                }
            }

            var newheader = Multipart.HeaderComposer.Compose(headers);
            return newheader + "\r\n" + newbody;
        }


        public static string ComposeGroupMail(string MessageSource, string NewReplyTo)
        {
            // send the group mail... 
            // Change reply-to header to the group email address... ReGenerate the emai.... 
            return HeaderUtility.RepalceRepyTo(MessageSource, NewReplyTo);
        }


        public static string ComposeDeliveryFailed(string MailFrom, string RcptTo, string OrginalMessageBody, string errorMessage)
        {
            // string mailfrom, string rcptto, string messagebody, string errorreason
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Subject", "Delivery Status Notification (Failure)");
            headers.Add("From", "postmaster@noreply.kooboo.com");
            headers.Add("To", MailFrom);

            var newheader = Multipart.HeaderComposer.Compose(headers);

            string html = "<p>This is an automatically generated Delivery Status Notification.</p>";
            html += "<p>Delivery to recipient " + RcptTo + " failed for: </p>";
            html += "<p>" + errorMessage + "</p>";

            var bodycomposer = new Multipart.BodyComposer(html);

            return newheader + bodycomposer.Body();
        }


        private static string GetMessageBodyPart(ref string MessageSource)
        {
            int index = MessageSource.IndexOf("\r\n\r\n");
            if (index > 0)
            {
                return MessageSource.Substring(index + 4);
            }
            return null;
        }

        public static TransferEncoding GetTransferEncoding(ref string textbody, ref string htmlbody)
        {
            int totallen = 0;
            int biggercount = 0;

            if (textbody != null)
            {
                totallen += textbody.Length;

                for (int i = 0; i < textbody.Length; i++)
                {
                    if (textbody[i] > 127)
                    {
                        biggercount += 1;
                    }

                }
            }
            if (htmlbody != null)
            {
                totallen += htmlbody.Length;

                for (int i = 0; i < htmlbody.Length; i++)
                {
                    if (htmlbody[i] > 127)
                    {
                        biggercount += 1;
                    }
                }
            }

            if (biggercount == 0)
            {
                return TransferEncoding.Unknown;
            }
            else
            {
                // 5% of bigger char, make it into base64. 

                int percent5 = (int)totallen / 10;

                if (biggercount > percent5)
                {
                    return TransferEncoding.Base64;
                }
                else
                {
                    return TransferEncoding.QuotedPrintable;
                }
            }

        }


        public static string GetTransferEncodingHeader(TransferEncoding transferEncoding)
        {
            //Content - Transfer - Encoding := "BASE64" / "QUOTED-PRINTABLE" /
            //  "8BIT" / "7BIT" /
            // "BINARY" / x - token 
            if (transferEncoding == TransferEncoding.QuotedPrintable)
            {
                return "Content-Transfer-Encoding: QUOTED-PRINTABLE";
            }
            else if (transferEncoding == TransferEncoding.Base64)
            {
                return "Content-Transfer-Encoding: BASE64";
            }
            return null;
        }

        public static string GetEncoding(ref string textbody, ref string htmlbody)
        {
            if (textbody != null)
            {
                for (int i = 0; i < textbody.Length; i++)
                {
                    if (textbody[i] > 127)
                    {
                        return "UTF-8";
                    }
                }
            }
            if (htmlbody != null)
            {
                for (int i = 0; i < htmlbody.Length; i++)
                {
                    if (htmlbody[i] > 127)
                    {
                        return "UTF-8";
                    }
                }
            }

            return null;
        }


        public static string Encode(string input, TransferEncoding transferEncoding)
        {
            if (transferEncoding == TransferEncoding.QuotedPrintable)
            {
                return Kooboo.Mail.Smtp.QuotedPrintable.Encode(input);
            }
            else if (transferEncoding == TransferEncoding.Base64)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(bytes);
            }

            return input;
        }

    }


}

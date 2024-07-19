//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Kooboo.Data.Models;

namespace Kooboo.Mail.Multipart
{
    public class BodyComposer
    {
        public BodyComposer(string HTMLlBody, List<Models.Attachment> Attachments = null, User User = null)
        {
            init(HTMLlBody, null, null, User, Attachments);
        }

        public BodyComposer(string HTMLBody, string TextBody, List<Models.Attachment> attachments = null, User user = null)
        {
            init(HTMLBody, TextBody, null, user, attachments);
        }

        public BodyComposer(string HTMLBody, string TextBody, string ICalendarBody, List<Models.Attachment> attachments = null, User user = null)
        {
            init(HTMLBody, TextBody, ICalendarBody, user, attachments);
        }

        private void init(string HtmlBody, string TextBody, string ICalendarBody, User user, List<Models.Attachment> attachments)
        {
            if (user != null)
            {
                this.InlineImages = ParseInlineImages(user, ref HtmlBody);
            }

            if (string.IsNullOrEmpty(TextBody))
            {
                TextBody = Kooboo.Search.Utility.RemoveHtml(HtmlBody);
            }

            this.CharSet = Utility.ComposeUtility.GetEncoding(ref TextBody, ref HtmlBody);
            this.TransferEncoding = Utility.ComposeUtility.GetTransferEncoding(ref TextBody, ref HtmlBody);

            //StringBuilder htmlBuilder = new StringBuilder();
            //htmlBuilder.Append("<html>\r\n<head>\r\n    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>\r\n</head>\r\n<body>\r\n");
            //htmlBuilder.Append(HtmlBody);   
            //htmlBuilder.Append("\r\n</body>\r\n</html>");

            if (HtmlBody != null)
            {
                if (HtmlBody.IndexOf("</body>", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    this.HtmlBody = HtmlBody;
                }
                else
                {
                    if (this.CharSet != null)
                    {
                        this.HtmlBody = "<html><head><meta http-equiv='Content-Type' content='text/html; charset=" + this.CharSet + "'></head><body>" + HtmlBody + "</body></html>";
                    }
                    else
                    {
                        this.HtmlBody = "<html><head><meta http-equiv='Content-Type' content='text/html'></head><body>" + HtmlBody + "</body></html>";
                    }

                }
            }

            this.TextBody = TextBody;
            this.ICalendarBody = ICalendarBody;
            this.Attachments = attachments;

            this.user = user;

        }

        private string HtmlBody { get; set; }

        private string TextBody { get; set; }

        private string ICalendarBody { get; set; }

        private User user { get; set; }

        private List<InlineImageModel> _inlineImg;
        private List<InlineImageModel> InlineImages
        {
            get
            {
                if (_inlineImg == null)
                {
                    _inlineImg = new List<InlineImageModel>();
                }
                return _inlineImg;
            }
            set
            {
                _inlineImg = value;
            }
        }

        private List<Models.Attachment> _attachments;
        private List<Models.Attachment> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    _attachments = new List<Models.Attachment>();
                }
                return _attachments;
            }
            set
            {
                _attachments = value;
            }
        }

        public string CharSet { get; set; }

        public TransferEncoding TransferEncoding { get; set; }

        private string MixedBoundary = System.Guid.NewGuid().ToString().Replace("-", "");

        private string AlternativeBoundary = System.Guid.NewGuid().ToString().Replace("-", "");

        private string RelatedBoundary = System.Guid.NewGuid().ToString().Replace("-", "");

        private string ICalendarBoundary = System.Guid.NewGuid().ToString().Replace("-", "");

        public string Body()
        {
            string body = "MIME-Version: 1.0\r\n";
            if (!string.IsNullOrEmpty(ICalendarBody))
            {
                body += "Content-Type: multipart/Mixed; boundary=" + this.MixedBoundary + "\r\n\r\n";

                body += "--" + this.MixedBoundary + "\r\n";
                body += TextHtmlAlternative();
                body += "\r\n";

                foreach (var item in this.Attachments)
                {
                    string contentType = null;
                    if (!string.IsNullOrWhiteSpace(item.Type) && !string.IsNullOrWhiteSpace(item.SubType))
                    {
                        contentType = item.Type + "/" + item.SubType;
                    }
                    else
                    {
                        contentType = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(item.FileName);
                    }
                    body += "\r\n--" + this.MixedBoundary + "\r\n";
                    body += "Content-Type:" + contentType + "; name=\"" + Utility.HeaderUtility.EncodeField(item.FileName) + "\"\r\n";
                    body += "Content-Disposition:attachment;filename=\"" + Utility.HeaderUtility.EncodeField(item.FileName) + "\"\r\n";
                    body += "Content-Transfer-Encoding:base64\r\n";
                    body += "\r\n";

                    var bytes = Kooboo.Mail.MultiPart.FileService.Get(user, item.FileName);

                    body += ToBase64(bytes);
                    body += "\r\n";
                }

                body += AttachICSContent();

                body += "--" + this.MixedBoundary + "--";
                return body;
            }
            else if (this.Attachments.Any())
            {
                body += "Content-Type: multipart/Mixed;boundary=" + this.MixedBoundary + "\r\n\r\n";

                body += "--" + this.MixedBoundary + "\r\n";
                body += TextHtmlAlternative();
                body += "\r\n";

                foreach (var item in this.Attachments)
                {
                    string contentType = null;
                    if (!string.IsNullOrWhiteSpace(item.Type) && !string.IsNullOrWhiteSpace(item.SubType))
                    {
                        contentType = item.Type + "/" + item.SubType;
                    }
                    else
                    {
                        contentType = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(item.FileName);
                    }
                    body += "\r\n--" + this.MixedBoundary + "\r\n";
                    body += "Content-Type:" + contentType + ";name=\"" + Utility.HeaderUtility.EncodeField(item.FileName) + "\"\r\n";
                    body += "Content-Disposition:attachment;filename=\"" + Utility.HeaderUtility.EncodeField(item.FileName) + "\"\r\n";
                    body += "Content-Transfer-Encoding:base64\r\n";
                    body += "\r\n";

                    var bytes = Kooboo.Mail.MultiPart.FileService.Get(user, item.FileName);

                    body += ToBase64(bytes);
                    body += "\r\n";
                }
                body += "--" + this.MixedBoundary + "--";
                return body;
            }
            else
            {
                return TextHtmlAlternative();
            }
        }

        private string ICalendarContent(string method)
        {
            StringBuilder iCalendarContentBuilder = new StringBuilder();
            iCalendarContentBuilder.Append("Content-Type: text/calendar; charset=\"UTF-8\"; method=" + method + "\n");
            iCalendarContentBuilder.Append("Content-Transfer-Encoding: base64\n\n");
            iCalendarContentBuilder.Append(ToBase64(Encoding.UTF8.GetBytes(ICalendarBody)));

            return iCalendarContentBuilder.ToString();
        }

        private string AttachICSContent()
        {
            StringBuilder attachmentICSBuilder = new StringBuilder();
            attachmentICSBuilder.Append("\r\n--" + this.MixedBoundary + "\r\n");
            attachmentICSBuilder.Append("Content-Type:application/ics; name=\"invite.ics\"\n");
            attachmentICSBuilder.Append("Content-Disposition:attachment;filename=\"invite.ics\"\n");
            attachmentICSBuilder.Append("Content-Transfer-Encoding: base64\n\n");
            attachmentICSBuilder.Append(ToBase64(Encoding.UTF8.GetBytes(ICalendarBody)));

            return attachmentICSBuilder.ToString();
        }

        public string TextHtmlAlternative()
        {
            var alternative = _html_text_alternative();

            if (!this.InlineImages.Any())
            {
                return alternative;
            }
            else
            {
                string body = "Content-Type: multipart/related; boundary=" + this.RelatedBoundary + "\r\n\r\n";

                body += "--" + this.RelatedBoundary + "\r\n";

                body += alternative;

                body += "\r\n";

                foreach (var item in this.InlineImages)
                {
                    body += "--" + this.RelatedBoundary + "\r\n";
                    var contentType = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(item.FileName);
                    body += "Content-Type:" + contentType + ";name=" + Utility.HeaderUtility.EncodeField(item.FileName) + "\r\n";
                    body += "Content-Transfer-Encoding:base64\r\n";
                    body += "Content-Disposition:inline;filename=\"" + Utility.HeaderUtility.EncodeField(item.FileName) + "\"\r\n";
                    body += "Content-ID:<" + item.ContentId + ">\r\n";
                    body += "\r\n";

                    body += ToBase64(item.Binary);
                    body += "\r\n";
                }
                body += "--" + this.RelatedBoundary + "--\r\n";
                return body;
            }
        }

        private string _html_text_alternative()
        {
            string body = null;

            body += "Content-Type: multipart/alternative;boundary=" + this.AlternativeBoundary;

            if (this.CharSet != null)
            {
                body += "; CharSet=" + this.CharSet;
            }

            body += "\r\n\r\n";

            body += "This is a multipart message in MIME format";
            body += "\r\n\r\n";

            body += "--" + this.AlternativeBoundary + "\r\n";
            body += "Content-Type: text/plain";
            if (this.CharSet != null)
            {
                body += ";CharSet=" + this.CharSet;
            }

            if (this.TransferEncoding != TransferEncoding.Unknown)
            {
                body += "\r\n";
                body += Utility.ComposeUtility.GetTransferEncodingHeader(this.TransferEncoding);
            }

            body += "\n\n";

            body += Utility.ComposeUtility.Encode(this.TextBody, this.TransferEncoding);

            body += "\r\n";

            body += "--" + this.AlternativeBoundary + "\r\n";

            body += Html();

            body += "\r\n";

            if (ICalendarBody != null)
            {
                body += "--" + this.AlternativeBoundary + "\r\n";
                Calendar calendar = Calendar.Load(ICalendarBody);
                CalendarEvent calendarEvent = calendar.Events[0];
                body += ICalendarContent(calendarEvent.Calendar.Method);
                body += "\r\n";
            }

            body += "--" + this.AlternativeBoundary + "--";

            return body;
        }

        public string Html()
        {
            string body = "Content-Type: text/html";
            if (this.CharSet != null)
            {
                body += ";CharSet=" + this.CharSet;
            }

            if (this.TransferEncoding != TransferEncoding.Unknown)
            {
                body += "\r\n";
                body += Utility.ComposeUtility.GetTransferEncodingHeader(this.TransferEncoding);
            }

            body += "\r\n\r\n" + Utility.ComposeUtility.Encode(this.HtmlBody, this.TransferEncoding) + "\r\n";


            return body;
        }


        public string Text()
        {
            string body = "Content-Type: text/plain";
            if (this.CharSet != null)
            {
                body += ";CharSet=" + this.CharSet;
            }
            if (this.TransferEncoding != TransferEncoding.Unknown)
            {
                body += "\r\n";
                body += Utility.ComposeUtility.GetTransferEncodingHeader(this.TransferEncoding);
            }
            body += "\r\n\r\n" + Utility.ComposeUtility.Encode(this.TextBody, this.TransferEncoding) + "\r\n";
            return body;
        }

        public static List<InlineImageModel> ParseInlineImages(User user, ref string html)
        {
            List<InlineImageModel> result = new List<Multipart.InlineImageModel>();

            var inlineimages = GetInlineImageElements(html);

            if (inlineimages == null || inlineimages.Count == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            int currentindex = 0;
            int totallen = html.Length;

            foreach (var item in inlineimages.OrderBy(o => o.location.openTokenStartIndex))
            {
                string src = item.getAttribute("src");

                if (string.IsNullOrWhiteSpace(src))
                {
                    continue;
                }
                src = src.Trim();

                string filename = null;
                byte[] bytes = null;

                if (src.StartsWith(InlineImagePrefix))
                {
                    filename = src.Substring(InlineImagePrefix.Length);
                    bytes = Kooboo.Mail.MultiPart.FileService.Get(user, filename);
                }
                else if (src.StartsWith(InlineImageMessageFilePrefix))
                {
                    filename = src.Substring(InlineImageMessageFilePrefix.Length);
                    bytes = MsgFile(user, ref filename);
                }
                else if (Lib.Utilities.DataUriService.isDataUri(src))
                {
                    var uriItem = Lib.Utilities.DataUriService.PraseDataUri(src);
                    if (uriItem.isBase64)
                    {
                        var exten = MimeMapping.MimeUtility.GetExtensions(uriItem.MineType);
                        if (exten != null && exten.Any())
                        {
                            filename = Lib.Security.ShortGuid.GetNewShortId() + "." + exten.FirstOrDefault();

                            bytes = Convert.FromBase64String(uriItem.DataString);
                        }

                    }

                }
                else
                {
                    string relative = Kooboo.Lib.Helper.UrlHelper.RelativePath(src);

                    if (relative.StartsWith(InlineImagePrefix))
                    {
                        filename = relative.Substring(InlineImagePrefix.Length);
                        bytes = Kooboo.Mail.MultiPart.FileService.Get(user, filename);
                    }
                    else if (relative.StartsWith(InlineImageMessageFilePrefix))
                    {
                        filename = relative.Substring(InlineImageMessageFilePrefix.Length);
                        bytes = MsgFile(user, ref filename);
                    }
                }


                if (bytes != null)
                {
                    sb.Append(html.Substring(currentindex, item.location.openTokenStartIndex - currentindex));

                    InlineImageModel model = new InlineImageModel();
                    model.Binary = bytes;
                    model.FileName = filename;
                    model.ContentId = Lib.Security.ShortGuid.GetNewShortId();
                    result.Add(model);

                    string newsrc = "cid:" + model.ContentId;
                    string outerhtml = item.OuterHtml;

                    outerhtml = outerhtml.Replace(src, newsrc);
                    sb.Append(outerhtml);

                    currentindex = item.location.endTokenEndIndex + 1;
                }
            }

            if (currentindex < totallen)
            {
                sb.Append(html.Substring(currentindex, totallen - currentindex));
            }

            html = sb.ToString();
            return result;
        }


        private static byte[] MsgFile(User user, ref string originalFileName)
        {
            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                return null;
            }

            originalFileName = originalFileName.Trim();
            originalFileName = originalFileName.Replace("\\", "/");

            var spe = "/".ToCharArray();

            string[] para = originalFileName.Split(spe, StringSplitOptions.RemoveEmptyEntries);

            string filename = null;
            int messageid = 0;


            if (para.Count() == 2)
            {
                messageid = Convert.ToInt32(para[0]);
                filename = System.Web.HttpUtility.UrlDecode(para[1]);
                originalFileName = filename;
            }
            else if (para.Count() == 1)
            {
                messageid = Convert.ToInt32(para[0]);
            }

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);

            ///var content = maildb.Msgstore.GetContent(messageid);
            var content = maildb.Message2.GetContent(messageid);

            if (!string.IsNullOrEmpty(content))
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    return Kooboo.Mail.Utility.MessageUtility.GetFileBinary(content, filename);

                }
                else
                {
                    return Mail.Utility.MessageUtility.GetAllAttachmentZip(content);
                }
            }

            return null;
        }


        public static string RestoreInlineImages(string html, User user, int MsgId)
        {
            var cidImages = GetCidImageElements(html);

            if (cidImages == null || cidImages.Count == 0)
            {
                return html;
            }
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);
            var msgbody = maildb.Message2.GetContent(MsgId);
            if (string.IsNullOrEmpty(msgbody))
            {
                return html;
            }

            var mimeMsg = Kooboo.Mail.Utility.MessageUtility.ParseMessage(msgbody);

            StringBuilder sb = new StringBuilder();
            int currentindex = 0;
            int totallen = html.Length;

            foreach (var item in cidImages.OrderBy(o => o.location.openTokenStartIndex))
            {
                string src = item.getAttribute("src");

                string contentid = src.Trim().Substring("cid:".Length);

                var filename = Kooboo.Mail.Utility.MessageUtility.GetFileNameByContentId(mimeMsg, contentid);

                if (!string.IsNullOrEmpty(filename))
                {

                    sb.Append(html.Substring(currentindex, item.location.openTokenStartIndex - currentindex));

                    string newsrc = InlineImageMessageFilePrefix + MsgId.ToString() + "/" + System.Web.HttpUtility.UrlEncode(filename);
                    string outerhtml = item.OuterHtml;

                    outerhtml = outerhtml.Replace(src, newsrc);
                    sb.Append(outerhtml);
                    currentindex = item.location.endTokenEndIndex + 1;
                }
            }

            if (currentindex < totallen)
            {
                sb.Append(html.Substring(currentindex, totallen - currentindex));
            }

            return sb.ToString();
        }

        public static string InlineImagePrefix = "/_api/emailattachment/file/";

        public static string InlineImageMessageFilePrefix = "/_api/emailattachment/msgfile/";

        public static List<Kooboo.Dom.Element> GetInlineImageElements(string htmlbody)
        {
            List<Kooboo.Dom.Element> result = new List<Dom.Element>();

            // format = "/_api/emailattachment/file/{filename};  
            var dom = Kooboo.Dom.DomParser.CreateDom(htmlbody);

            foreach (var item in dom.images.item)
            {
                string src = item.getAttribute("src");

                if (string.IsNullOrEmpty(src))
                {
                    continue;
                }

                src = src.Trim();
                if (src.StartsWith(InlineImagePrefix) || src.StartsWith(InlineImageMessageFilePrefix))
                {
                    result.Add(item);
                }
                else
                {
                    if (src.ToLower().StartsWith("http"))
                    {
                        var relative = Kooboo.Lib.Helper.UrlHelper.RelativePath(src);

                        if (relative != null && (relative.StartsWith(InlineImagePrefix) || relative.StartsWith(InlineImageMessageFilePrefix)))
                        {
                            result.Add(item);
                        }
                    }
                    else if (Lib.Utilities.DataUriService.isDataUri(src))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public static List<Kooboo.Dom.Element> GetCidImageElements(string htmlbody)
        {
            List<Kooboo.Dom.Element> result = new List<Dom.Element>();

            var dom = Kooboo.Dom.DomParser.CreateDom(htmlbody);

            foreach (var item in dom.images.item)
            {
                string src = item.getAttribute("src");

                if (string.IsNullOrEmpty(src))
                {
                    continue;
                }
                src = src.Trim().ToLower();
                if (src.StartsWith("cid:"))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static string ToBase64(byte[] binary)
        {
            var value = Convert.ToBase64String(binary);

            var len = value.Length;

            StringBuilder sb = new StringBuilder();

            int index = 0;
            int nextindex = 0;

            while (true)
            {
                nextindex = index + 76;
                if (nextindex > len)
                {
                    sb.Append(value.Substring(index)).Append("\r\n");
                    break;
                }
                else
                {
                    sb.Append(value.Substring(index, 76)).Append("\r\n");
                    index = index + 76;
                }
            }
            return sb.ToString();
        }


    }

}
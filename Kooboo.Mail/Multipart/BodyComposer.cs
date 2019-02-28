//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Multipart
{
    public class BodyComposer
    {
        //always should be below format. 
        //mixed
        //alternative
        //text
        //related
        //html
        //inline image
        //inline image
        //attachment
        //attachment

        public BodyComposer(string HtmlBody, List<Models.Attachment> Attachments = null, User User = null)
        {
            init(HtmlBody, null, User, Attachments);
        }

        public BodyComposer(string htmlbody, string TextBody, List<Models.Attachment> attachments = null, User user = null)
        {
            init(htmlbody, TextBody, user, attachments);
        }

        private void init(string HtmlBody, string TextBody, User user, List<Models.Attachment> attachments)
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

            this.HtmlBody = HtmlBody;
            this.TextBody = TextBody;
            this.Attachments = attachments;

            this.user = user;

        }

        private string HtmlBody { get; set; }

        private string TextBody { get; set; }

        private User user { get; set; }

        private List<InlineImageModel> InlineImages { get; set; }

        private List<Models.Attachment> Attachments { get; set; }

        public string CharSet { get; set; }

        public TransferEncoding TransferEncoding { get; set; }


        private string _mixedBoundary;
        private string MixedBondary
        {
            get
            {
                if (_mixedBoundary == null)
                {
                    _mixedBoundary = Kooboo.Lib.Security.ShortGuid.GetNewShortId();
                }
                return _mixedBoundary;
            }
        }

        private string _alterBoundary;
        private string AlterBoundary
        {
            get
            {
                if (_alterBoundary == null)
                {
                    _alterBoundary = Kooboo.Lib.Security.ShortGuid.GetNewShortId();
                }
                return _alterBoundary;
            }
        }

        private string _relatedBoundary;
        private string RelatedBoundary
        {
            get
            {
                if (_relatedBoundary == null)
                {
                    _relatedBoundary = Kooboo.Lib.Security.ShortGuid.GetNewShortId();
                }
                return _relatedBoundary;
            }
        }

        public string HeaderContentType()
        {
            string header = string.Empty;
            if (this.Attachments != null && this.Attachments.Count > 0)
            {
                header += "MIME-Version: 1.0\r\n";
                if (this.TransferEncoding != TransferEncoding.Unknown)
                {
                    header += Utility.ComposeUtility.GetTransferEncodingHeader(this.TransferEncoding) + "\r\n";
                }

                header += "Content-Type: multipart/mixed;";
                if (this.CharSet != null)
                {
                    header += "Charset=" + this.CharSet + ";";
                }
                header += "boundary=\"" + this.MixedBondary + "\"; ";

            }
            else
            {
                header += "MIME-Version: 1.0\r\n";
                if (this.TransferEncoding != TransferEncoding.Unknown)
                {
                    header += Utility.ComposeUtility.GetTransferEncodingHeader(this.TransferEncoding) + "\r\n";
                }

                header += "Content-Type: multipart/alternative;";
                if (this.CharSet != null)
                {
                    header += "Charset=" + this.CharSet + ";";
                }
                header += "boundary=\"" + this.AlterBoundary + "\"; ";
            }

            return header;
        }

        public string Body()
        {

            if (this.Attachments != null && this.Attachments.Count() > 0)
            {
                // has attachmented with mixed.   
                string body = "MIME-Version: 1.0\r\nContent-Type: multipart/Mixed; boundary=" + this.MixedBondary;

                if (this.CharSet != null)
                {
                    body += "\r\nContent-Transfer-Encoding: 8Bit";
                }

                body += "\r\n\r\nThis is a multipart message in MIME format\r\n\r\n";

                body += "--" + this.MixedBondary + "\r\n";
                body += TextHtmlAlternative(false);

                body += "\r\n";

                foreach (var item in this.Attachments)
                {
                    string contenttype = null;
                    if (!string.IsNullOrWhiteSpace(item.Type) && !string.IsNullOrWhiteSpace(item.SubType))
                    {
                        contenttype = item.Type + "/" + item.SubType;
                    }
                    else
                    {
                        contenttype = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(item.FileName);
                    }
                    body += "\r\n--" + this.MixedBondary + "\r\n";
                    body += "Content-Type:" + contenttype + "; name=\"" + Utility.HeaderUtility.EncodeField(item.FileName) + "\"\r\n";

                    body += "Content-Disposition:attachment;filename=\"" + Utility.HeaderUtility.EncodeField(item.FileName) + "\"\r\n";
                    body += "Content-Transfer-Encoding:base64\r\n";
                    body += "\r\n";

                    var bytes = Kooboo.Mail.MultiPart.FileService.Get(user, item.FileName);

                    body += ToBase64(bytes);
                    body += "\r\n\r\n";
                }

                body += "--" + this.MixedBondary + "--";
                return body;
            }
            else
            {
                return TextHtmlAlternative(true);
            }
        }

        public string TextHtmlAlternative(bool mimemsessage = false)
        {
            string body = null;

            if (mimemsessage)
            {
                body = "MIME-Version: 1.0\r\n";
            }

            body += "Content-Type: multipart/alternative; boundary=" + this.AlterBoundary;

            if (this.CharSet != null)
            {
                body += "; CharSet=" + this.CharSet;
            }

            body += "\r\n\r\n";

            if (mimemsessage)
            {
                body += "This is a multipart message in MIME format";
                body += "\r\n\r\n";
            }

            body += "--" + this.AlterBoundary + "\r\n";
            body += "Content-Type: text/plain";
            if (this.CharSet != null)
            {
                body += "; CharSet=" + this.CharSet;
            }

            if (this.TransferEncoding != TransferEncoding.Unknown)
            {
                body += "\r\n";
                body += Utility.ComposeUtility.GetTransferEncodingHeader(this.TransferEncoding);
            }


            body += "\r\n\r\n";
            body += Utility.ComposeUtility.Encode(this.TextBody, this.TransferEncoding);

            body += "\r\n";

            body += "--" + this.AlterBoundary + "\r\n";

            body += Html();

            body += "--" + this.AlterBoundary + "--";

            return body;
        }

        public string Html()
        {
            string body = null;
            if (this.InlineImages == null || this.InlineImages.Count == 0)
            {
                body = "Content-Type: text/html";
                if (this.CharSet != null)
                {
                    body += "; CharSet=" + this.CharSet;
                }

                if (this.TransferEncoding != TransferEncoding.Unknown)
                {
                    body += "\r\n";
                    body += Utility.ComposeUtility.GetTransferEncodingHeader(this.TransferEncoding);
                }

                body += "\r\n\r\n" + Utility.ComposeUtility.Encode(this.HtmlBody, this.TransferEncoding) + "\r\n";
            }
            else
            {
                body = "Content-Type: multipart/related; boundary=" + this.RelatedBoundary;
                if (this.CharSet != null)
                {
                    body += "; CharSet=" + this.CharSet;
                }

                body += "\r\n\r\n";

                body += "--" + this.RelatedBoundary + "\r\n";

                if (this.TransferEncoding != TransferEncoding.Unknown)
                {
                    body += Utility.ComposeUtility.GetTransferEncodingHeader(this.TransferEncoding);
                    body += "\r\n";
                }

                body += "Content-Type: text/html";
                if (this.CharSet != null)
                {
                    body += "; CharSet=" + this.CharSet;
                }
                body += "\r\n\r\n";

                body += Utility.ComposeUtility.Encode(this.HtmlBody, this.TransferEncoding);

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
            }

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
                    bytes = MsgFile(user, filename);  
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
                        bytes = MsgFile(user, filename);
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


        private static byte[] MsgFile(User user, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            path = path.Trim();
            path = path.Replace("\\", "/");

            var spe = "/".ToCharArray(); 

            string[] para = path.Split(spe,StringSplitOptions.RemoveEmptyEntries); 
              
            string filename = null;
            int messageid = 0;

    

            if (para.Count() == 2)
            {
                messageid = Convert.ToInt32(para[0]);
                filename = System.Web.HttpUtility.UrlDecode(para[1]);
            }
            else if (para.Count() ==1)
            {
                messageid = Convert.ToInt32(para[0]);
            }

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);

            var content = maildb.Messages.GetContent(messageid);
             
            if (!string.IsNullOrEmpty(content))
            {
                if (!string.IsNullOrEmpty(filename))
                {
                   return Kooboo.Mail.Utility.MessageUtility.GetFileBinary(content, filename);
               
                }
                else
                {
                    return Mail.Utility.MessageUtility.GenerateAllAttachmentZip(content); 
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
            var msgbody = maildb.Messages.GetContent(MsgId);
            if (string.IsNullOrEmpty(msgbody))
            {
                return html;
            }

            var mimeMsg = Kooboo.Mail.Utility.MessageUtility.ParseMineMessage(msgbody);

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

        internal static string InlineImagePrefix = "/_api/emailattachment/file/";

        internal static string InlineImageMessageFilePrefix = "/_api/emailattachment/msgfile/";

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

                        if (relative !=null && (relative.StartsWith(InlineImagePrefix) || relative.StartsWith(InlineImageMessageFilePrefix)))
                        {
                            result.Add(item); 
                        }
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
                nextindex = index + 254;
                if (nextindex > len)
                {
                    sb.Append(value.Substring(index)).Append("\r\n");
                    break;
                }
                else
                {
                    sb.Append(value.Substring(index,254)).Append("\r\n");
                    index = index + 254;
                }
            }
            return sb.ToString();
        }


    }



}

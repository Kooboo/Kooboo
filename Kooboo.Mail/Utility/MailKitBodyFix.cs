using System;
using System.Linq;
using MimeKit;

namespace Kooboo.Mail.Utility
{
    public class MailKitBodyFix
    {

        public static MailKitBodyFix Instance { get; set; } = new MailKitBodyFix();

        public MimeMessage ToCorrectMessage(MimeMessage msg)
        {
            if (!IsMultiplePart(msg))
            {
                EnsureMultipart(msg);
            }
            //apple mail.
            ChangeAppleMailInlineToAttachment(msg);
            CheckInlineImageHaveNotDeclaration(msg);
            return msg;
        }
        private static void CheckInlineImageHaveNotDeclaration(MimeMessage msg)
        {
            msg.BodyParts.ToList().ForEach(part =>
            {
                if ((!string.IsNullOrEmpty(part.ContentId)) && part.ContentDisposition is null)
                {
                    ContentDisposition contentDisposition = ContentDisposition.Parse("inline");
                    contentDisposition.FileName = part.ContentType.Name;
                    part.ContentDisposition = contentDisposition;
                }
            });
        }

        public void ChangeAppleMailInlineToAttachment(MimeMessage msg)
        {
            if (msg == null || msg.BodyParts == null || !msg.BodyParts.Any())
            {
                return;
            }

            var htmlparts = MailKitUtility.GetTextParts(msg, true);
            // apple mail, when only plain text, should not have inline image.
            if (htmlparts == null || !htmlparts.Any())
            {
                foreach (var item in msg.BodyParts)
                {
                    if (item.ContentType.MimeType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (item.ContentDisposition != null && item.ContentDisposition.Disposition != null && item.ContentDisposition.Disposition.ToLower().Trim() == "inline")
                    {
                        item.ContentDisposition.IsAttachment = true;
                        //item.ContentDisposition.Disposition = "attachment";
                    }
                }
            }
        }

        public void EnsureMultipart(MimeMessage msg)
        {
            // single part, must be text or html.
            var html = msg.HtmlBody;
            var text = msg.TextBody;

            if (string.IsNullOrWhiteSpace(html) && string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                text = Lib.Utilities.HtmlUtility.ConvertToText(html);
            }

            if (string.IsNullOrWhiteSpace(html))
            {
                html = Lib.Utilities.HtmlUtility.ConvertToHtml(text);
            }

            var textPart = new TextPart("plain")
            {
                Text = text
            };

            var textEncoding = GetEncoding(ref text);
            if (textEncoding == ContentEncoding.Base64 || textEncoding == ContentEncoding.QuotedPrintable)
            {
                textPart.ContentTransferEncoding = textEncoding;
            }

            var htmlPart = new TextPart("html")
            {
                Text = html
            };

            var htmlEncoding = GetEncoding(ref html);
            if (htmlEncoding == ContentEncoding.Base64 || htmlEncoding == ContentEncoding.QuotedPrintable)
            {
                htmlPart.ContentTransferEncoding = htmlEncoding;
            }

            // now create the multipart/mixed container to hold the message text and the
            // image attachment
            var multipart = new MultipartAlternative();   // Multipart("mixed");
            multipart.Boundary = "--" + Lib.Security.Hash.ComputeGuidIgnoreCase(msg.MessageId).ToString();
            multipart.Add(textPart);
            multipart.Add(htmlPart);

            msg.Body = multipart;

            //var newtext = msg.ToMessageText();
            //System.IO.MemoryStream mo = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(newtext));
            //msg = MimeMessage.Load(mo);
        }


        private MimeKit.ContentEncoding GetEncoding(ref string body)
        {
            int totallen = 0;
            int biggercount = 0;

            if (body != null)
            {
                totallen += body.Length;

                for (int i = 0; i < body.Length; i++)
                {
                    if (body[i] > 127)
                    {
                        biggercount += 1;
                    }
                }
            }

            if (biggercount == 0)
            {
                return ContentEncoding.QuotedPrintable;
            }
            else
            {

                return ContentEncoding.Base64;

            }

        }


        public bool IsMultiplePart(MimeMessage msg)
        {
            if (msg == null)
            {
                return false;
            }

            if (msg.BodyParts.Count() > 1)
            {
                return true;
            }

            return msg.Body is MimeKit.Multipart;
        }


        public string ToCorrectMsgBody(string originalSource)
        {
            var msg = Utility.MailKitUtility.LoadMessage(originalSource);

            var result = msg.ToMessageText();

            return result;
        }

    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;
using MimeKit;

namespace Kooboo.Mail.Models
{
    public class Attachment
    {
        public string FileName { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }

        public long Size { get; set; }


        public static List<Attachment> LoadFromMimeMessage(MimeMessage msg)
        {
            List<Attachment> Attachments = new List<Attachment>();

            foreach (var item in msg.BodyParts.Where(o => o.IsAttachment || (!string.IsNullOrEmpty(o?.ContentDisposition?.FileName) && string.IsNullOrEmpty(o?.ContentId))).ToArray())
            {
                var newAtt = loadFromMimeEntity(item);
                if (newAtt != null)
                {
                    Attachments.Add(newAtt);
                }
            }
            return Attachments;
        }

        private static Attachment loadFromMimeEntity(MimeEntity entity)
        {
            if (entity == null)
            {
                return null;
            }
            Attachment result = new Attachment();

            if (entity.ContentDisposition != null)
            {
                result.FileName = entity.ContentDisposition.FileName;

                if (entity.ContentDisposition.Size.HasValue)
                {
                    result.Size = entity.ContentDisposition.Size.Value;
                }
            }

            result.Type = entity.ContentType.MediaType;
            result.SubType = entity.ContentType.MediaSubtype;

            if (result.Size == 0)
            {
                var body = Utility.MailKitUtility.GetAttachmentBody(entity);
                if (body != null)
                {
                    result.Size = body.Length;
                }
            }

            return result;

        }
    }
}

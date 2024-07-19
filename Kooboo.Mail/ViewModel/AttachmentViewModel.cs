using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Mail.Models;
using MimeKit;

namespace Kooboo.Mail.ViewModel
{
    public class AttachmentViewModel : Attachment
    {

        public string downloadUrl { get; set; }

        public static List<AttachmentViewModel> FromAttachments(long MsgId, List<Attachment> attachments)
        {
            if (attachments == null || !attachments.Any())
            {
                return null;
            }

            List<AttachmentViewModel> result = new List<AttachmentViewModel>();

            foreach (var item in attachments)
            {
                AttachmentViewModel model = new AttachmentViewModel();
                model.FileName = item.FileName;
                model.SubType = item.SubType;
                model.Type = item.Type;

                // if subtype is octet-stream, check the file real type
                if (!string.IsNullOrEmpty(item.SubType) && item.SubType.Equals("octet-stream", StringComparison.OrdinalIgnoreCase) && item.FileName != default)
                {
                    var fileMimeMapping = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(item.FileName);

                    if (ContentType.TryParse(fileMimeMapping, out var contentType))
                    {
                        model.SubType = contentType.MediaSubtype;
                        model.Type = contentType.MediaType;
                    }
                }

                model.Size = item.Size;

                model.downloadUrl = "/_api/EmailAttachment/msgfile/" + MsgId.ToString() + "/" + System.Web.HttpUtility.UrlEncode(item.FileName);

                result.Add(model);
            }

            return result;
        }

    }

}

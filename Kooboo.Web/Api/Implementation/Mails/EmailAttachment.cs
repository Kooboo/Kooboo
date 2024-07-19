//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using System.Net.Http;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Context;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailAttachmentApi : IApi
    {
        private const int MaxAttachmentSize = 10 * 1024 * 1024;
        private const int MaxImageSize = 1 * 1024 * 1024;

        public string ModelName
        {
            get
            {
                return "EmailAttachment";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        //attachment image. 
        public BinaryResponse File(ApiCall call)
        {
            string filename = call.Command.Value;

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var method = nameof(EmailAttachmentApi.MsgFile) + "/" + filename;
                if (!string.IsNullOrEmpty(filename))
                {
                    method += "/" + filename;
                }

                var fileBytes = EmailForwardManager.Post(this.ModelName, method, call.Context.User, null, null);
                var response = new BinaryResponse();
                response.Headers.Add("Content-Disposition", $"filename={System.Web.HttpUtility.UrlEncode(filename)}");
                response.BinaryBytes = fileBytes;
                return response;
            }

            var bytes = Kooboo.Mail.MultiPart.FileService.Get(call.Context.User, filename);

            if (bytes != null && bytes.Length > 0)
            {
                var response = new BinaryResponse();

                response.ContentType = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(filename);
                response.Headers.Add("Content-Disposition", $"filename={System.Web.HttpUtility.UrlEncode(filename)}");
                response.BinaryBytes = bytes;
                return response;
            }
            return null;
        }

        // formate. /_api/EmailAttachment/msgfile/{messageid}/filename 
        public BinaryResponse MsgFile(ApiCall call)
        {
            string filename = null;
            int messageId = 0;

            var para = call.Command.Parameters;

            if (para.Count() == 2)
            {
                messageId = Convert.ToInt32(para[0]);
                filename = System.Web.HttpUtility.UrlDecode(para[1]);
            }
            else
            {
                messageId = Convert.ToInt32(call.Command.Value);
            }

            var forceDownload = call.GetValue<bool>("forceDownload");

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var method = nameof(EmailAttachmentApi.MsgFile) + "/" + messageId;
                if (!string.IsNullOrEmpty(filename))
                {
                    method += "/" + System.Web.HttpUtility.UrlEncode(filename);
                }

                var dict = new Dictionary<string, string>
                {
                    { "forceDownload", forceDownload.ToString() }
                };

                var bytes = EmailForwardManager.Post(this.ModelName, method, call.Context.User, null, dict);
                filename = !string.IsNullOrEmpty(filename) ? System.Web.HttpUtility.UrlEncode(filename) : "attachment.zip";
                var response = GenerateBinaryResponse(bytes, forceDownload, string.IsNullOrEmpty(filename), filename);
                return response;
            }
            var mailDB = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var content = mailDB.Message2.GetContent(messageId);


            if (!string.IsNullOrEmpty(content))
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    var bytes = Kooboo.Mail.Utility.MessageUtility.GetFileBinary(content, filename);

                    if (bytes != null && bytes.Length > 0)
                    {
                        var response = GenerateBinaryResponse(bytes, forceDownload, false, filename);
                        return response;
                    }
                }
                else
                {
                    var bytes = Mail.Utility.MessageUtility.GetAllAttachmentZip(content);
                    var response = GenerateBinaryResponse(bytes, forceDownload, true, "attachment.zip");
                    return response;
                }
            }

            return null;
        }

        private BinaryResponse GenerateBinaryResponse(byte[] bytes, bool forceDownload, bool downloadAll, string fileName)
        {
            var response = new BinaryResponse();
            if (!downloadAll)
            {
                response.ContentType = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(fileName);
                if (forceDownload)
                {
                    response.ContentType = "application/octet-stream";
                    response.Headers.Add("Content-Disposition", $"attachment;filename={System.Web.HttpUtility.UrlEncode(fileName)}"); ;
                }
                else
                {
                    response.Headers.Add("Content-Disposition", $"filename={System.Web.HttpUtility.UrlEncode(fileName)}"); ;
                }
                bool textIndex = "text/plain".Equals(response.ContentType, StringComparison.OrdinalIgnoreCase);
                if (textIndex)
                {
                    response.ContentType = "text/plain; charset=UTF-8";
                }
            }
            else
            {
                response.ContentType = "application/zip";
                response.Headers.Add("Content-Disposition", $"filename={fileName}");
            }
            response.BinaryBytes = bytes;
            return response;
        }

        private (string boundary, byte[] binary) GetBinaryFromForm(HttpRequest request)
        {
            string boundary = "---------------------------" + Guid.NewGuid().ToString("N");
            var httpContent = new MultipartFormDataContent(boundary);
            foreach (var item in request.Files)
            {
                httpContent.Add(new ByteArrayContent(item.Bytes), item.FileName, item.FileName);
            }

            foreach (string item in request.Forms.Keys)
            {
                httpContent.Add(new StringContent(request.Forms.Get(item)), item);
            }
            var postData = httpContent.ReadAsByteArrayAsync().Result;
            return (boundary, postData);
        }


        public object AttachmentPost(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                // call.Context.HttpContext.Request.Form.
                var (Boundary, Binary) = GetBinaryFromForm(call.Context.Request);
                return EmailForwardManager.Post<object>(this.ModelName, nameof(EmailAttachmentApi.AttachmentPost), call.Context.User, Binary, null, $"multipart/form-data; boundary={Boundary}");
            }

            if (!call.Context.Request.Files.Any())
                return null;

            var file = call.Context.Request.Files[0];
            if (file.Bytes.Length > MaxAttachmentSize)
            {
                return new JsonResponse
                {
                    Success = false,
                    Messages = new List<string> { $"File size must be less than {MaxAttachmentSize / 1024 / 1024}" }
                };
            }

            var fileName = call.Context.Request.Forms.Get("filename");

            fileName = Lib.Helper.StringHelper.ToValidFileName(fileName);

            Kooboo.Mail.MultiPart.FileService.Upload(call.Context.User, fileName, file.Bytes);

            return new Mail.Models.Attachment() { FileName = fileName, Size = file.Bytes.Length, Type = file.ContentType };
        }


        public void DeleteAttachment(string filename, ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("filename", filename);
                EmailForwardManager.Post<bool>(this.ModelName, nameof(EmailAttachmentApi.DeleteAttachment), call.Context.User, dic);
                return;
            }
            Kooboo.Mail.MultiPart.FileService.DeleteFile(call.Context.User, filename);
        }

        public object ImagePost(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                var (Boundary, Binary) = GetBinaryFromForm(call.Context.Request);
                //string convert to object will throw exception
                var url = EmailForwardManager.Post<string>(this.ModelName, nameof(EmailAttachmentApi.ImagePost), call.Context.User, Binary, null, $"multipart/form-data; boundary={Boundary}");
                if (!string.IsNullOrEmpty(url))
                {
                    return url;
                }

                return new JsonResponse
                {
                    Success = false,
                    Messages = new List<string> { $"File size must be less than {MaxImageSize / 1024 / 1024}" }
                };
            }

            if (!call.Context.Request.Files.Any())
                return null;

            var file = call.Context.Request.Files[0];
            if (file.Bytes.Length > MaxImageSize)
            {
                return new JsonResponse
                {
                    Success = false,
                    Messages = new List<string> { $"File size must be less than {MaxImageSize / 1024 / 1024}" }
                };
            }

            var fileName = call.Context.Request.Forms.Get("fileName");

            fileName = Lib.Helper.StringHelper.ToValidFileName(fileName);

            Kooboo.Mail.MultiPart.FileService.Upload(call.Context.User, fileName, file.Bytes);

            return $"/_api/emailattachment/file/" + System.Web.HttpUtility.UrlEncode(fileName);
        }

    }

}
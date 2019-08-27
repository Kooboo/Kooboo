//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                var filebytes = EmailForwardManager.Post(this.ModelName, method, call.Context.User, null, null);
                var response = new BinaryResponse();
                response.Headers.Add("Content-Disposition", $"filename={System.Web.HttpUtility.UrlEncode(filename)}");
                response.BinaryBytes = filebytes;
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

        // formate. msgfile/{messageid}/filename. 
        public BinaryResponse MsgFile(ApiCall call)
        {
            string filename = null;
            int messageid = 0;

            var para = call.Command.Parameters;

            if (para.Count() == 2)
            {
                messageid = Convert.ToInt32(para[0]);
                filename = System.Web.HttpUtility.UrlDecode(para[1]);
            }
            else
            {
                messageid = Convert.ToInt32(call.Command.Value);
            }

            if (EmailForwardManager.RequireForward(call.Context))
            {
                var method = nameof(EmailAttachmentApi.MsgFile) + "/" + messageid;
                if (!string.IsNullOrEmpty(filename))
                {
                    method += "/" + filename;
                }

                var bytes = EmailForwardManager.Post(this.ModelName, method, call.Context.User, null, null);

                filename = !string.IsNullOrEmpty(filename) ? System.Web.HttpUtility.UrlEncode(filename) : "attachment.zip";
                var response = new BinaryResponse();
                response.ContentType = "application/zip";
                response.Headers.Add("Content-Disposition", $"filename={filename}");
                response.BinaryBytes = bytes;
                return response;
            }
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(call.Context.User);

            var content = maildb.Messages.GetContent(messageid);
             

            if (!string.IsNullOrEmpty(content))
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    var bytes = Kooboo.Mail.Utility.MessageUtility.GetFileBinary(content, filename);
                    if (bytes != null && bytes.Length > 0)
                    {
                        var response = new BinaryResponse();

                        response.ContentType = Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GetMimeMapping(filename);
                        response.Headers.Add("Content-Disposition", $"filename={System.Web.HttpUtility.UrlEncode(filename)}");
                        response.BinaryBytes = bytes;
                        return response;
                    }
                }
                else
                {
                    var bytes = Mail.Utility.MessageUtility.GenerateAllAttachmentZip(content); 
                    var response = new BinaryResponse();
                    response.ContentType = "application/zip";
                    response.Headers.Add("Content-Disposition", $"filename=attachment.zip");
                    response.BinaryBytes = bytes;
                    return response;
                } 
            }

            return null;
        }

        public object AttachmentPost(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                return EmailForwardManager.Post<object>(this.ModelName, nameof(EmailAttachmentApi.AttachmentPost), call.Context.User, call.Context.Request.PostData, null);
            }

            var form = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(call.Context.Request.PostData);
            if (!form.Files.Any())
                return null;

            var file = form.Files[0];
            if (file.Bytes.Length > MaxAttachmentSize)
            {
                return new JsonResponse
                {
                    Success = false,
                    Messages = new List<string> { $"File size must be less than {MaxAttachmentSize / 1024 / 1024}" }
                };
            }

            var fileName = form.FormData["filename"];

            fileName = Lib.Helper.StringHelper.ToValidFileName(fileName);

            Kooboo.Mail.MultiPart.FileService.Upload(call.Context.User, fileName, file.Bytes);
            return new Mail.Models.Attachment() { FileName = fileName };
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
                //string convert to object will throw exception
                var url =EmailForwardManager.Post<string>(this.ModelName, nameof(EmailAttachmentApi.ImagePost), call.Context.User, call.Context.Request.PostData, null);
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

            var form = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(call.Context.Request.PostData);
            if (!form.Files.Any())
                return null;

            var file = form.Files[0];
            if (file.Bytes.Length > MaxImageSize)
            {
                return new JsonResponse
                {
                    Success = false,
                    Messages = new List<string> { $"File size must be less than {MaxImageSize / 1024 / 1024}" }
                };
            }

            var fileName = form.FormData["fileName"];

            fileName = Lib.Helper.StringHelper.ToValidFileName(fileName);

            Kooboo.Mail.MultiPart.FileService.Upload(call.Context.User, fileName, file.Bytes);

            return $"/_api/emailattachment/file/" + System.Web.HttpUtility.UrlEncode(fileName);
        }


    }
}

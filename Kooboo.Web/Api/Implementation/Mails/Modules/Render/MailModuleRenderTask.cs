using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Typescript;
using Kooboo.Mail.Extension;
using Kooboo.Mail.Models;
using Kooboo.Sites;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Render.View;

namespace Kooboo.Web.Api.Implementation.Mails.Modules.Render
{
    public class MailModuleRenderTask
    {
        public static MailModuleRenderTask instance { get; set; } = new MailModuleRenderTask();

        public Kooboo.Api.ApiResponse.IResponse Render(ApiCall call, Mail.OrgDb orgDb, MailModule module, string file, string objectType)
        {
            if (string.IsNullOrEmpty(objectType))
            {
                objectType = "root";
            }

            var type = Lib.Helper.EnumHelper.GetEnum<EnumMailResourceType>(objectType);


            var mailModuleContext = MailModuleContext.FromRenderContext(call.Context, module);

            if (type == EnumMailResourceType.read || type == EnumMailResourceType.compose)
            {
                // read or compose may has message id. 
            }

            MailResourceType resouceType = new MailResourceType(type);

            var diskHandler = DiskHandler.FromMailModuleContext(mailModuleContext, resouceType.Name);


            var FileInfo = MailModuleRenderHelper.TryGetFileInfo(mailModuleContext, file, false);

            if (FileInfo == null)
            {
                call.Context.Response.StatusCode = 404;
                return null;
            }


            if (type == EnumMailResourceType.api)
            {
                return RenderCode(mailModuleContext, resouceType, FileInfo);
            }
            else if (type == EnumMailResourceType.read || type == EnumMailResourceType.compose || type == EnumMailResourceType.backend)
            {
                return RenderView(mailModuleContext, resouceType, FileInfo);
            }
            else
            {

                if (resouceType.IsText)
                {
                    return RenderText(mailModuleContext, resouceType, FileInfo);
                }
                else if (resouceType.IsBinary)
                {
                    return RenderBinary(mailModuleContext, resouceType, FileInfo);
                }
            }

            return null;
        }

        public Kooboo.Api.ApiResponse.IResponse RenderText(MailModuleContext context, MailResourceType type, ModuleFileInfo info)
        {
            var res = new Kooboo.Api.ApiResponse.PlainResponse();

            if (System.IO.File.Exists(info.FullDiskFileName))
            {
                var text = System.IO.File.ReadAllText(info.FullDiskFileName);

                if (type.Type == EnumMailResourceType.js)
                {
                    var header = GetScriptHeader(context);
                    text = header + text;
                }
                res.Content = text;
                res.ContentType = Lib.Helper.IOHelper.MimeType(info.FullDiskFileName);

                return res;
            }
            return null;
        }

        public Kooboo.Api.ApiResponse.IResponse RenderBinary(MailModuleContext context, MailResourceType type, ModuleFileInfo info)
        {
            var res = new Kooboo.Api.ApiResponse.BinaryResponse();

            if (System.IO.File.Exists(info.FullDiskFileName))
            {
                var binary = System.IO.File.ReadAllBytes(info.FullDiskFileName);
                res.BinaryBytes = binary;
                res.ContentType = Lib.Helper.IOHelper.MimeType(info.FullDiskFileName);
                return res;
            }
            return null;
        }


        public Kooboo.Api.ApiResponse.IResponse RenderCode(MailModuleContext context, MailResourceType type, ModuleFileInfo info)
        {
            var res = new Kooboo.Api.ApiResponse.PlainResponse();


            if (System.IO.File.Exists(info.FullDiskFileName))
            {
                res.ContentType = "application/javascript";

                var code = System.IO.File.ReadAllText(info.FullDiskFileName);
                var cache = TypescriptCache.Instance.GetOrCreate(Guid.Empty, context.Module.Id, code, info.FullDiskFileName);
                res.Content = Sites.Scripting.Manager.ExecuteCode(context.RenderContext, cache, new Sites.Scripting.ExecuteOptions
                {
                    EnableDebug = false,
                    ForceType = ScriptType.Classic
                });

                return res;
            }
            return null;

        }


        public Kooboo.Api.ApiResponse.IResponse RenderView(MailModuleContext context, MailResourceType type, ModuleFileInfo info)
        {
            string result = MailViewRender.Render(context, type, info);

            if (!string.IsNullOrEmpty(result))
            {
                var res = new Kooboo.Api.ApiResponse.PlainResponse();
                res.Content = result;
                res.ContentType = type.DefaultContentType;

                return res;
            }

            return null;

        }


        public string GetScriptHeader(MailModuleContext context)
        {
            var basurl = context.GetBaseUrl();
            return @$"
            if(!window.k) window.k = {{}};
            k.mailModuleUrl ='{basurl}';
            k.makeUrl = function(file){{ return k.mailModuleUrl + '&file=' + file}};
            ";
        }

    }
}

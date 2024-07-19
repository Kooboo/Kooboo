using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Mail.Extension;
using Kooboo.Web.Api.Implementation.Mails.Modules.Render;

namespace Kooboo.Web.Api.Implementation.Mails.Modules
{
    public class MailExtensionRenderApi : IApi
    {
        public string ModelName => "MailExtension";

        public bool RequireSite => false;

        public bool RequireUser => true;

        // render one module. 
        public Kooboo.Api.ApiResponse.IResponse Render(Guid ModuleId, string file, ApiCall call)
        {
            var objectType = call.GetValue("objectType");
            if (objectType != null)
            {
                if (objectType.ToLower() == "compose")
                {
                    return RenderCompose(ModuleId, call);
                }
                else if (objectType.ToLower() == "read")
                {
                    return RenderRead(ModuleId, call);
                }
                else if (objectType.ToLower() == "backend")
                {
                    return RenderBackend(ModuleId, call);
                }
            }

            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(ModuleId);

            if (module == null)
            {
                throw new Exception("Module not found");
            }

            return MailModuleRenderTask.instance.Render(call, orgDb, module, file, objectType);
        }

        public Kooboo.Api.ApiResponse.IResponse RenderRead(Guid ModuleId, ApiCall call)
        {
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(ModuleId);

            if (module == null)
            {
                throw new Exception("Module not found");
            }

            var context = MailModuleContext.FromRenderContext(call.Context, module);

            var resType = new MailResourceType(EnumMailResourceType.read);

            return MailModuleRenderTask.instance.RenderView(context, resType, null);
        }

        public Kooboo.Api.ApiResponse.IResponse RenderCompose(Guid ModuleId, ApiCall call)
        {
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(ModuleId);

            if (module == null)
            {
                throw new Exception("Module not found");
            }

            var context = MailModuleContext.FromRenderContext(call.Context, module);

            var resType = new MailResourceType(EnumMailResourceType.compose);

            return MailModuleRenderTask.instance.RenderView(context, resType, null);
        }


        public Kooboo.Api.ApiResponse.IResponse RenderBackend(Guid ModuleId, ApiCall call)
        {
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var module = orgDb.Module.Get(ModuleId);

            if (module == null)
            {
                throw new Exception("Module not found");
            }

            var context = MailModuleContext.FromRenderContext(call.Context, module);

            var resType = new MailResourceType(EnumMailResourceType.backend);

            return MailModuleRenderTask.instance.RenderView(context, resType, null);
        }



        public List<ExtendResult> ExtendRead(long MessageId, ApiCall call)
        {
            // return ExtendList(call, true);
            return ExtendList2(call, true);
        }

        public List<ExtendResult> ExtendCompose(ApiCall call)
        {
            var messageid = call.GetLongValue("messageid");
            ///return ExtendList(call, false); 
            return ExtendList2(call, false);
        }

        private Kooboo.Api.ApiResponse.IResponse ExtendList(ApiCall call, bool IsRead)
        {
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var modules = orgDb.Module.List();

            string RenderResult = null;

            foreach (var item in modules)
            {
                if (!item.Online)
                {
                    continue;
                }

                var mailContext = MailModuleContext.CreateNewFromRenderContext(call.Context, item);

                call.Context.SetItem<MailModuleContext>(mailContext);

                Kooboo.Api.ApiResponse.IResponse ModuleResult = null;

                if (IsRead)
                {
                    ModuleResult = RenderRead(item.Id, call);
                }
                else
                {
                    ModuleResult = RenderCompose(item.Id, call);
                }


                if (ModuleResult != null)
                {
                    var plain = ModuleResult as Kooboo.Api.ApiResponse.PlainResponse;
                    if (plain != null)
                    {
                        RenderResult += plain.Content;
                    }
                }

            }

            if (!string.IsNullOrEmpty(RenderResult))
            {
                var res = new Kooboo.Api.ApiResponse.PlainResponse();

                res.Content = RenderResult;
                res.ContentType = "text/html;charset=utf-8";
                return res;
            }

            return null;
        }


        private List<ExtendResult> ExtendList2(ApiCall call, bool IsRead)
        {
            List<ExtendResult> result = new List<ExtendResult>();

            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var modules = orgDb.Module.List();


            foreach (var item in modules)
            {
                if (!item.Online)
                {
                    continue;
                }

                var mailContext = MailModuleContext.CreateNewFromRenderContext(call.Context, item);

                call.Context.SetItem<MailModuleContext>(mailContext);

                Kooboo.Api.ApiResponse.IResponse ModuleResult = null;

                if (IsRead)
                {
                    ModuleResult = RenderRead(item.Id, call);
                }
                else
                {
                    ModuleResult = RenderCompose(item.Id, call);
                }

                if (ModuleResult != null && ModuleResult is PlainResponse)
                {
                    var plain = ModuleResult as PlainResponse;

                    result.Add(new ExtendResult() { Name = item.Name, Content = plain.Content });
                }

            }

            return result;
        }


    }

    public class ExtendResult
    {
        public string Name { get; set; }
        public string Content { get; set; }

    }



}

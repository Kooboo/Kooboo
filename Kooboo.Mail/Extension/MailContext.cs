using System;
using Kooboo.Data.Context;
using Kooboo.Mail.Events;
using Kooboo.Mail.Models;


namespace Kooboo.Mail.Extension
{

    public class MailModuleContext
    {
        public Message Message { get; set; }

        public EnumMailEvent Event { get; set; }

        [Kooboo.Data.Attributes.KIgnore]
        public OrgDb OrgDb { get; set; }

        public MailDb MailDb { get; set; }

        //For module own database storage. 
        //public string DbPath { get; set; }

        public MailModule Module { get; set; }

        public RenderContext RenderContext { get; set; }

        // the executing view path...
        public string ExecutingView { get; set; }

        public string RootFolder { get; set; }

        public string Culture { get; set; }

        //  public ModuleRequest Request { get; set; }

        public string GetBaseUrl()
        {
            var request = this.RenderContext.Request;
            var orgid = this.RenderContext.User.CurrentOrgId.ToString();

            var uri = new UriBuilder(request.Scheme, request.Host, request.Port, "/_api/MailExtension/Render").Uri;

            var url = uri.AbsoluteUri;

            url += "?moduleId=" + this.Module.Id.ToString();

            return url;
        }

        public static MailModuleContext FromRenderContext(RenderContext context, MailModule module)
        {
            var exists = context.GetItem<MailModuleContext>();
            if (exists != null)
            {
                return exists;
            }

            MailModuleContext modulecontext = new MailModuleContext();
            modulecontext.RenderContext = context;

            var moduleroot = MailModuleHelper.ModuleRootFolder(context);

            modulecontext.RootFolder = System.IO.Path.Combine(moduleroot, module.Name);

            modulecontext.Module = module;

            modulecontext.Culture = context.Culture;

            context.SetItem<MailModuleContext>(modulecontext);

            return modulecontext;
        }

        public static MailModuleContext CreateNewFromRenderContext(RenderContext context, MailModule module)
        {
            MailModuleContext modulecontext = new MailModuleContext();
            modulecontext.RenderContext = context;

            var moduleroot = MailModuleHelper.ModuleRootFolder(context);

            modulecontext.RootFolder = System.IO.Path.Combine(moduleroot, module.Name);

            modulecontext.Module = module;

            modulecontext.Culture = context.Culture;

            return modulecontext;
        }

        //Make the url, merge with the base...
        public string MakeModuleUrl(string moduleRelativeUrl)
        {
            var url = this.GetBaseUrl();
            return url + "&view=" + moduleRelativeUrl;
        }

    }






}

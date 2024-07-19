using Kooboo.Data.Context;
using Kooboo.Mail.Extension;
using Kooboo.Sites.Render;
using Kooboo.Web.Api.Implementation.Mails.Modules.Render;

namespace Kooboo.Sites.ScriptModules.Render.View
{

    public class MailModuleSrcRenderTask : IRenderTask
    {
        private string srcValue { get; set; }

        private ValueRenderTask RenderValueTask { get; set; }

        private bool IsExternalLink { get; set; }

        // special, no change. 
        private bool IsSpecial { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public string tagName { get; set; }

        public string objectType { get; set; }

        public MailModuleSrcRenderTask(string tagname, string srcValue)
        {
            this.srcValue = srcValue;
            this.tagName = tagname.ToLower();

            this.IsSpecial = Service.DomUrlService.IsSpecialUrl(this.srcValue);

            if (!IsSpecial)
            {
                if (Service.DomUrlService.IsExternalLink(this.srcValue) || this.srcValue == "#")
                {
                    this.IsExternalLink = true;
                }
            }

            if (this.tagName == "img")
            {
                this.objectType = "img";
            }
            else if (this.tagName == "script")
            {
                this.objectType = "js";
            }
            else if (this.tagName == "link")
            {
                this.objectType = "css";
            }
        }

        public string Render(RenderContext context)
        {
            if (this.IsExternalLink)
            {
                return this.srcValue;
            }

            var mailContext = context.GetItem<MailModuleContext>();

            if (mailContext == null)
            {
                return null;
            }

            var fileName = this.srcValue.Replace("/", "\\");


            ModuleFileInfo fileinfo = MailModuleRenderHelper.TryGetFileInfo(mailContext, fileName, false);

            if (fileinfo != null)
            {
                var baseUrl = mailContext.GetBaseUrl();

                return baseUrl + "&file=" + System.Net.WebUtility.UrlEncode(fileinfo.FullName) + "&objectType=" + this.objectType;
            }

            return null;
        }


        // The file of Image/File/Css/Js. 
        private ModuleFileInfo TryGetFileInfo(MailModuleContext mailContext, string fileName)
        {
            var rootDisk = DiskHandler.FromMailModuleContext(mailContext, "");
            var fileinfo = rootDisk.GetFileInfo("", fileName);
            if (fileinfo == null)
            {
                foreach (var item in MailResourceTypeManager.AvailableResource())
                {
                    if (item.Type != EnumMailResourceType.root && item.Type != EnumMailResourceType.undefined && item.Type != EnumMailResourceType.read && item.Type != EnumMailResourceType.compose && item.Type != EnumMailResourceType.backend)
                    {
                        fileinfo = rootDisk.GetFileInfo(item.Name, fileName);
                        if (fileinfo != null)
                        {
                            break;
                        }
                    }
                }
            }

            // try to take out the first /img/a.png 
            return null;
        }

        // TODO: check if this method is needed.  get the value to repalce {key}
        private string PraseBracket(RenderContext context, string result)
        {
            if (result == null)
            {
                return null;
            }
            int start = result.IndexOf("{");
            if (start == -1)
            {
                return result;
            }

            int end = result.IndexOf("}");

            if (end > start && start > -1)
            {
                string key = result.Substring(start + 1, end - start - 1);

                object value = context.DataContext.GetValue(key);

                string strvalue = null;
                if (value != null)
                {
                    strvalue = value.ToString();
                }

                if (!string.IsNullOrEmpty(strvalue) && !strvalue.Contains("{") && !strvalue.Contains("}"))
                {
                    string replace = "{" + key + "}";

                    string newvalue = result.Replace(replace, strvalue);

                    return PraseBracket(context, newvalue);
                }
                else
                {
                    return result;
                }

            }
            else
            {
                return result;
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }
    }


}

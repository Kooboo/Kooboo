using Kooboo.Data.Context;
using Kooboo.Sites.Render;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Render.Customized
{
    public class AdminVersionRenderTask : IRenderTask
    {
        public AdminVersionRenderTask(string url, bool isStyle = false, bool IsImage = false)
        {
            if (url != null)
            {
                IsGroup = Kooboo.Sites.Service.GroupService.IsGroupUrl(url);
                this.Url = url;
                this.IsStyle = isStyle;
                this.IsImage = IsImage;
            }
        }

        // only has style or script now. 
        public bool IsStyle { get; set; }

        public bool IsGroup { get; set; }

        public bool IsImage { get; set; }

        public string Url { get; set; }

        public bool ClearBefore => false;

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }

        public string Render(RenderContext context)
        {
            var version = GetVersion(context);

            if (version == null)
            {
                return this.Url;
            } 

            if (this.Url == null)
            {
                return null;
            }

            string result = this.Url;

            if (result.Contains("text.js"))
            {
                result = result + "?lang=" + GetCulture(context); 
            }

            if (result.Contains("?"))
            {
              return  result + "&version=" + version;
            }
            else
            {
              return    result + "?version=" + version;
            } 
        }

        public string GetVersion(RenderContext context)
        { 
            return Data.AppSettings.Version.ToString(); 
        }

        public string GetCulture(RenderContext context)
        {
            string culture = null;
            if (context.User != null)
            {
                culture = context.User.Language;
            }

            if (string.IsNullOrWhiteSpace(culture))
            {
                if (!string.IsNullOrWhiteSpace(context.Culture))
                {
                    culture = context.Culture;
                }
            }

            if (string.IsNullOrWhiteSpace(culture))
            {
                culture = Kooboo.Data.AppSettings.CmsLang; 
            }

            return culture;
        }


    }







}

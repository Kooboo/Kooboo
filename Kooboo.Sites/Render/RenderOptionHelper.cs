using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render
{
    public static class RenderOptionHelper
    {

        public static EvaluatorOption GetPageOption(FrontContext context)
        {
            EvaluatorOption renderoption = new EvaluatorOption();

            if (context.Page.Headers.HasValue())
            {
                if (context.Page.HasLayout)
                {
                    renderoption.RenderHeader = false;
                }
                else
                {
                    renderoption.RenderHeader = true;
                }
            }
            else
            {
                renderoption.RenderHeader = false;
            }

            renderoption.OwnerObjectId = context.Page.Id;

            SetOption(renderoption, context.RenderContext);

            return renderoption;
        }


        public static EvaluatorOption GetFormOption(RenderContext context, Guid FormId)
        {
            EvaluatorOption option = new EvaluatorOption();
            option.OwnerObjectId = FormId;
            SetOption(option, context);
            return option;
        }

        public static EvaluatorOption GetHeaderOption(RenderContext context)
        {
            EvaluatorOption option = new EvaluatorOption(); 

            SetOption(option, context);
            option.RequireBindingInfo = false;
            option.RenderHeader = false;
            option.RenderUrl = false;
            return option;
        }

        public static EvaluatorOption GetLayoutOption(RenderContext context, Guid LayoutId)
        {
            EvaluatorOption options = new EvaluatorOption();
            options.RenderHeader = true;
            options.OwnerObjectId = LayoutId;

            SetOption(options, context);
            return options;
        }


        public static EvaluatorOption GetViewOption(RenderContext context, Guid ViewId)
        {
            EvaluatorOption options = new EvaluatorOption();

            SetOption(options, context);

            options.RenderUrl = true;
            options.RenderHeader = false;
            options.OwnerObjectId = ViewId;

            return options; 
        }

        public static void SetOption(EvaluatorOption option, RenderContext context)
        {
            if (context.WebSite != null)
            {
                if (context.WebSite.EnableJsCssBrowerCache)
                {
                    option.EnableJsCssBrowserCache = context.WebSite.EnableJsCssBrowerCache;
                }

                if (context.WebSite.EnableImageBrowserCache && context.WebSite.ImageCacheDays <= 0)
                {
                    option.EnableImageBrowserCache = true;
                }
                else
                {
                    option.EnableImageBrowserCache = false;
                }
            }

            option.RenderUrl = RenderUrl(context);
            option.RequireBindingInfo = RenderBindingInfo(context);

        }

        private static bool RenderUrl(RenderContext context)
        {

            if (context.WebSite != null && context.WebSite.EnableSitePath)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool RenderBindingInfo(RenderContext context)
        {
            return context.Request.Channel == Data.Context.RequestChannel.InlineDesign;
        }

    }
}

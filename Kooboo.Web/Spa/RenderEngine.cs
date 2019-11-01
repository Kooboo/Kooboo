//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Render;

namespace Kooboo.Web.Spa
{
    public class RenderEngine
    {
        private static Sites.Render.Commands.ICommandSourceProvider GetSourceProvider(RenderContext context, SpaRenderOption option)
        {
            Sites.Render.Commands.ICommandSourceProvider sourceprovider;
            if (!context.HasItem<Sites.Render.Commands.ICommandSourceProvider>("commandsource"))
            {
                sourceprovider = new CommandDiskSourceProvider(option);
                context.SetItem<Sites.Render.Commands.ICommandSourceProvider>(sourceprovider, "commandsource");
            }
            else
            {
                sourceprovider = context.GetItem<Sites.Render.Commands.ICommandSourceProvider>("commandsource");
            }
            return sourceprovider;
        }

        public static RenderRespnose Render(RenderContext context, SpaRenderOption option, string relativeurl)
        {
            var sourceprovider = GetSourceProvider(context, option);

            var fileType = RenderHelper.GetFileType(relativeurl);
            RenderRespnose response = new RenderRespnose();

            switch (fileType)
            {
                case UrlFileType.Image:

                    return RenderImage(context, option, relativeurl);

                case UrlFileType.JavaScript:

                    response.ContentType = "application/javascript";
                    response.BinaryBytes = sourceprovider.GetBinary(context, relativeurl);
                    break;

                case UrlFileType.Style:

                    response.ContentType = "text/css";
                    response.BinaryBytes = sourceprovider.GetBinary(context, relativeurl);

                    break;

                case UrlFileType.File:

                    string contenttype = IOHelper.MimeType(relativeurl);
                    if (string.IsNullOrEmpty(contenttype))
                    {
                        contenttype = "application/octet-stream";
                    }
                    response.ContentType = contenttype;

                    response.BinaryBytes = sourceprovider.GetBinary(context, relativeurl);

                    break;

                case UrlFileType.Html:

                    return RenderHtml(context, option, relativeurl);

                default:
                    break;
            }

            return response;
        }

        public static RenderRespnose RenderImage(RenderContext context, SpaRenderOption option, string relativeurl)
        {
            RenderRespnose response = new RenderRespnose {ContentType = "image"};


            string extension = Kooboo.Lib.Helper.UrlHelper.FileExtension(relativeurl);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                if (!string.IsNullOrEmpty(extension) && extension.StartsWith("."))
                {
                    extension = extension.Substring(1);
                }
                response.ContentType = response.ContentType + "/" + extension;

                if (extension.ToLower() == "svg")
                {
                    response.ContentType = response.ContentType + "+xml";
                }
            }

            var provider = GetSourceProvider(context, option);

            if (provider != null)
            {
                response.BinaryBytes = provider.GetBinary(context, relativeurl);
            }
            return response;
        }

        public static RenderRespnose RenderHtml(RenderContext context, SpaRenderOption option, string relativeurl)
        {
            var sourceprovider = GetSourceProvider(context, option);

            RenderRespnose response = new RenderRespnose {ContentType = "text/html"};
            string minetype = IOHelper.MimeType(relativeurl);
            if (!string.IsNullOrEmpty(minetype))
            {
                response.ContentType = minetype;
            }

            if (context == null || sourceprovider == null)
            {
                return response;
            }
            string htmlbody = sourceprovider.GetString(context, relativeurl);

            if (string.IsNullOrEmpty(htmlbody))
            {
                return response;
            }

            var hashid = Lib.Security.Hash.ComputeHashGuid(htmlbody);

            var evaluatorOption = new EvaluatorOption
            {
                IgnoreEvaluators = EnumEvaluator.Form | EnumEvaluator.LayoutCommand,
                Evaluators = Kooboo.Render.Components.EvaluatorContainer.ListWithServerComponent
            };

            var renderPlan = RenderPlanCache.GetOrAddRenderPlan(hashid, () => RenderEvaluator.Evaluate(htmlbody, evaluatorOption));

            string result = Sites.Render.RenderHelper.Render(renderPlan, context);

            string finalreseult = null;

            response.Body = !string.IsNullOrEmpty(finalreseult) ? finalreseult : result;
            return response;
        }
    }
}
//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Render.ObjectSource;
using Kooboo.Sites.Render;
using Kooboo.Lib;
using Kooboo.Lib.Helper;
using System;

namespace Kooboo.Render
{
    public class RenderEngine
    {
        private static Sites.Render.Commands.ICommandSourceProvider GetSourceProvider(RenderContext context, RenderOption option)
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

        public static RenderRespnose Render(RenderContext Context, RenderOption option, string relativeurl)
        {
            var sourceprovider = GetSourceProvider(Context, option);

            var FileType = RenderHelper.GetFileType(relativeurl);
            RenderRespnose response = new RenderRespnose();

            switch (FileType)
            {
                case UrlFileType.Image:

                    response = RenderImage(Context, option, relativeurl);
                     
                    break; 

                case UrlFileType.JavaScript:

                    if (sourceprovider is CommandDiskSourceProvider)
                    {
                        response = ServerSide.ServerEngine.RenderJs(sourceprovider as CommandDiskSourceProvider, option, Context, relativeurl); 
                    }
                    else
                    {
                        response.ContentType = "application/javascript";
                        response.BinaryBytes = sourceprovider.GetBinary(Context, relativeurl);
                    }

                    break;

                case UrlFileType.Style:

                    response.ContentType = "text/css";
                    response.BinaryBytes = sourceprovider.GetBinary(Context, relativeurl);

                    break;
                case UrlFileType.File:

                    string contenttype = IOHelper.MimeType(relativeurl);
                    if (string.IsNullOrEmpty(contenttype))
                    {
                        contenttype = "application/octet-stream";
                    }
                    response.ContentType = contenttype;

                    if (contenttype.ToLower().Contains("zip"))
                    {
                        var stream = sourceprovider.GetStream(Context, relativeurl);

                        if (stream != null)
                        {
                            response.Stream = stream;
                        }
                        else
                        {
                            response.BinaryBytes = sourceprovider.GetBinary(Context, relativeurl);
                        }
                    }
                    else
                    {
                        response.BinaryBytes = sourceprovider.GetBinary(Context, relativeurl);

                        if (contenttype.ToLower().Contains("font"))
                        { 
                            Context.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r");
                        }
                    }
                     
                    break;
                case UrlFileType.Html:

                    return RenderHtml(Context, option, relativeurl);
                default:
                    break;
            }

            var version = Context.Request.Get("version"); 
            if(!string.IsNullOrEmpty(version))
            {
                if (Lib.Helper.CharHelper.isAsciiDigit(version))
                {
                    Context.Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString("r");
                }
            } 
            return response;
        }

        public static RenderRespnose RenderImage(RenderContext Context, RenderOption option, string relativeurl)
        {
            RenderRespnose response = new RenderRespnose();

            response.ContentType = "image";

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

                if (extension.ToLower() == "ico")
                {
                    // favorite icon. 
                   Context.Response.Headers["Expires"] = DateTime.UtcNow.AddDays(7).ToString("r");
                }

            }

            var provider = GetSourceProvider(Context, option);

            if (provider != null)
            {
                response.BinaryBytes = provider.GetBinary(Context, relativeurl);
            }
            return response;
        }

        public static RenderRespnose RenderHtml(RenderContext Context, RenderOption option, string relativeurl)
        {
            var sourceprovider = GetSourceProvider(Context, option);

            RenderRespnose response = new RenderRespnose();
            response.ContentType = "text/html";
            string minetype = IOHelper.MimeType(relativeurl);
            if (!string.IsNullOrEmpty(minetype))
            {
                response.ContentType = minetype;
            }

            if (Context == null || sourceprovider == null)
            {
                return response;
            }
            string htmlbody = sourceprovider.GetString(Context, relativeurl);

            if (string.IsNullOrEmpty(htmlbody))
            {
                return response;
            }

            var hashid = Lib.Security.Hash.ComputeHashGuid(htmlbody);

            var EvaluatorOption = new EvaluatorOption();
            EvaluatorOption.EnableImageBrowserCache = true;
            EvaluatorOption.EnableJsCssBrowserCache = true; 
             
            EvaluatorOption.Evaluators = Kooboo.Render.Components.EvaluatorContainer.ListWithServerComponent; 
             
            var RenderPlan = RenderPlanCache.GetOrAddRenderPlan(hashid, () => RenderEvaluator.Evaluate(htmlbody, EvaluatorOption));

            // set the culture...
            string culture = Context.Culture; 
            if (string.IsNullOrEmpty(culture))
            {
                Context.DataContext.Push("culture", culture); 
            }

            string result = Kooboo.Sites.Render.RenderHelper.Render(RenderPlan, Context);

            string finalreseult = null;

            if (!string.IsNullOrEmpty(finalreseult))
            {
                response.Body = finalreseult;
            }
            else
            {
                response.Body = result;
            }

            return response;
        }
         
         
    }
}

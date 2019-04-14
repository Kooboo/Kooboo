using System;
using System.Collections.Generic;
using System.Linq;

using Kooboo.Dom;

namespace Kooboo.Model.Render.Parsers
{
    public class LoadParser : IVirtualElementParser
    {
        public string Name => "load";

        public int Priority => ParserPriority.Low;

        public void Parse(Element el, TagParseContext context, Action visitChildren)
        {
            // Get url
            context.TryGet<string>(SubmitParser.Data_Url, out string url);
            url = url ?? el.getAttribute(context.Options.GetAttributeName(Name));

            // Get modelName
            var modelGenerated = context.TryGet<string>(SubmitParser.Data_ModelName, out string modelName);
            modelName = modelName ?? el.getAttribute(context.Options.GetAttributeName(SubmitParser.ModelAttribute));
            if (String.IsNullOrEmpty(modelName))
            {
                modelName = ParserHelper.GetModelNameFromUrl(url);
            }

            // Get meta
            var meta = context.Options.ApiMetaProvider.GetMeta(url);

            // data model
            if (!modelGenerated)
            {
                context.Js.Data(modelName, ParserHelper.GetJsonFromModel(meta.Result));
                // sub view props
                if (context.ViewContext.ViewType == ViewType.Sub)
                {
                    context.Js.Data(LogicKeywords.PropsData, null);
                }
            }

            // load js
            var urlWithParams = ParserHelper.GenerateUrlFromApiParameters(url, meta.Parameters);
            context.Js.Load(urlWithParams, modelName);

            visitChildren?.Invoke();
        }
    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render
{
    public static class ModelBinder
    {
        public static string Bind(object model, string html, string modelName = "model")
        {
            if (model == null)
            {
                return html;
            }
            RenderContext context = new RenderContext();
            context.DataContext.Push(modelName, model);

            var plans = RenderEvaluator.Evaluate(html, new EvaluatorOption());
            return plans.Render(context);
        }

        public static string Bind(string html, RenderContext context)
        {
            var plans = RenderEvaluator.Evaluate(html, new EvaluatorOption());
            return plans.Render(context);
        }
    }
}
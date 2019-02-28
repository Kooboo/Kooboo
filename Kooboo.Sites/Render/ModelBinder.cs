//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render
{
  public static  class ModelBinder
    { 
        public static string Bind(object Model, string Html, string ModelName = "model")
        {
            if (Model == null)
            {
                return Html;
            }
            RenderContext context = new RenderContext();
            context.DataContext.Push(ModelName, Model);

            var plans = RenderEvaluator.Evaluate(Html, new EvaluatorOption());
            return  RenderHelper.Render(plans, context);
        }
         
        public static string Bind(string Html, RenderContext context)
        { 
            var plans = RenderEvaluator.Evaluate(Html, new EvaluatorOption());
            return RenderHelper.Render(plans, context);
        }

    }
}

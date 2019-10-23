using Kooboo.Sites.Render;
using Kooboo.Sites.Render.Evaluators;
using System.Collections.Generic;

namespace Kooboo.Render.Components
{
    public static class EvaluatorContainer
    {
        private static List<IEvaluator> _list;

        public static List<IEvaluator> ListWithServerComponent
        {
            get
            {
                return _list ?? (_list = new List<IEvaluator>
                {
                    new PlaceHolderEvaluator(),
                    new SiteLayoutEvaluator(),
                    new RepeaterEvaluator(),
                    new ConditionEvaluator(),
                    new ForEvaluator(),
                    new AttributeEvaluator(),
                    new LabelEvaluator(),
                    new OmitTagEvaluator(),
                    new ContentEvaluator(),
                    new ComponentEvaluator(),
                    new HeaderEvaluator(),
                    new FormEvaluator(),
                    new CommandEvaluator(),
                    new KConfigContentEvaluator(),
                    new ServerComponentEvaluator()
                });
            }
        }
    }
}
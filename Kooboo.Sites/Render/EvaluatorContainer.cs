//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.Render.Evaluators;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class EvaluatorContainer
    {
        private static List<IEvaluator> _list;

        public static List<IEvaluator> DefaultList
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
                    new kExternalCacheEvaluator(),
                    new UrlEvaluator(),
                    new LabelEvaluator(),
                    new OmitTagEvaluator(),
                    new ContentEvaluator(),
                    new ComponentEvaluator(),
                    new HeaderEvaluator(),
                    new FormEvaluator(),
                    new CommandEvaluator(),
                    new KConfigContentEvaluator()
                });
            }
        }
    }
}
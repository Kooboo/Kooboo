using Kooboo.Render.Customized;
using Kooboo.Sites.Render;
using Kooboo.Sites.Render.Evaluators;
using System.Collections.Generic;

namespace Kooboo.Render.Components
{
    public static    class EvaluatorContainer
    {

        private static List<IEvaluator> _list;
        public static List<IEvaluator> ListWithServerComponent
        {
            get
            {
                if (_list == null)
                {
                    _list = new List<IEvaluator>();
                    _list.Add(new PlaceHolderEvaluator());
                    _list.Add(new SiteLayoutEvaluator());
                    _list.Add(new RepeaterEvaluator());
                    _list.Add(new ConditionEvaluator());
                    _list.Add(new ForEvaluator());
                    _list.Add(new AttributeEvaluator());
                 // _list.Add(new UrlEvaluator());
                    _list.Add(new LabelEvaluator());
                    _list.Add(new OmitTagEvaluator());
                    _list.Add(new OmitOuterTagEvaluator());
                    _list.Add(new ContentEvaluator());
                    _list.Add(new ComponentEvaluator());
                  //_list.Add(new HeaderEvaluator());
                    _list.Add(new FormEvaluator());
                    _list.Add(new CommandEvaluator());
                    _list.Add(new KConfigContentEvaluator());

                    _list.Add(new ServerComponentEvaluator());

                    _list.Add(new AdminVersionEvaluator());
                    //add the 
                }
                return _list;
            }
        }

    }
}

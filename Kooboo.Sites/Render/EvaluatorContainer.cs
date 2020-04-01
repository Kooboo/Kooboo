//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Render.Evaluators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{
    public class EvaluatorContainer
    {
        private static List<IEvaluator> _list;
        public static List<IEvaluator> DefaultList
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
                    _list.Add(new kExternalCacheEvaluator());
                    _list.Add(new UrlEvaluator());
                    _list.Add(new LabelEvaluator());
                    _list.Add(new OmitTagEvaluator());
                    _list.Add(new OmitOuterTagEvaluator());
                    _list.Add(new ContentEvaluator());
                    _list.Add(new ComponentEvaluator());
                    _list.Add(new HeaderEvaluator());
                    _list.Add(new FormEvaluator());
                    _list.Add(new CommandEvaluator());
                    _list.Add(new KConfigContentEvaluator());
                    _list.Add(new VersionEvaluator());  
                }
                return _list;
            }
        }
    }
}
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
                    _list.Add(new LocalCacheEvaluator());
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



        private static List<IEvaluator> _mockList;

        public static List<IEvaluator> MockData
        {
            get
            {
                if (_mockList == null)
                {
                    _mockList = new List<IEvaluator>();
                    _mockList.Add(new PlaceHolderEvaluator());
                    _mockList.Add(new SiteLayoutEvaluator());
                    // _mockList.Add(new RepeaterEvaluator());
                    // _mockList.Add(new ConditionEvaluator());
                    // _mockList.Add(new ForEvaluator());
                    _mockList.Add(new AttributeEvaluator());
                    // _mockList.Add(new kExternalCacheEvaluator());
                    //_mockList.Add(new UrlEvaluator());
                    //_mockList.Add(new LocalCacheEvaluator());
                   // _mockList.Add(new LabelEvaluator());
                    _mockList.Add(new OmitTagEvaluator());
                    _mockList.Add(new OmitOuterTagEvaluator());
                    //_mockList.Add(new ContentEvaluator());
                    _mockList.Add(new ComponentEvaluator());
                    //_mockList.Add(new HeaderEvaluator());
                    _mockList.Add(new FormEvaluator());
                    _mockList.Add(new CommandEvaluator());
                    //_mockList.Add(new KConfigContentEvaluator());
                    _mockList.Add(new VersionEvaluator());
                }
                return _mockList;
            }
        }
    }
}
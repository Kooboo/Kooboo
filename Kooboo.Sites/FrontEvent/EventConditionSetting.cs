//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.FrontEvent
{
    public class EventConditionSetting : Kooboo.Data.Models.SimpleSetting
    {
        private List<string> _operator;

        public List<string> Operator
        {
            get
            {
                if ((_operator == null || !_operator.Any()) && this.DataType != null)
                {
                    _operator = FrontEvent.ConditionManager.GetOperators(this.DataType);
                }
                return _operator;
            }
            set
            { _operator = value; }
        }
    }
}
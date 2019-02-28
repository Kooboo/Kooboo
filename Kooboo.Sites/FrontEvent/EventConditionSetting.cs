//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;
  
namespace Kooboo.Sites.FrontEvent
{

public  class EventConditionSetting :Kooboo.Data.Models.SimpleSetting
    {   
        private List<string> _Operator; 

        public List<string> Operator
        {
            get
            {
                if ((_Operator == null || _Operator.Count()==0) && this.DataType != null)
                {
                    _Operator = FrontEvent.ConditionManager.GetOperators(this.DataType);
                }
                return _Operator; 
            } 
            set
            { _Operator = value;      }
        }

    }
     
}

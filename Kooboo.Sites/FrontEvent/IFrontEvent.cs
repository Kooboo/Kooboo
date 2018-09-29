//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic;


namespace Kooboo.Sites.FrontEvent
{
   public interface IFrontEvent
    {
        enumEventType EventType { get;  }

        List<EventConditionSetting> GetConditionSetting(RenderContext context);
  
        RenderContext Context { get; set; } 

        bool DataChange { get; set; }
    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{
   public class EvaluatorOption
    {
        public bool RenderHeader { get; set; }

        public bool RenderUrl { get; set; }

        public bool RequireBindingInfo { get; set;} 

        public Guid OwnerObjectId { get; set; } 

        public EnumEvaluator IgnoreEvaluators { get; set; }
        

        public  EvaluatorOption Clone()
        {
            EvaluatorOption newoption = new EvaluatorOption();
            newoption.RenderHeader = this.RenderHeader;
            newoption.RenderUrl = this.RenderUrl;
            newoption.RequireBindingInfo = this.RequireBindingInfo;
            newoption.OwnerObjectId = this.OwnerObjectId; 
            return newoption; 
        }
          
        public List<IEvaluator> Evaluators { get; set; }

    }


    public enum EnumEvaluator
    {
        Attribute = 1,
        LayoutCommand = 2, 
        Component = 4,
        Condition = 5,
        Content = 6,
        For = 7,
        Form = 8,
        Header = 9,
        Label = 10,
        OmitTag = 11,
        PlaceHolder = 12,
        Repeater = 13,
        SiteLayout = 14,
        Url = 15,
        KFrom = 16, 
        kConfig = 17,
        kCache = 18,
        OmitOuterTag = 19,
        Version = 20
    }

}

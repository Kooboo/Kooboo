//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Automation.Ranking
{
 public static   class BuiltInRankingRules
    {

     private static List<RankingRule> _layoutrules; 

     public static List<RankingRule> LayoutRules()
     {
         if (_layoutrules == null)
         {
             _layoutrules = new List<RankingRule>(); 

             _layoutrules.Add(new RankingRule()
             {
                 SiteObjectType = ConstObjectType.Layout,
                 NotationName = "CssWidth",
                 CompareType = Ranking.CompareType.GreaterThan,
                 CompareValue = 100,
                 AddedValue = 20
             });

             _layoutrules.Add(new RankingRule()
             {
                 SiteObjectType = ConstObjectType.Layout,
                 NotationName = "HasBackground",
                 CompareType = Ranking.CompareType.Equal,
                 CompareValue = true,
                 AddedValue = 5
             });

             _layoutrules.Add(new RankingRule()
             {
                 SiteObjectType = ConstObjectType.Layout,
                 NotationName = "HasBorder",
                 CompareType = Ranking.CompareType.Equal,
                 CompareValue = true,
                 AddedValue = 5
             });

             _layoutrules.Add(new RankingRule()
             {
                 SiteObjectType = ConstObjectType.Layout,
                 NotationName = "HasMargin",
                 CompareType = Ranking.CompareType.Equal,
                 CompareValue = true,
                 AddedValue = 5
             });

             _layoutrules.Add(new RankingRule()
             {
                 SiteObjectType = ConstObjectType.Layout,
                 NotationName = "HasMinMaxWidth",
                 CompareType = Ranking.CompareType.Equal,
                 CompareValue = true,
                 AddedValue = 30
             });

             _layoutrules.Add(new RankingRule()
             {
                 SiteObjectType = ConstObjectType.Layout,
                 NotationName = "SementicName",
                 CompareType = Ranking.CompareType.Equal,
                 CompareValue = true,
                 AddedValue = 15
             });
         }

         return _layoutrules;

     }


    }
}

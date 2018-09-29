//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.NodeTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Dom; 

namespace Kooboo.Sites.Automation.Ranking
{
  public static  class RankingRuleEvaluaor
    {

      /// <summary>
      /// calculation the notation value and apply the score to site node. 
      /// </summary>
      /// <param name="node"></param>
      /// <param name="rules"></param>
      public static void ApplyScore(SiteNode node, List<RankingRule> rules)
      {
          if (node.DomNode == null || node.DomNode.nodeType != Dom.enumNodeType.ELEMENT)
          {
              return;
          }

          Element element = node.DomNode as Element; 

          foreach (var item in rules)
          {
              var notation = Automation.Notation.NotationContainer.getNataitonByName(item.NotationName);

              int score = getElementScore(element, notation, item);

              node.Score += score;
          }

      }

      public static int getElementScore(Dom.Element element, Automation.Notation.INotation notation, RankingRule rule)
      {
          if (notation != null)
          {
              object value = notation.Execute(element);
              return getScore(notation.ReturnType, value, rule);
          }

          return 0;
      }

      public static int getScore(Type valueType, object Value, RankingRule rule)
      {
          if (valueType == typeof(bool))
          {
              return getBoolScore(Value, rule);
          }
          else if (valueType == typeof(int))
          {
              return GetIntScore(Value, rule); 
          }
          else if (valueType == typeof(string))
          {
              return getStringScore(Value, rule); 
          }
          return 0; 
      }

      private static int getBoolScore(object objectValue, RankingRule rule)
      {
          bool boolvalue = (bool)objectValue;

          bool comparevalue = (bool)rule.CompareValue;

          if (boolvalue == comparevalue)
          {
              return rule.AddedValue;
          }
          else
          {
              return 0; 
          }
      }

      private static int getStringScore(object objectValue, RankingRule rule)
      {

          string value = objectValue.ToString();

          string comparevalue = rule.CompareValue.ToString();

          bool isMatch = false; 

          switch (rule.CompareType)
          {
              case CompareType.Equal:
                  isMatch = (string.Compare(value, comparevalue, true) == 0);
                  break;

              case CompareType.GreaterThan:
                  // string can not have greaterthan.
                  isMatch = (string.Compare(value, comparevalue, true) > 0);
                  break;
              case CompareType.GreaterThanOrEqual:
                  isMatch = (string.Compare(value, comparevalue, true) >= 0);
                  break;
              case CompareType.LessThan:
                  isMatch = (string.Compare(value, comparevalue, true) < 0);
                  break;
              case CompareType.LessThanOrEqual:
                  isMatch = (string.Compare(value, comparevalue, true) <= 0);
                  break;
              case CompareType.Contains:
                  if (value.Contains(comparevalue))
                  {
                      isMatch = true;
                  }
                  break;
              default:
                  break;
          }

          if (isMatch)
          {
              return rule.AddedValue;
          }
          else
          {
              return 0; 
          }

      }

      private static int GetIntScore(object objectValue, RankingRule rule)
      {

          int value = (int)objectValue;
          int comparevalue = (int)rule.CompareValue;


          bool isMatch = false;

          switch (rule.CompareType)
          {
              case CompareType.Equal:
                  isMatch = (value==comparevalue);
                  break;

              case CompareType.GreaterThan:
                  // string can not have greaterthan.
                  isMatch = (value>comparevalue);
                  break;
              case CompareType.GreaterThanOrEqual:
                  isMatch = (value>=comparevalue);
                  break;
              case CompareType.LessThan:
                  isMatch = (value < comparevalue);
                  break;
              case CompareType.LessThanOrEqual:
                  isMatch = (value <= comparevalue);
                  break;
              case CompareType.Contains:
                  
                  break;
              default:
                  break;
          }

          if (isMatch)
          {
              return rule.AddedValue;
          }
          else
          {
              return 0;
          }


      }

    }
}

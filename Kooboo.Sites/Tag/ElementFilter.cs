//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom; 

namespace Kooboo.Sites.Tag
{

    /// <summary>
    /// used to filter out nodes that can not be used for one or more kind of automation analyzers. 
    /// </summary>
  public static  class ElementFilter
    {

      public static Func<Node, bool> GetFilter(Byte SiteObjectType)
      {
          switch (SiteObjectType)
          {
              case ConstObjectType.Layout:
                 return LayoutFilter;

              case ConstObjectType.Menu:
                  break;
              case ConstObjectType.View:
                  break;
              default:
                  break;
          }

          return null; 
      }

      /// <summary>
      /// Select the nodes that might be used as layout. 
      /// </summary>
      /// <param name="node"></param>
      /// <returns></returns>
      public static bool LayoutFilter(Node node)
      {
          if (node.nodeType == enumNodeType.ELEMENT)
          {
              // for element type, meta and text tag are not allowed, the rest is accept. 

              Element e = node as Element;
              if (e.tagName == "body" || e.tagName == "html")
              {
                  return true;
              }

              if (Tag.ContentModel.MetaList.Contains(e.tagName))
              {
                  return false; 
              }

              if (Tag.ContentModel.TextList.Contains(e.tagName))
              {
                  return false; 
              }

              return true; 
          }
          //else if (node.nodeType == enumNodeType.TEXT)
          //{
          //    return !StringContainsSpeceOnly(node.textContent);
          //}

          return false; 
      }

      /// <summary>
      /// return element and non-null text node. 
      /// </summary>
      /// <param name="node"></param>
      /// <returns></returns>
      public static bool ElementNTextNode(Node node)
      {
          if (node.nodeType == enumNodeType.ELEMENT)
          {
              return true;
          }

          if (node.nodeType == enumNodeType.TEXT)
          {

              return !StringContainsSpeceOnly(node.textContent);
              
          }

          return false; 
      }

      private static bool StringContainsSpeceOnly(string input)
      {
          int len = input.Length;

          for (int i = 0; i < len; i++)
          {
              char ichar = input[i];

              if (ichar == '\u0020' || ichar == '\u0009' || ichar == '\u000a' || ichar == '\u000c' || ichar == '\u000d')
              {
                  continue;
              }
              else
              {
                  return false;
              }
          }

          return true; 

      }

    }

 
}

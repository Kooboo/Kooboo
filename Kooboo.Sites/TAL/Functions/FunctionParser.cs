//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.TAL.Functions
{
 public    class FunctionParser
    {

     /// <summary>
     /// parse an expression into a function body.
     /// </summary>
     /// <param name="expression"></param>
     /// <param name="datasource"></param>
     /// <returns></returns>
     public static Function Parse(string expression)
     {
         Function function = new Function();

         int firstbracketIndex = expression.IndexOf("(");
         int nextbrackerIndex = expression.LastIndexOf(")");

         if (firstbracketIndex < 0 || nextbrackerIndex < 0)
         {
             return null;
         }

         function.FunctionName = expression.Substring(0, firstbracketIndex).ToLower();

         string parameters = expression.Substring(firstbracketIndex + 1, nextbrackerIndex - firstbracketIndex - 1);

         if (string.IsNullOrEmpty(parameters))
         {
             return function;
         }

         var paralist = StringParser.getCommaSeparatedString(parameters);

         foreach (var item in paralist)
         {
             if (string.IsNullOrEmpty(item))
             {
                 function.ParameterValues.Add(null);
                 continue;
             }
             string eachitem = item.Trim();

             if (string.IsNullOrEmpty(eachitem))
             {
                 function.ParameterValues.Add(null);
                 continue;
             }

             if (eachitem.StartsWith("\"") && eachitem.EndsWith("\""))
             {
                 eachitem = eachitem.Trim('"');
                 function.ParameterValues.Add(eachitem);
                 continue;
             }

             if (eachitem.StartsWith("'") && eachitem.EndsWith("'"))
             {
                 eachitem = eachitem.Trim('\'');
                 function.ParameterValues.Add(eachitem);
                 continue;
             }

             //this is not a string with single or double quote.
             function.ParameterValues.Add(eachitem);
         }

         return function;
     }
     
     /// <summary>
     /// whether the input is a function expression or not.
     /// </summary>
     /// <param name="input"></param>
     /// <returns></returns>
     public static bool isExpression(string input)
     {
         if (input.Contains(")") && input.Contains("("))
         {
             return true;
         }
         else
         {
             return false;
         }
     }


    }
}

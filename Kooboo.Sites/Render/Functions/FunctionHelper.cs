//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic;

namespace Kooboo.Sites.Render.Functions
{
   public static class FunctionHelper
    {
        public static bool IsFunction(string input)
        {
            input = input.Trim();  
            return (input.IndexOf(")") > 0 && input.EndsWith(")"));  
        }
         
        public static IFunction Parse(string expression)
        {
            var functionexpr = ParseFunctionExpr(expression); 
            if (functionexpr != null && !string.IsNullOrEmpty(functionexpr.Name))
            {
                var function = FunctionContainer.GetFunction(functionexpr.Name); 
                if (function != null)
                {
                    function.Parameters = ParseParamter(functionexpr.Body); 
                    return function; 
                }
                else
                {
                    if (functionexpr.Name.Contains("."))
                    {
                        int index = functionexpr.Name.LastIndexOf(".");
                        if (index > -1)
                        {
                            string objectname = functionexpr.Name.Substring(0, index);
                            string methodname = functionexpr.Name.Substring(index + 1); 
                            if (!string.IsNullOrEmpty(objectname) && !string.IsNullOrEmpty(methodname))
                            {
                                var method = FunctionContainer.GetFunction(methodname);
                                if (method != null)
                                {
                                    method.Parameters = ParseParamter(objectname); 
                                    return method;
                                }
                            }
                        }
                    }
                    else
                    {
                        var kfunc = new kScriptFunction(); 
                        kfunc.Parameters = ParseParamter(functionexpr.Body);
                        kfunc.FunctionName = functionexpr.Name;
                        return kfunc;  
                    }
                }
            } 
        
            return null;
        }

        public static bool isString(string input)
        {
            input = input.Trim();

            if (input.StartsWith("\"") && input.EndsWith("\""))
            {
                return true;
            }

            if (input.StartsWith("'") && input.EndsWith("'"))
            {
                return true;
            }

            return false;
        }


        public static List<IFunction> ParseParamter(string functionbody)
        {
            if (!string.IsNullOrEmpty(functionbody))
            {
                string[] paras = functionbody.Split(',');

                var result = new List<IFunction>(); 
                  
                foreach (var item in paras)
                {
                   if (item == null)
                    {
                        continue; 
                    }
                    string value = item.Trim(); 

                    var parafunction = Parse(value);
                    if (parafunction == null)
                    {
                        if (isString(value))
                        {
                            ValueFunction valuefunction = new ValueFunction(value);
                            result.Add(valuefunction);
                        }
                        else
                        {
                            GetValueFunction getvalue = new GetValueFunction(value);
                            result.Add(getvalue);
                        }
                    }
                    else
                    {
                        result.Add(parafunction);
                    }
                }

                return result; 
            }
            return null; 
        }

        public static FunctionExpression ParseFunctionExpr(string input)
        {
            int index = input.IndexOf("(");
            if (index == -1)
            {
                return null; 
            }

            string functionname = input.Substring(0, index);
            string functionbody = input.Substring(index);

           if (!string.IsNullOrEmpty(functionbody))
            {
                int begine = functionbody.IndexOf("(");
                int end = functionbody.LastIndexOf(")"); 
                if (begine >-1 && end > begine)
                {
                    functionbody = functionbody.Substring(begine + 1, end - begine-1); 
                }
                else
                {
                    return null; 
                } 
            }
             
            if (!string.IsNullOrEmpty(functionname))
            {
                return new FunctionExpression() { Name = functionname, Body = functionbody }; 

            }
            return null; 
        }

        public static List<object> RenderParameter(RenderContext context, List<IFunction> paras)
        {
            List<object> values = new List<object>();
            if (paras != null)
            { 
                foreach (var item in paras)
                {
                    var value = item.Render(context);
                    values.Add(value);
                }
            }
            return values; 
        }
          
        public class FunctionExpression
        {
            public string Name { get; set; }

            public string Body { get; set; }
        }

          
    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Jint.Parser;

namespace Kooboo.Lib.Helper
{
    public static class JintHelper
    {
        public static string GetFuncBody(string js, int startLine, int startColumn, int endLine, int endColumn)
        {
            if (startLine > endLine || (startLine == endLine && startColumn > endColumn))
            {
                return null;
            }
            MemoryStream mo = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(js));
            StreamReader reader = new StreamReader(mo);

            string body = string.Empty;

            for (int i = 1; i <= endLine; i++)
            {
                var line = reader.ReadLine();
                if (line == null) { break; }

                if (i < startLine)
                {
                    continue;
                }
                else if (i == startLine)
                {
                    if (i == endLine)
                    {
                        body = line.Substring(startColumn, endColumn - startColumn);
                        break;
                    }
                    else
                    {
                        body += line.Substring(startColumn) + "\r\n";
                    }
                }
                else
                {
                    if (i < endLine)
                    {
                        body += line + "\r\n";
                    }
                    else
                    {
                        body += line.Substring(0, endColumn);
                        break;
                    }
                }
            }

            return body;
        }

        public static string GetFuncBody(string js, Jint.Parser.Ast.FunctionDeclaration func)
        {
            return GetFuncBody(js, func.Location.Start.Line, func.Location.Start.Column, func.Location.End.Line, func.Location.End.Column);
        }

        public static string GetFuncBody(string js, string functionName)
        {
            Jint.Parser.JavaScriptParser parser = new Jint.Parser.JavaScriptParser();

            var jsprogram = parser.Parse(js);

            if (jsprogram != null)
            {

                var func = GetFuncByName(jsprogram, functionName);
                if (func != null)
                {
                    return GetFuncBody(js, func);
                }
            }
            return null;
        }

        public static Jint.Parser.Ast.FunctionDeclaration GetFuncByName(Jint.Parser.Ast.Program program, string FuncName)
        {
            if (program == null || program.FunctionDeclarations.Count == 0)
            {
                return null;
            }

            if (string.IsNullOrEmpty(FuncName))
            {
                return null;
            }
            string lower = FuncName.ToLower();

            foreach (var item in program.FunctionDeclarations)
            {
                if (item.Id.Name.ToLower() == lower)
                {
                    return item;
                }
            }
            return null;
        }

        public static HashSet<string> ListFunctionNames(string js)
        {
            HashSet<string> result = new HashSet<string>();

            if (IsRequireJs(js))
            {
                var functions = ListRequireJsFuncs(js);

                foreach (var item in functions)
                {
                    result.Add(item.Id.Name); 
                }
            }
            else
            {
                Jint.Parser.JavaScriptParser parser = new Jint.Parser.JavaScriptParser();

                var jsprogram = parser.Parse(js);

                foreach (var item in jsprogram.FunctionDeclarations)
                {
                    result.Add(item.Id.Name);
                }
            }
            return result;
        }


        public static Dictionary<string,List<string>> ListFunctions(string js)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();  

            if (IsRequireJs(js))
            {
                var functions = ListRequireJsFuncs(js);

                foreach (var item in functions)
                {
                    string name = item.Id.Name;
                    List<string> paras = new List<string>(); 
                    if (item.Parameters !=null)
                    {
                        foreach (var p in item.Parameters)
                        {
                            paras.Add(p.Name); 
                        }
                    }

                    result[name] = paras; 
                }
            }
            else
            {
                Jint.Parser.JavaScriptParser parser = new Jint.Parser.JavaScriptParser();

                var jsprogram = parser.Parse(js);

                foreach (var item in jsprogram.FunctionDeclarations)
                {
                    string name = item.Id.Name;
                    List<string> paras = new List<string>();
                    if (item.Parameters != null)
                    {
                        foreach (var p in item.Parameters)
                        {
                            paras.Add(p.Name);
                        }
                    }

                    result[name] = paras;
                }
            }
            return result;
        }

         
        public static List<Jint.Parser.Ast.FunctionDeclaration> ListRequireJsFuncs(string requireJsBlock)
        {
            Jint.Parser.JavaScriptParser parser = new Jint.Parser.JavaScriptParser();

            var prog = parser.Parse(requireJsBlock);

            if (prog != null && prog.Body.Count() > 0)
            {

                var item = prog.Body.First();

                if (item is Jint.Parser.Ast.ExpressionStatement)
                {
                    var expres = item as Jint.Parser.Ast.ExpressionStatement;

                    if (expres.Expression is Jint.Parser.Ast.CallExpression)
                    {
                        var call = expres.Expression as Jint.Parser.Ast.CallExpression;
                        if (call != null && call.Arguments.Count() == 2)
                        {
                            var requireargu = call.Arguments[1];

                            if (requireargu != null && requireargu is Jint.Parser.Ast.FunctionExpression)
                            {
                                var requireFunc = requireargu as Jint.Parser.Ast.FunctionExpression;

                                if (requireFunc != null)
                                {
                                    return requireFunc.FunctionDeclarations.ToList();
                                }
                            }

                        }

                    }
                }
            }


            return new List<Jint.Parser.Ast.FunctionDeclaration>();
        }

        public static bool IsRequireJs(string JsBlock)
        {
            Jint.Parser.JavaScriptParser parser = new Jint.Parser.JavaScriptParser();

            var prog = parser.Parse(JsBlock);

            foreach (var item in prog.Body)
            {
                if (item is Jint.Parser.Ast.ExpressionStatement)
                {
                    var expres = item as Jint.Parser.Ast.ExpressionStatement;

                    if (expres.Expression is Jint.Parser.Ast.CallExpression)
                    {
                        var call = expres.Expression as Jint.Parser.Ast.CallExpression;

                        if (call.Callee is Jint.Parser.Ast.Identifier)
                        {
                            var id = call.Callee as Jint.Parser.Ast.Identifier;
                            if (id.Name.ToLower() == "require" || id.Name.ToLower() == "define")
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        // append to end of the functions. 
        public static string AppendRequireJsBlock(string Js, string append, List<Jint.Parser.Ast.FunctionDeclaration> list = null)
        {
            if (list == null)
            {
                list = ListRequireJsFuncs(Js);
            }

            if (list.Count() == 0)
            {
                return Js;
            }

            int LastLine = 0;
            int LastCol = 0;

            foreach (var item in list)
            {
                if (item.Location.End.Line > LastLine)
                {
                    LastLine = item.Location.End.Line;
                    LastCol = item.Location.End.Column;
                }
                else if (item.Location.End.Line == LastLine)
                {
                    if (item.Location.End.Column > LastCol)
                    {
                        LastCol = item.Location.End.Column;
                    }
                }
            }

            if (LastCol == 0 && LastLine == 0)
            {
                return Js;
            }

            MemoryStream mo = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Js));
            StreamReader reader = new StreamReader(mo);

            string result = string.Empty;

            var line = reader.ReadLine();
            int linenumber = 0;
            while (line != null)
            {
                linenumber += 1;
                if (linenumber != 1)
                {
                    result += "\r\n";
                }

                if (linenumber == LastLine)
                {
                    string before = line.Substring(0, LastCol);
                    result += before;
                    result += append;
                    if (line.Length > LastCol)
                    {
                        string after = line.Substring(LastCol);
                        result += after;
                    }
                }
                else
                {
                    result += line;
                }

                line = reader.ReadLine();
            }

            return result;
        }

        private static object GetValue(Jint.Runtime.Debugger.DebugInformation info, string property)
        { 
            var value = info.Locals.Where(item => item.Key.ToLower() == property).Select(item => item.Value).FirstOrDefault();
            if (value == null)
            {
                value = info.Globals.Where(item => item.Key.ToLower() == property).Select(item => item.Value).FirstOrDefault();
            }
            return value;
        }
        public static object GetGebuggerValue(Jint.Engine engine, string FullProperty)
        {
            if (string.IsNullOrEmpty(FullProperty))
            {
                return null;
            }
               
            FullProperty = FullProperty.Trim();
            FullProperty = FullProperty.TrimEnd(';'); 

            string[] parts = FullProperty.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var info = engine.DebugHandler.GetDebugInformation();

            object value = null;

            for (int i = 0; i < parts.Length; i++)
            {
                if (i == 0)
                {
                    value = GetValue(info,parts[i]);
                    if (value == null)
                    {
                        return null;
                    }
                }
                else
                {
                    value = getMember(value, parts[i]);
                    if (value == null)
                    {
                        return null;
                    }
                }
            }


            if (value is Jint.Native.JsValue)
            {
                var jsvalue = value as Jint.Native.JsValue;
                if (jsvalue != null)
                {
                    return jsvalue.ToObject();
                }
            }

            return value;


        }


        private static object getMember(object obj, string PropertyName)
        {
            if (obj is IDictionary)
            {
                var dict = obj as IDictionary;
                return GetValueDic(dict, PropertyName);
            }

            else if (obj is JObject)
            {
                return Lib.Helper.JsonHelper.GetObject(obj as JObject, PropertyName);
            }

            else if (obj is System.Dynamic.ExpandoObject)
            {
                IDictionary<String, Object> value = obj as IDictionary<String, Object>;
                if (value != null)
                {
                    return value.Where(item => EqualsIgnoreCasing(PropertyName, item.Key)).Select(item => item.Value).FirstOrDefault();
                }
                return null;
            }
            else if (obj is IDictionary<string, object>)
            {
                IDictionary<string, object> value = obj as IDictionary<string, object>;
                if (value != null)
                {
                    return value.Where(item => EqualsIgnoreCasing(PropertyName, item.Key)).Select(item => item.Value).FirstOrDefault();
                }
                return null;
            }
            else if (obj is Jint.Native.JsValue)
            {
                var value = obj as Jint.Native.JsValue;

                var jsObject = value.ToObject();
                if (jsObject ==null)
                {
                    return null; 
                }

                IDictionary<String, Object> rightvalue = jsObject as IDictionary<String, Object>;
                if (rightvalue != null)
                {
                    return rightvalue.Where(item => EqualsIgnoreCasing(PropertyName, item.Key)).Select(item => item.Value).FirstOrDefault();
                }
                else
                {
                    if (jsObject is System.Dynamic.ExpandoObject)
                    {
                        IDictionary<String, Object> expvalue = obj as IDictionary<String, Object>;
                        if (expvalue != null)
                        {
                            return expvalue.Where(item => EqualsIgnoreCasing(PropertyName, item.Key)).Select(item => item.Value).FirstOrDefault();
                        }
                        return null;
                    }
                    else
                    {
                        return Kooboo.Lib.Reflection.Dynamic.GetObjectMember(jsObject, PropertyName);
                    }
                }

            }

            return Kooboo.Lib.Reflection.Dynamic.GetObjectMember(obj, PropertyName);
        }

        private static object GetValueDic(IDictionary dictionary,string name)
        {
            var keys = dictionary.Keys;
            string matchKey=null;
            foreach(var key in keys)
            {
                if (EqualsIgnoreCasing(name, key as string))
                {
                    matchKey = key as string;
                    break;
                }
            }
            if(!string.IsNullOrEmpty(matchKey))
            {
                return dictionary[matchKey];
            }
            return null;
        }

        private static bool EqualsIgnoreCasing(string s1, string s2)
        {
            bool equals = false;
            if (s1.Length == s2.Length)
            {
                if (s1.Length > 0 && s2.Length > 0)
                {
                    equals = (s1.ToLower()[0] == s2.ToLower()[0]);
                }
                if (s1.Length > 1 && s2.Length > 1)
                {
                    equals = equals && (s1.Substring(1) == s2.Substring(1));
                }
            }
            return equals;
        }


        public static bool IsMemberExpression(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            if (code.Contains("\n") || code.Contains("=") || code.Contains("{") || code.Contains("("))
            {
                return false;
            }

            var parser = new JavaScriptParser();
            var prog = parser.Parse(code);

            if (prog == null || prog.Body == null || prog.Body.Count() != 1)
            {
                return false;
            }

            var statement = prog.Body.First(); 

            if (statement == null)
            {
                return false; 
            } 
            var exp = statement as Jint.Parser.Ast.ExpressionStatement; 

            if (exp == null)
            {
                return false; 
            }
              
            var t = exp.Expression.GetType();

            if (t == typeof(Jint.Parser.Ast.MemberExpression) || t== typeof(Jint.Parser.Ast.Identifier))
            {
                return true; 
            } 
            return false;  
        }

        public static bool IsAssignmentExpression(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            if (code.Contains("{") || code.Contains("("))
            {
                return false;
            }

            var parser = new JavaScriptParser();
            var prog = parser.Parse(code);

            if (prog == null || prog.Body == null || prog.Body.Count() != 1)
            {
                return false;
            }

            var statement = prog.Body.First();

            if (statement == null)
            {
                return false;
            }
            var exp = statement as Jint.Parser.Ast.ExpressionStatement;

            if (exp == null)
            {
                return false;
            }
              
            var t = exp.Expression.GetType();

            if (t == typeof(Jint.Parser.Ast.AssignmentExpression))
            {
                return true;
            }
            return false;
        }

        public static object GetAssignmentValue(string code)
        { 
            var parser = new JavaScriptParser();
            var prog = parser.Parse(code);
             
            var statement = prog.Body.First(); 
  
            var exp = statement as Jint.Parser.Ast.ExpressionStatement;

            var ass = exp.Expression as Jint.Parser.Ast.AssignmentExpression;

            var rightvalue = ass.Right as Jint.Parser.Ast.Literal; 

            if (rightvalue !=null)
            {
                return rightvalue.Value; 
            }

            return null; 
        }

    }
}

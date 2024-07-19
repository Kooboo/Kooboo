//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Esprima;
using Esprima.Ast;

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

        public static string GetFuncBody(string js, Esprima.Ast.FunctionDeclaration func)
        {
            return GetFuncBody(js, func.Location.Start.Line, func.Location.Start.Column, func.Location.End.Line, func.Location.End.Column);
        }

        public static FunctionDeclaration[] GetFunctionDeclarations(Node script)
        {
            var nodes = script?.ChildNodes;

            if (script?.Type == Nodes.FunctionExpression)
            {
                nodes = script.As<FunctionExpression>().Body.ChildNodes;
            }

            return nodes?.Where(w => w?.Type == Nodes.FunctionDeclaration)?.Select(s => s.As<FunctionDeclaration>())?.ToArray();
        }

        public static Esprima.Ast.FunctionDeclaration GetFuncByName(Script program, string FuncName)
        {
            var functionDeclarations = GetFunctionDeclarations(program);

            if (program == null || functionDeclarations.Count() == 0)
            {
                return null;
            }

            if (string.IsNullOrEmpty(FuncName))
            {
                return null;
            }
            string lower = FuncName.ToLower();

            foreach (var item in functionDeclarations)
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
                var jsprogram = new JavaScriptParser().ParseScript(js);
                var functionDeclarations = GetFunctionDeclarations(jsprogram);

                foreach (var item in functionDeclarations)
                {
                    result.Add(item.Id.Name);
                }
            }
            return result;
        }

        public static Identifier[] GetParameters(IFunction function)
        {
            return function.Params.Where(w => w.Type == Nodes.Identifier)?.Select(s => (s as Identifier))?.ToArray();
        }

        public static Dictionary<string, List<string>> ListFunctions(string js)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            if (IsRequireJs(js))
            {
                var functions = ListRequireJsFuncs(js);

                foreach (var item in functions)
                {
                    string name = item.Id.Name;
                    List<string> paras = new();
                    var parameters = GetParameters(item);
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            paras.Add(p.Name);
                        }
                    }

                    result[name] = paras;
                }
            }
            else
            {
                var jsprogram = new Esprima.JavaScriptParser().ParseScript(js);
                var functionDeclarations = GetFunctionDeclarations(jsprogram);

                foreach (var item in functionDeclarations)
                {
                    string name = item.Id.Name;
                    List<string> paras = new List<string>();
                    var parameters = GetParameters(item);
                    if (parameters != null)
                    {
                        foreach (var p in parameters)
                        {
                            paras.Add(p.Name);
                        }
                    }

                    result[name] = paras;
                }
            }
            return result;
        }

        public static List<Esprima.Ast.FunctionDeclaration> ListRequireJsFuncs(string requireJsBlock)
        {
            var prog = new Esprima.JavaScriptParser().ParseScript(requireJsBlock);

            if (prog != null && prog.Body.Count() > 0)
            {

                var item = prog.Body.First();

                if (item is Esprima.Ast.ExpressionStatement)
                {
                    var expres = item as Esprima.Ast.ExpressionStatement;

                    if (expres.Expression is Esprima.Ast.CallExpression)
                    {
                        var call = expres.Expression as Esprima.Ast.CallExpression;
                        if (call != null && call.Arguments.Count() == 2)
                        {
                            var requireargu = call.Arguments[1];

                            if (requireargu != null && requireargu is Esprima.Ast.FunctionExpression)
                            {
                                var requireFunc = requireargu as Esprima.Ast.FunctionExpression;

                                if (requireFunc != null)
                                {
                                    var functionDeclarations = GetFunctionDeclarations(requireFunc);
                                    return functionDeclarations.ToList();
                                }
                            }

                        }

                    }
                }
            }


            return new List<Esprima.Ast.FunctionDeclaration>();
        }

        public static bool IsRequireJs(string JsBlock)
        {
            var prog = new Esprima.JavaScriptParser().ParseScript(JsBlock);

            foreach (var item in prog.Body)
            {
                if (item is Esprima.Ast.ExpressionStatement)
                {
                    var expres = item as Esprima.Ast.ExpressionStatement;

                    if (expres.Expression is Esprima.Ast.CallExpression)
                    {
                        var call = expres.Expression as Esprima.Ast.CallExpression;

                        if (call.Callee is Esprima.Ast.Identifier)
                        {
                            var id = call.Callee as Esprima.Ast.Identifier;
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
        public static string AppendRequireJsBlock(string Js, string append, List<Esprima.Ast.FunctionDeclaration> list = null)
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

            var prog = new Esprima.JavaScriptParser().ParseScript(code);

            if (prog?.Body.Count != 1)
            {
                return false;
            }

            var statement = prog.Body.First();

            if (statement == null)
            {
                return false;
            }
            var exp = statement as Esprima.Ast.ExpressionStatement;

            if (exp == null)
            {
                return false;
            }

            var t = exp.Expression.GetType();

            if (t == typeof(MemberExpression) || t == typeof(Identifier) || t == typeof(StaticMemberExpression))
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

            var prog = new Esprima.JavaScriptParser().ParseScript(code);

            if (prog?.Body.Count != 1)
            {
                return false;
            }

            var statement = prog.Body.First();

            if (statement == null)
            {
                return false;
            }
            var exp = statement as Esprima.Ast.ExpressionStatement;

            if (exp == null)
            {
                return false;
            }

            var t = exp.Expression.GetType();

            if (t == typeof(Esprima.Ast.AssignmentExpression))
            {
                return true;
            }
            return false;
        }

        public static object GetAssignmentValue(string code)
        {
            var prog = new Esprima.JavaScriptParser().ParseScript(code);

            var statement = prog.Body.First();

            var exp = statement as Esprima.Ast.ExpressionStatement;

            var ass = exp.Expression as Esprima.Ast.AssignmentExpression;

            var rightvalue = ass.Right as Esprima.Ast.Literal;

            if (rightvalue != null)
            {
                return rightvalue.Value;
            }

            return null;
        }

        public static List<string> GetDependencies(string code)
        {
            if (Lib.Helper.JsonHelper.IsJson(code)) return new List<string>();
            var script = new Esprima.JavaScriptParser().ParseModule(code);
            return GetDependencies(script);
        }

        public static List<string> GetDependencies(Module script)
        {
            var result = new List<string>();
            // ImportDeclaration only allow define in top level
            foreach (var item in script.ChildNodes)
            {
                if (item is ImportDeclaration importDeclaration)
                {
                    result.Add(importDeclaration.Source.StringValue);
                }
            }
            return result;
        }

        public static Script ChangeTopLevelVariableDeclaration(string script, ParserOptions parserOptions = null, string source = null)
        {
            var parser = parserOptions == default ? new JavaScriptParser() : new JavaScriptParser(parserOptions);
            var ast = parser.ParseScript(script, source);

            // Change top level 'let','const' variableDeclaration to 'var'
            // **强制将顶级的变量定义类型改为var,因为let 和 const 是执行时瞬态,无法保留到下一次,会导致kview的绑定无法正确获取到值.
            // 此做法违反了标准执行方式,在必要时可酌情进行移除,但是会导致现有站点出现html模版取不到let和const定义的变量
            var nodes = new List<Statement>();

            foreach (var declaration in ast.Body)
            {
                if (declaration is VariableDeclaration variableDeclaration && variableDeclaration.Kind != VariableDeclarationKind.Var)
                {
                    var newVariableDeclaration = new VariableDeclaration(variableDeclaration.Declarations, VariableDeclarationKind.Var);
                    newVariableDeclaration.Location = variableDeclaration.Location;
                    newVariableDeclaration.Range = variableDeclaration.Range;
                    nodes.Add(newVariableDeclaration);
                }
                else
                {
                    nodes.Add(declaration);
                }
            }

            return new Script(NodeList.Create(nodes), ast.Strict);
        }
    }
}
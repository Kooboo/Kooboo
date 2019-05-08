//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Jint.Native;
using Jint.Runtime.Environments;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kooboo.Lib.Reflection;
using Jint.Runtime;
using Kooboo.Sites.Scripting.Global;

namespace Kooboo.Sites.Scripting
{
    public class Manager
    {
        static Manager()
        {
            Jint.JintEngineHelper.AddTypeMapper();
        }

        public static string DebuggerEngineName { get; set; } = "__kooboodebugger";

        public static k GetOrSetK(RenderContext context)
        {
            var kcontext = context.GetItem<k>();
            if (kcontext == null)
            {
                kcontext = new k(context);
                context.SetItem<k>(kcontext);
            }
            return kcontext;
        }

        public static Session GetOrSetSession(RenderContext context)
        {
            var ksession = context.GetItem<Session>();
            if (ksession == null)
            {
                ksession = new Session(context);
                context.SetItem<Session>(ksession);
            }
            return ksession;
        }


        public static Jint.Engine GetJsEngine(RenderContext context)
        {
            var item = context.GetItem<Jint.Engine>();
            if (item == null)
            {
                item = new Jint.Engine(JintSetting.SetOption);

                var kcontext = context.GetItem<k>();
                if (kcontext == null)
                {
                    kcontext = new k(context);
                    context.SetItem<k>(kcontext);
                }
                item.SetValue("k", kcontext);
                context.SetItem<Jint.Engine>(item);
            }
            return item;
        }

        public static Jint.Engine GetDebugJsEngine(RenderContext context)
        {
            var item = context.GetItem<Jint.Engine>(DebuggerEngineName);

            if (item == null)
            {
                item = new Jint.Engine(JintSetting.SetDebugOption);

                var kcontext = context.GetItem<k>();
                if (kcontext == null)
                {
                    kcontext = new k(context);
                    context.SetItem<k>(kcontext);
                }

                item.SetValue("k", kcontext);

                context.SetItem<Jint.Engine>(item, DebuggerEngineName);
            }
            return item;
        }

        public static object ExeConfig(Models.Code code, WebSite site)
        {
            RenderContext context = new RenderContext();
            context.WebSite = site;
            var engine = new Jint.Engine(JintSetting.SetOption);
            var kcontext = new k(context);
            engine.SetValue("k", kcontext);

            var k = engine.GetValue("k");

            kcontext.ReturnValues.Clear();
            try
            {
                engine.Execute(code.Config);
            }
            catch (System.Exception ex)
            {
            }

            if (kcontext.ReturnValues.Count() > 0)
            {
                return kcontext.ReturnValues.Last();
            }
            return null;
        }

        public static List<Data.Models.SimpleSetting> GetSetting(WebSite website, Models.Code code)
        {
            var config = Kooboo.Sites.Scripting.Manager.ExeConfig(code, website);

            List<Data.Models.SimpleSetting> settings = new List<SimpleSetting>();

            if (config != null)
            {
                var items = ((IEnumerable)config).Cast<object>().ToList();

                if (items != null && items.Count() > 0)
                {
                    foreach (var item in items)
                    {
                        SimpleSetting setting = new SimpleSetting();

                        var dict = item as IDictionary<string, object>;

                        object SelectionValues = null;

                        foreach (var keyvalue in dict)
                        {
                            var lowerkey = keyvalue.Key.ToLower();

                            if (lowerkey == "name")
                            {
                                setting.Name = keyvalue.Value.ToString();
                            }
                            else if (lowerkey == "tooltip")
                            {
                                setting.ToolTip = keyvalue.Value.ToString(); 
                            }
                            else if (lowerkey == "controltype")
                            {
                                setting.ControlType = GetControlType(keyvalue.Value.ToString());
                            }
                            else if (lowerkey == "value" || lowerkey == "defaultvalue")
                            {
                                setting.DefaultValue = keyvalue.Value;
                            }
                            else if (lowerkey == "selectionvalues" || lowerkey == "selectionvalues")
                            {
                                SelectionValues = keyvalue.Value;
                            }
                        }

                        if (setting.ControlType == Data.ControlType.Selection && SelectionValues != null)
                        {

                            var selections = SelectionValues as IDictionary<string, object>;

                            if (selections != null)
                            {
                                if (setting.SelectionValues == null)
                                {
                                    setting.SelectionValues = new Dictionary<string, string>();
                                }

                                foreach (var se in selections)
                                {
                                    setting.SelectionValues.Add(se.Key, se.Value.ToString());
                                }
                            }

                        }

                        settings.Add(setting);
                    }
                }
            }

            return settings;
        }

        private static Kooboo.Data.ControlType GetControlType(string input)
        {
            if (input == null)
            {
                return Data.ControlType.TextBox;
            }
            var lower = input.ToLower();
            if (lower == "textarea")
            {
                return Data.ControlType.TextArea;
            }
            else if (lower == "textbox")
            {
                return Data.ControlType.TextBox;
            }
            else if (lower == "selection" || lower == "selections")
            {
                return Data.ControlType.Selection;
            }
            else if (lower == "checkbox")
            {
                return Data.ControlType.CheckBox;
            }

            return Data.ControlType.TextBox;
        }

        public static string ExecuteCode(RenderContext context, string JsCode, Guid CodeId = default(Guid))
        {
            if (string.IsNullOrEmpty(JsCode))
            {
                return null;
            }


            Jint.Engine engine = null;

            var debugsession = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(context, CodeId);

            if (debugsession == null)
            {
                engine = Kooboo.Sites.Scripting.Manager.GetJsEngine(context);
            }
            else
            {
                engine = Kooboo.Sites.Scripting.Manager.GetDebugJsEngine(context);

                foreach (var item in debugsession.BreakLines)
                {
                    engine.BreakPoints.Add(new Jint.Runtime.Debugger.BreakPoint(item, 0));
                }

                debugsession.JsEngine = engine;

                engine.Break += (s, e) => Engine_Break(s, e, debugsession);

                engine.Step += (s, e) => Engine_Step(s, e, debugsession);

            }
            try
            {
                engine.Execute(JsCode);
               
            }
            catch (Exception ex)
            {
                if (debugsession != null)
                {
                    var info = new ScriptDebugger.DebugInfo
                    {
                        Start = new Jint.Parser.Position() { Line = -1 },
                        End = new Jint.Parser.Position() { Line = -1 },
                        EndOfExe = true,
                        HasValue = true,
                        IsException = true
                    };

                    if (ex is JavaScriptException)
                    {
                        var jsex = ex as JavaScriptException;
                        info.Start.Line = jsex.LineNumber;
                        info.End.Line = jsex.LineNumber;

                        info.Message = "Line " + jsex.LineNumber.ToString() + " " + jsex.Error.ToString() + " " + jsex.Message + " " + jsex.Source;
                    }
                    else
                    {
                        info.Message = ex.Message + ex.StackTrace;
                    }

                    debugsession.DebugInfo = info;
                }

                return ex.Message + " " + ex.Source;

            }

            if (debugsession != null)
            {
                if (debugsession.DebugInfo != null)
                {
                    debugsession.DebugInfo.EndOfExe = true;
                }
                else
                {
                    var info = new ScriptDebugger.DebugInfo
                    {
                        Start = new Jint.Parser.Position() { Line = -1 },
                        End = new Jint.Parser.Position() { Line = -1 },
                        EndOfExe = true,
                        HasValue = true
                    };
                    debugsession.DebugInfo = info;
                }
            }

            string output = context.GetItem<string>(Constants.OutputName);
            if (!string.IsNullOrWhiteSpace(output))
            {
                context.SetItem<string>(null, Constants.OutputName);
            }
            return output;
        }
         

        public static string ExecuteInnerScript(RenderContext context, string InnerJsCode)
        {
            if (string.IsNullOrEmpty(InnerJsCode))
            {
                return null;
            }

            Jint.Engine engine = null;

            var debugsession = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(context, InnerJsCode);

            if (debugsession == null)
            {
                engine = Kooboo.Sites.Scripting.Manager.GetJsEngine(context);
            }
            else
            {
                engine = Kooboo.Sites.Scripting.Manager.GetDebugJsEngine(context);

                foreach (var item in debugsession.BreakLines)
                {
                    engine.BreakPoints.Add(new Jint.Runtime.Debugger.BreakPoint(item, 0));
                }

                debugsession.JsEngine = engine;

                engine.Break += (s, e) => Engine_Break(s, e, debugsession);

                engine.Step += (s, e) => Engine_Step(s, e, debugsession);

            }
            try
            {
                engine.Execute(InnerJsCode);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            if (debugsession != null)
            {
                var info = new ScriptDebugger.DebugInfo
                {
                    Start = new Jint.Parser.Position() { Line = -1 },
                    End = new Jint.Parser.Position() { Line = -1 },
                    EndOfExe = true,
                    HasValue = true
                };
                debugsession.DebugInfo = info;
            }

            string output = context.GetItem<string>(Kooboo.Sites.Scripting.Constants.OutputName);
            if (!string.IsNullOrWhiteSpace(output))
            {
                context.SetItem<string>(null, Kooboo.Sites.Scripting.Constants.OutputName);
            }
            return output;
        }

        public static object ExecuteDataSource(RenderContext context, Guid CodeId, Dictionary<string, object> parameters)
        {
            var sitedb = context.WebSite.SiteDb();

            var code = sitedb.Code.Get(CodeId);

            if (code == null)
            {
                return null;
            }

            object result = null;
            Jint.Engine engine = null;

            var debugsession = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(context, code.Id);

            if (debugsession == null)
            {
                engine = Kooboo.Sites.Scripting.Manager.GetJsEngine(context);
            }
            else
            {
                engine = Kooboo.Sites.Scripting.Manager.GetDebugJsEngine(context);

                foreach (var item in debugsession.BreakLines)
                {
                    engine.BreakPoints.Add(new Jint.Runtime.Debugger.BreakPoint(item, 0));
                }

                debugsession.JsEngine = engine;

                engine.Break += (s, e) => Engine_Break(s, e, debugsession);

                engine.Step += (s, e) => Engine_Step(s, e, debugsession);

            }

            try
            {

                var kcontext = context.GetItem<k>();

                Dictionary<string, string> config = new Dictionary<string, string>();
                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        if (item.Value != null)
                        {
                            config.Add(item.Key, item.Value.ToString());
                        }
                        else
                        {
                            config.Add(item.Key, null);
                        }
                    }
                }

                kcontext.config = config;
                kcontext.ReturnValues.Clear();

                engine.Execute(code.Body);

                kcontext.config = null;

                if (kcontext.ReturnValues.Count > 0)
                {
                    result = kcontext.ReturnValues.Last();
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            if (debugsession != null)
            {
                var info = new ScriptDebugger.DebugInfo
                {
                    Start = new Jint.Parser.Position() { Line = -1 },
                    End = new Jint.Parser.Position() { Line = -1 },
                    EndOfExe = true,
                    HasValue = true
                };
                debugsession.DebugInfo = info;
            }

            return result;
        }


        private static Jint.Runtime.Debugger.StepMode Engine_Step(object sender, Jint.Runtime.Debugger.DebugInformation e, ScriptDebugger.DebugSession session)
        {
            if (session.EndOfSession)
            {
                return Jint.Runtime.Debugger.StepMode.None;
            }

            var info = new ScriptDebugger.DebugInfo
            {
                Start = e.CurrentStatement.Location.Start,
                End = e.CurrentStatement.Location.End,

                Variables = Kooboo.Sites.Scripting.Manager.GetVariables(sender as Jint.Engine)
            };

            session.DebugInfo = info;

            int count = 1;
            while (session.Action == null)
            {
                System.Threading.Thread.Sleep(100);
                count += 1;
                if (count > 10000)
                {
                    break;
                }

                if (session.EndOfSession)
                {
                    return Jint.Runtime.Debugger.StepMode.None;
                }
            }


            if (session.EndOfSession)
            {
                return Jint.Runtime.Debugger.StepMode.None;
            }

            if (session.Action != null)
            {
                var mode = session.Action.StepMode;
                session.Action = null;
                return mode;
            }

            return Jint.Runtime.Debugger.StepMode.None;
        }

        private static Jint.Runtime.Debugger.StepMode Engine_Break(object sender, Jint.Runtime.Debugger.DebugInformation e, ScriptDebugger.DebugSession session)
        {
            if (session.EndOfSession)
            {
                return Jint.Runtime.Debugger.StepMode.None;
            }
            var info = new ScriptDebugger.DebugInfo
            {
                Start = e.CurrentStatement.Location.Start,
                End = e.CurrentStatement.Location.End,

                Variables = Kooboo.Sites.Scripting.Manager.GetVariables(sender as Jint.Engine)
            };

            session.DebugInfo = info;

            int count = 1;
            while (session.Action == null)
            {
                System.Threading.Thread.Sleep(100);
                count += 1;
                if (count > 10000)
                {
                    break;
                }

                if (session.EndOfSession)
                {
                    return Jint.Runtime.Debugger.StepMode.None;
                }
            }

            if (session.EndOfSession)
            {
                return Jint.Runtime.Debugger.StepMode.None;
            }

            if (session.Action != null)
            {
                var mode = session.Action.StepMode;
                session.Action = null;
                return mode;
            }

            return Jint.Runtime.Debugger.StepMode.None;
        }

        public static Kooboo.Sites.ScriptDebugger.DebugVariables GetVariables(Jint.Engine engines)
        {
            ScriptDebugger.DebugVariables variables = new ScriptDebugger.DebugVariables();

            if (engines.ExecutionContext.LexicalEnvironment != null)
            {
                var lexicalEnvironment = engines.ExecutionContext.LexicalEnvironment;

                variables.Local = GetLocalVariables(lexicalEnvironment);
                variables.Global = GetGlobalVariables(lexicalEnvironment);
            }

            return variables;
        }

        private static Dictionary<string, object> GetLocalVariables(LexicalEnvironment lex)
        {
            Dictionary<string, object> locals = new Dictionary<string, object>();
            if (lex != null && lex.Record != null)
            {
                AddRecordsFromEnvironment(lex, locals);
            }
            return locals;
        }

        private static Dictionary<string, object> GetGlobalVariables(LexicalEnvironment lex)
        {
            Dictionary<string, object> globals = new Dictionary<string, object>();
            LexicalEnvironment tempLex = lex;

            while (tempLex != null && tempLex.Record != null)
            {
                AddRecordsFromEnvironment(tempLex, globals);
                tempLex = tempLex.Outer;
            }
            return globals;
        }

        private static void AddRecordsFromEnvironment(LexicalEnvironment lex, Dictionary<string, object> locals)
        {
            var bindings = lex.Record.GetAllBindingNames();
            foreach (var binding in bindings)
            {
                if (locals.ContainsKey(binding) == false)
                {
                    var jsValue = lex.Record.GetBindingValue(binding, false);
                    if (jsValue.TryCast<ICallable>() == null)
                    {
                        if (!IsSystem(binding))
                        {
                            locals.Add(binding, GetObject(jsValue));
                        }
                    }
                }
            }
        }

        private static bool IsSystem(string binding)
        {
            if (string.IsNullOrWhiteSpace(binding))
            {
                return true;
            }

            string lower = binding.ToLower();

            if (lower == "json" || lower == "date" || lower == "nan" || lower == "infinity" || lower == "math" || lower == "undefined")
            {
                return true;
            }

            return false;
        }

        public static string GetString(object value)
        {
            if (value == null)
            {
                return "undefined";
            }

            if (value.GetType().IsValueType || value.GetType() == typeof(string))
            {
                return value.ToString();
            }
            else
            {
                if (value is Jint.Native.JsValue)
                {
                    var jsvalue = value as Jint.Native.JsValue;
                    if (jsvalue != null)
                    {
                        var jsobject = jsvalue.ToObject();
                        if (jsobject != null && (jsobject.GetType().IsValueType || jsobject.GetType() == typeof(string)))
                        {
                            return jsobject.ToString();
                        }
                        if (jsobject == null)
                        {
                            return "undefined";
                        }
                    }
                }
                else if (IsArray(value.GetType()))
                {
                    var valueType = value.GetType();
                    System.Text.StringBuilder builder = new System.Text.StringBuilder();

                    var items = value as IEnumerable<object>;
                    foreach (var item in items)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(",");
                        }
                        builder.Append(string.Format("\"{0}\"", GetPropValue(item.GetType(), item)));
                    }
                    builder.Insert(0, "[");
                    builder.Append("]");
                    return builder.ToString();
                }

                var members = GetDynamicMembers(value);
                return Lib.Helper.JsonHelper.Serialize(members);
            }
        }


        private static Dictionary<string, string> GetDynamicMembers(object obj)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (obj == null)
            {
                return result;
            }

            if (obj is System.Dynamic.ExpandoObject)
            {
                IDictionary<String, Object> ExpValues = obj as IDictionary<String, Object>;

                foreach (var item in ExpValues)
                {
                    var value = item.Value;
                    if (value == null)
                    {
                        result[item.Key] = null;
                    }
                    else
                    {
                        result[item.Key] = value.ToString();
                    }
                }
            }
            else if (obj is IDictionary<string, object>)
            {
                IDictionary<string, object> dictvalues = obj as IDictionary<string, object>;

                foreach (var item in dictvalues)
                {
                    var value = item.Value;
                    if (value == null)
                    {
                        result[item.Key] = null;
                    }
                    else
                    {
                        result[item.Key] = value.ToString();
                    }
                }

            }
            else if (obj is Jint.Native.JsValue)
            {
                var value = obj as Jint.Native.JsValue;

                if (value != null)
                {
                    var jsObject = value.ToObject();
                    return GetDynamicMembers(jsObject);
                }
            }

            else if (obj is IDictionary)
            {
                var dict = obj as IDictionary;

                foreach (var item in dict.Keys)
                {
                    if (item != null)
                    {
                        var itemvalue = dict[item];
                        string key = item.ToString();
                        if (itemvalue == null)
                        {
                            result[key] = null;
                        }
                        else
                        {
                            result[key] = itemvalue.ToString();
                        }
                    }
                }
            }

            else if (Lib.Reflection.TypeHelper.IsGenericCollection(obj.GetType()))
            {
                return result;
            }

            else if (obj.GetType().IsClass)
            {

                Type myType = obj.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    try
                    {
                        object propValue = prop.GetValue(obj, null);

                        result[prop.Name] = GetPropValue(prop.PropertyType, propValue);
                    }
                    catch (Exception)
                    {

                    }
                }

                // TODO: also get the methods...

            }

            return result;
        }

        private static string GetPropValue(Type returnType, object propValue)
        {
            string value = "undefined";

            if (propValue != null)
            {
                if (IsArray(returnType))
                {
                    int arrayCount = GetArrayCount(returnType, propValue);
                    value = string.Format("Array({0})", arrayCount);
                }
                else if ((returnType.IsClass && returnType != typeof(string)) ||
                    TypeHelper.IsDictionary(returnType))
                {
                    value = "{...}";//object
                }
                else
                {
                    value = propValue.ToString();
                }
            }
            return value;
        }

        private static int GetArrayCount(Type type, object value)
        {
            var countPropInfo = type.GetProperty("Count");
            var propInfo = countPropInfo != null ? countPropInfo : type.GetProperty("Length");
            if (propInfo != null)
            {
                int count;
                int.TryParse(propInfo.GetValue(value).ToString(), out count);
                return count;
            }
            return 0;
        }
        private static bool IsArray(Type type)
        {
            bool isArray = false;
            if (type.IsArray)
            {
                isArray = true;
            }
            else
            {
                bool isCol = TypeHelper.IsGenericCollection(type);
                if (TypeHelper.IsDictionary(type))
                {
                    isArray = false;
                }
                else
                {
                    isArray = isCol;
                }

            }
            return isArray;
        }
        private static Object GetObject(object obj)
        {
            var type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }

            else if (obj is Jint.Native.JsValue)
            {
                var value = obj as Jint.Native.JsValue;

                if (value != null)
                {
                    var jsObject = value.ToObject();
                    if (jsObject != null)
                    {
                        return GetObject(jsObject);
                    }
                    else
                    {
                        return "undefined";
                    }

                }
                return null;
            }

            return GetDynamicMembers(obj);
        }


    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data.Extensions;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.ScriptDebugger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class DebuggerApi : IApi
    {
        public string ModelName => "Debugger";

        public bool RequireSite => true;


        public bool RequireUser => true;

        public List<string> StartSession(Guid CodeId, ApiCall call)
        {
            if (CodeId != default(Guid))
            {
                var code = call.WebSite.SiteDb().Code.Get(CodeId);

                if (code != null && !string.IsNullOrEmpty(code.Body))
                {
                    var sesssion = Kooboo.Sites.ScriptDebugger.SessionManager.CreateSession(call.Context, CodeId);

                    return code.Body.Split("\n".ToCharArray()).ToList();
                }
            }
            return null;
        }

        public void StopSession(Guid CodeId, ApiCall call)
        {
            if (CodeId != default(Guid))
            {
                Kooboo.Sites.ScriptDebugger.SessionManager.RemoveSession(call.Context, CodeId, call.Context.Request.IP);
            }
        }

        public void SetBreakPoints(Guid CodeId, List<int> Points, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, CodeId);

            if (session != null)
            {
                if (session.JsEngine != null)
                {
                    // remvoe points.  
                    List<int> toremove = new List<int>();
                    foreach (var item in session.JsEngine.BreakPoints)
                    {
                        if (!Points.Any(o => o == item.Line))
                        {
                            toremove.Add(item.Line);
                        }
                    }

                    session.JsEngine.BreakPoints.RemoveAll(o => toremove.Contains(o.Line));

                    foreach (var item in Points)
                    {
                        var find = session.JsEngine.BreakPoints.Find(o => o.Line == item);
                        if (find == null)
                        {
                            session.JsEngine.BreakPoints.Add(new Jint.Runtime.Debugger.BreakPoint(item, 0));
                        }
                    }

                }

                else
                {

                    List<int> toremove = new List<int>();
                    foreach (var item in session.BreakLines)
                    {
                        if (!Points.Any(o => o == item))
                        {
                            toremove.Add(item);
                        }
                    }

                    session.BreakLines.RemoveAll(o => toremove.Contains(o));

                    foreach (var item in Points)
                    {
                        if (!session.BreakLines.Contains(item))
                        {
                            session.BreakLines.Add(item);
                        }
                    }
                }
            }

            else
            {
                var msg = Data.Language.Hardcoded.GetValue("You need to start the debugger first", call.Context);
                throw new Exception(msg);
            }
        }

        // get the break point info..... only return one for one break. 
        public Kooboo.Sites.ScriptDebugger.DebugInfo GetInfo(Guid CodeId, ApiCall call)
        {
            var session = SessionManager.GetDebugSession(call.Context, CodeId);

            if (session == null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Debugger not started", call.Context));
            }

            int counter = 0;

            while (counter < 10)
            {
                if (session.DebugInfo == null)
                {
                    counter += 1;
                    System.Threading.Thread.Sleep(100);
                }
                else
                {
                    var info = session.DebugInfo;
                    session.DebugInfo = null;

                    if (info.EndOfExe)
                    {
                        Sites.ScriptDebugger.SessionManager.RemoveSession(call.Context, session);
                    }
                    return info;
                }
            }

            return new Sites.ScriptDebugger.DebugInfo() { HasValue = false };
        }

        public void Step(Guid CodeId, string action, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, CodeId);
            if (session != null & !string.IsNullOrEmpty(action))
            {
                string lower = action.ToLower();
                if (lower == "into")
                {
                    session.Action = new Sites.ScriptDebugger.ClientAction() { StepMode = Jint.Runtime.Debugger.StepMode.Into };
                }
                else if (lower == "out")
                {
                    session.Action = new Sites.ScriptDebugger.ClientAction() { StepMode = Jint.Runtime.Debugger.StepMode.Out };
                }
                else if (lower == "over")
                {
                    session.Action = new Sites.ScriptDebugger.ClientAction() { StepMode = Jint.Runtime.Debugger.StepMode.Over };
                }
                else if (lower == "none")
                {
                    session.Action = new Sites.ScriptDebugger.ClientAction() { StepMode = Jint.Runtime.Debugger.StepMode.None };
                }
            }
        }
        [Obsolete]
        public object GetValue(Guid CodeId, string FullName, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, CodeId);
            if (session == null || session.JsEngine == null)
            {
                return "undefined";
            }
            else
            {
                var value = Lib.Helper.JintHelper.GetGebuggerValue(session.JsEngine, FullName);

                if (value == null)
                {
                    return "undefined";
                }
                else
                {
                    if (value.GetType().IsValueType || value.GetType() == typeof(string))
                    {
                        return value.ToString();
                    }
                    else
                    {
                        return Lib.Helper.JsonHelper.Serialize(value);
                    }
                }
            }
        }

        public ExeResult Execute(Guid CodeId, string JsStatement, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, CodeId);
            ExeResult result = new ExeResult();

            if (session != null && session.JsEngine != null)
            {
                if (Lib.Helper.JintHelper.IsMemberExpression(JsStatement))
                {
                    object value;
                    try
                    {
                        value = Lib.Helper.JintHelper.GetGebuggerValue(session.JsEngine, JsStatement);
                        result.Success = true;
                        if (value == null)
                        {
                            ExecuteRepl(JsStatement, session);
                            result.Model = Kooboo.Sites.Scripting.Manager.GetString(session.JsEngine.GetCompletionValue());
                        }
                        else
                        {
                            result.Model = Kooboo.Sites.Scripting.Manager.GetString(value);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Model = ex.Message;
                    }
                }
                else if (Lib.Helper.JintHelper.IsAssignmentExpression(JsStatement))
                {

                    try
                    {
                        ExecuteRepl(JsStatement, session);
                        result.Success = true;
                        var value = Lib.Helper.JintHelper.GetAssignmentValue(JsStatement);
                        result.Model = Kooboo.Sites.Scripting.Manager.GetString(value);
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Model = ex.Message;
                    }
                }

                else
                {
                    try
                    {
                        ExecuteRepl(JsStatement, session);
                        result.Model = Kooboo.Sites.Scripting.Manager.GetString(session.JsEngine.GetCompletionValue());
                        result.Success = true;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Model = ex.Message;
                    }

                }

                var variables = Kooboo.Sites.Scripting.Manager.GetVariables(session.JsEngine);

                result.Variables = variables;
            }
            else
            {
                result.Success = false;
                result.Model = Data.Language.Hardcoded.GetValue("Debug engine not started or has ended", call.Context);
                return result;
            }

            return result;

        }

        private static void ExecuteRepl(string JsStatement, DebugSession session)
        {
            var old = session.JsEngine.SetDebugHandlerMode(Jint.Runtime.Debugger.StepMode.None);
            session.JsEngine.ExecuteWithErrorHandle(JsStatement, new Jint.Parser.ParserOptions() { Tolerant = true });
            session.JsEngine.SetDebugHandlerMode(old);
        }

        // call to get the update variables after exe code. 
        public Kooboo.Sites.ScriptDebugger.DebugVariables GetVariables(Guid CodeId, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, CodeId);

            if (session != null || session.JsEngine != null)
            {
                return Kooboo.Sites.Scripting.Manager.GetVariables(session.JsEngine);
            }
            return new Sites.ScriptDebugger.DebugVariables();
        }

    }
}

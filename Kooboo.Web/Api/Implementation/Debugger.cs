//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
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

        public List<string> StartSession(Guid codeId, ApiCall call)
        {
            if (codeId != default(Guid))
            {
                var code = call.WebSite.SiteDb().Code.Get(codeId);

                if (code != null && !string.IsNullOrEmpty(code.Body))
                {
                    var sesssion = Kooboo.Sites.ScriptDebugger.SessionManager.CreateSession(call.Context, codeId);

                    return code.Body.Split("\n".ToCharArray()).ToList();
                }
            }
            return null;
        }

        public void StopSession(Guid codeId, ApiCall call)
        {
            if (codeId != default(Guid))
            {
                Kooboo.Sites.ScriptDebugger.SessionManager.RemoveSession(call.Context, codeId, call.Context.Request.IP);
            }
        }

        public void SetBreakPoints(Guid codeId, List<int> points, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, codeId);

            if (session != null)
            {
                if (session.JsEngine != null)
                {
                    // remvoe points.
                    List<int> toremove = new List<int>();
                    foreach (var item in session.JsEngine.BreakPoints)
                    {
                        if (!points.Any(o => o == item.Line))
                        {
                            toremove.Add(item.Line);
                        }
                    }

                    session.JsEngine.BreakPoints.RemoveAll(o => toremove.Contains(o.Line));

                    foreach (var item in points)
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
                        if (!points.Any(o => o == item))
                        {
                            toremove.Add(item);
                        }
                    }

                    session.BreakLines.RemoveAll(o => toremove.Contains(o));

                    foreach (var item in points)
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
        public Kooboo.Sites.ScriptDebugger.DebugInfo GetInfo(Guid codeId, ApiCall call)
        {
            var session = SessionManager.GetDebugSession(call.Context, codeId);

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

        public void Step(Guid codeId, string action, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, codeId);
            if (session != null & !string.IsNullOrEmpty(action))
            {
                string lower = action.ToLower();
                switch (lower)
                {
                    case "into":
                        session.Action = new Sites.ScriptDebugger.ClientAction() { StepMode = Jint.Runtime.Debugger.StepMode.Into };
                        break;
                    case "out":
                        session.Action = new Sites.ScriptDebugger.ClientAction() { StepMode = Jint.Runtime.Debugger.StepMode.Out };
                        break;
                    case "over":
                        session.Action = new Sites.ScriptDebugger.ClientAction() { StepMode = Jint.Runtime.Debugger.StepMode.Over };
                        break;
                    case "none":
                        session.Action = new Sites.ScriptDebugger.ClientAction() { StepMode = Jint.Runtime.Debugger.StepMode.None };
                        break;
                }
            }
        }

        [Obsolete]
        public object GetValue(Guid codeId, string fullName, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, codeId);
            if (session?.JsEngine == null)
            {
                return "undefined";
            }
            else
            {
                var value = Lib.Helper.JintHelper.GetGebuggerValue(session.JsEngine, fullName);

                if (value == null)
                {
                    return "undefined";
                }
                else
                {
                    if (value.GetType().IsValueType || value is string)
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

        public ExeResult Execute(Guid codeId, string jsStatement, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, codeId);
            ExeResult result = new ExeResult();

            if (session?.JsEngine != null)
            {
                if (Lib.Helper.JintHelper.IsMemberExpression(jsStatement))
                {
                    try
                    {
                        var value = Lib.Helper.JintHelper.GetGebuggerValue(session.JsEngine, jsStatement);
                        result.Success = true;
                        result.Model = value == null ? "undefined" : Kooboo.Sites.Scripting.Manager.GetString(value);
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Model = ex.Message;
                    }
                }
                else if (Lib.Helper.JintHelper.IsAssignmentExpression(jsStatement))
                {
                    try
                    {
                        session.JsEngine.Execute(jsStatement);
                        result.Success = true;
                        var value = Lib.Helper.JintHelper.GetAssignmentValue(jsStatement);
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
                        session.JsEngine.Execute(jsStatement);
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

        // call to get the update variables after exe code.
        public Kooboo.Sites.ScriptDebugger.DebugVariables GetVariables(Guid codeId, ApiCall call)
        {
            var session = Kooboo.Sites.ScriptDebugger.SessionManager.GetDebugSession(call.Context, codeId);

            if (session != null || session.JsEngine != null)
            {
                return Kooboo.Sites.Scripting.Manager.GetVariables(session.JsEngine);
            }
            return new Sites.ScriptDebugger.DebugVariables();
        }
    }
}
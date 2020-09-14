//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Jint.Native;
using Jint.Runtime.Debugger;
using Kooboo.Api;
using Kooboo.Data.Extensions;
using Kooboo.Lib.Helper;
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

        public object GetSession(ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context, DebugSession.GetWay.AutoCreate);

            return new
            {
                session.BreakLines,
                session.DebugInfo,
                session.End,
                session.CurrentCodeId,
                session.Exception
            };
        }

        public void StartSession(ApiCall call)
        {
            SessionManager.GetSession(call.Context, DebugSession.GetWay.AutoCreate).Clear();
        }

        public void StopSession(ApiCall call)
        {
            SessionManager.RemoveSession(call.Context);
        }

        public List<DebugSession.Breakpoint> SetBreakPoint(DebugSession.Breakpoint point, ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context, DebugSession.GetWay.AutoCreate);
            var exist = session.BreakLines.Any(a => a.codeId == point.codeId && a.Line == point.Line);

            if (exist)
            {
                session.BreakLines = session.BreakLines.Where(w => w.codeId != point.codeId || w.Line != point.Line).ToList();
            }
            else
            {
                session.BreakLines.Add(point);
            }

            if (session.JsEngine != null && point.codeId == session.CurrentCodeId)
            {
                var bk = session.JsEngine.BreakPoints.FirstOrDefault(a => a.Line == point.Line);
                if (bk != null) session.JsEngine.BreakPoints.Remove(bk);
                else session.JsEngine.BreakPoints.Add(new BreakPoint(point.Line, 0));
            }

            return session == null ? new List<DebugSession.Breakpoint>() : session.BreakLines;
        }

        public void Step(string action, ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context);
            if (session == null || !Enum.TryParse<StepMode>(action, out var step)) return;
            session.Next(step);
        }

        public string Execute(string JsStatement, ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context);
            if (session == null || session.JsEngine == null) return null;
            var old = session.JsEngine.SetDebugHandlerMode(StepMode.None);
            session.JsEngine.ExecuteWithErrorHandle(JsStatement, new Jint.Parser.ParserOptions() { Tolerant = true });
            session.JsEngine.SetDebugHandlerMode(old);
            var result = session.JsEngine.GetCompletionValue().ToObject();
            session.DebugInfo.Variables = Kooboo.Sites.Scripting.Manager.GetVariables(session.JsEngine);
            return Sites.Scripting.Manager.GetString(result);
        }
    }
}

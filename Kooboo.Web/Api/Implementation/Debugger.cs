//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Esprima;
using Jint.Native;
using Jint.Runtime.Debugger;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Data.Typescript;
using Kooboo.Lib.Helper;
using Kooboo.Sites;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.ScriptDebugger;

namespace Kooboo.Web.Api.Implementation
{
    public class DebuggerApi : IApi
    {
        public string ModelName => "Debugger";

        public bool RequireSite => true;

        public bool RequireUser => true;

        [Permission(Feature.CODE, Action = "debug")]
        public object GetSession(ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context, DebugSession.GetWay.AutoCreate);

            return new
            {
                session.Breakpoints,
                session.DebugInfo,
                session.End,
                session.CurrentCode,
            };
        }

        [Permission(Feature.CODE, Action = "debug")]
        public void StartSession(ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context, DebugSession.GetWay.AutoCreate);
            session.Clear();
        }

        [Permission(Feature.CODE, Action = "debug")]
        public void StopSession(ApiCall call)
        {
            SessionManager.RemoveSession(call.Context);
        }

        public class Breakpoints
        {
            public string[] Sources { get; set; }
            public int Line { get; set; }
        }

        [Permission(Feature.CODE, Action = "debug")]
        public List<Breakpoint> SetBreakPoint(string source, int line, int column, ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context, DebugSession.GetWay.AutoCreate);
            var code = call.WebSite.SiteDb().Code.GetByNameOrId(source);
            var cache = TypescriptCache.Instance.GetOrCreate(call.WebSite, code);
            var breakpoint = new Breakpoint(source, line, column, cache.SourceMap);

            var exist = session.Breakpoints.FirstOrDefault(f => f.Equals(breakpoint));
            if (exist == default)
            {
                session.Breakpoints.Add(breakpoint);
            }
            else
            {
                session.Breakpoints.Remove(exist);
            }

            session.SyncBreakpoints();
            return session.Breakpoints;
        }

        [Permission(Feature.CODE, Action = "debug")]
        public void Step(string action, ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context);

            if (action?.ToLower() == "stop")
            {
                session.Stop();
                return;
            }

            if (session == null || !Enum.TryParse<StepMode>(action, out var step)) return;
            session.Next(step);
        }

        [Permission(Feature.CODE, Action = "debug")]
        public string Execute(string JsStatement, ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context);
            if (session == null || session.JsEngine == null) return null;
            var result = session.JsEngine.DebugHandler.Evaluate(JsStatement, new ParserOptions() { Tolerant = true });

            try
            {
                return new Jint.Native.Json.JsonSerializer(session.JsEngine).Serialize(result, JsValue.Undefined, JsValue.Undefined).ToString();
            }
            catch (System.Exception)
            {
                return JsonHelper.Serialize(result?.ToObject());
            }
        }
    }
}

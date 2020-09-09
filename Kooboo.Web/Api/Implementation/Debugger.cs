//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
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

        public DebugSession GetSession(ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context, DebugSession.GetWay.AutoCreate);

            return new DebugSession()
            {
                BreakLines = session.BreakLines,
                DebugInfo = session.DebugInfo,
                End = session.End,
                CurrentCodeId = session.CurrentCodeId,
                Exception = session.Exception
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

        // get the break point info..... only return one for one break. 
        //public DebugInfo GetInfo(ApiCall call)
        //{
        //    var session = SessionManager.GetSession(call.Context);

        //    if (session == null)
        //    {
        //        throw new Exception(Data.Language.Hardcoded.GetValue("Debugger not started", call.Context));
        //    }

        //    int counter = 0;

        //    while (counter < 10)
        //    {
        //        if (session.DebugInfo == null)
        //        {
        //            counter += 1;
        //            System.Threading.Thread.Sleep(100);
        //        }
        //        else
        //        {
        //            var info = session.DebugInfo;
        //            session.DebugInfo = null;

        //            if (info.EndOfExe)
        //            {
        //                SessionManager.RemoveSession(call.Context);
        //            }
        //            return info;
        //        }
        //    }

        //    return new DebugInfo() { HasValue = false };
        //}

        public void Step(string action, ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context);
            if (session == null || !Enum.TryParse<StepMode>(action, out var step)) return;
            session.Next(step);
        }

        //[Obsolete]
        //public object GetValue(string FullName, ApiCall call)
        //{
        //    var session = SessionManager.GetSession(call.Context);
        //    if (session == null || session.JsEngine == null)
        //    {
        //        return "undefined";
        //    }
        //    else
        //    {
        //        var value = JintHelper.GetGebuggerValue(session.JsEngine, FullName);

        //        if (value == null)
        //        {
        //            return "undefined";
        //        }
        //        else
        //        {
        //            if (value.GetType().IsValueType || value.GetType() == typeof(string))
        //            {
        //                return value.ToString();
        //            }
        //            else
        //            {
        //                return JsonHelper.Serialize(value);
        //            }
        //        }
        //    }
        //}

        public ExeResult Execute(string JsStatement, ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context);
            ExeResult result = new ExeResult();

            if (session != null && session.JsEngine != null)
            {
                if (JintHelper.IsMemberExpression(JsStatement))
                {
                    object value;
                    try
                    {
                        value = JintHelper.GetGebuggerValue(session.JsEngine, JsStatement);
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
        public DebugVariables GetVariables(ApiCall call)
        {
            var session = SessionManager.GetSession(call.Context);

            if (session != null || session.JsEngine != null)
            {
                return Kooboo.Sites.Scripting.Manager.GetVariables(session.JsEngine);
            }
            return new DebugVariables();
        }

    }
}

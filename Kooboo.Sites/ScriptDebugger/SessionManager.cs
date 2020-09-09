//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using dotless.Core.Parser.Functions;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Sites.ScriptDebugger
{
    public static class SessionManager
    {
        static readonly ConcurrentDictionary<string, DebugSession> _sessions = new ConcurrentDictionary<string, DebugSession>();
        static readonly TimeSpan _lifeTime = new TimeSpan(0, 0, 5);



        static SessionManager()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var shouldRemoveSessions = _sessions.Where(w => DateTime.UtcNow - w.Value.LastRefreshTime > _lifeTime);
                        foreach (var item in shouldRemoveSessions)
                        {
                            item.Value.Clear();
                            _sessions.TryRemove(item.Key, out _);
                        }
                    }
                    catch (Exception ex)
                    {
                        Kooboo.Data.Log.Instance.Exception.WriteException(ex);
                    }

                    Thread.Sleep(3000);
                }
            });
        }

        public static DebugSession GetSession(RenderContext context, DebugSession.GetWay getWay = DebugSession.GetWay.Normal)
        {
            var key = GetSessionKey(context);
            if (key == null) return null;
            DebugSession session;

            if ((getWay | DebugSession.GetWay.AutoCreate) > 0)
            {
                session = _sessions.GetOrAdd(key, new DebugSession());
            }
            else
            {
                _sessions.TryGetValue(key, out session);
            }

            if (session != null)
            {
                session.LastRefreshTime = DateTime.UtcNow;
                if (getWay == DebugSession.GetWay.CurrentContext && session.DebuggingContext != null && session.DebuggingContext != context)
                {
                    session = null;
                }
            }

            return session;

        }

        public static void RemoveSession(RenderContext context)
        {
            var key = GetSessionKey(context);
            _sessions.TryRemove(key, out var session);
            if (session != null) session.Clear();
        }

        public static string GetSessionKey(RenderContext context)
        {
            if (context.WebSite == null) return null;
            return $"{context.WebSite.Id}__{context.Request?.IP}";
        }

    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.ScriptDebugger
{
    public static class SessionManager
    {
        private static object _locker = new object();

        public static Dictionary<Guid, List<DebugSession>> Sessions = new Dictionary<Guid, List<DebugSession>>();

        public static DebugSession CreateSession(RenderContext context, Guid codeId)
        {
            if (context.WebSite == null)
            {
                return null;
            }

            if (Sessions.ContainsKey(context.WebSite.Id))
            {
                var list = Sessions[context.WebSite.Id];

                lock (_locker)
                {
                    var find = list.Find(o => o.CodeId == codeId && o.IpAddress == context.Request.IP);

                    if (find == null)
                    {
                        var sitedb = context.WebSite.SiteDb();
                        var code = sitedb.Code.Get(codeId);

                        find = new DebugSession {CodeId = codeId};

                        if (code != null && code.IsEmbedded)
                        {
                            find.BodyHash = code.BodyHash;
                        }

                        find.IpAddress = context.Request.IP;
                        list.Add(find);
                        return find;
                    }
                    else
                    {
                        return find;
                    }
                }
            }
            else
            {
                lock (_locker)
                {
                    List<DebugSession> newsessions = new List<DebugSession>();
                    Sessions[context.WebSite.Id] = newsessions;

                    DebugSession session = new DebugSession
                    {
                        CodeId = codeId, IpAddress = context.Request.IP, ActiveTime = DateTime.Now
                    };

                    var sitedb = context.WebSite.SiteDb();
                    var code = sitedb.Code.Get(codeId);

                    if (code != null && code.IsEmbedded)
                    {
                        session.BodyHash = code.BodyHash;
                    }

                    newsessions.Add(session);

                    return session;
                }
            }
        }

        public static DebugSession GetDebugSession(RenderContext context, Guid codeId)
        {
            if (context.WebSite == null || codeId == default(Guid))
            {
                return null;
            }

            if (Sessions.ContainsKey(context.WebSite.Id))
            {
                var list = Sessions[context.WebSite.Id];

                lock (_locker)
                {
                    if (context.Request.IP != null)
                    {
                        var result = list.Find(o => o.CodeId == codeId && o.IpAddress == context.Request.IP);
                        if (result != null)
                        {
                            result.ActiveTime = DateTime.Now;

                            return result;
                        }
                    }
                    else
                    {
                        var result = list.Find(o => o.CodeId == codeId);
                        if (result != null)
                        {
                            result.ActiveTime = DateTime.Now;
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        public static DebugSession GetDebugSession(RenderContext context, string scriptInnerHtml)
        {
            if (context.WebSite == null || scriptInnerHtml == null)
            {
                return null;
            }

            if (Sessions.ContainsKey(context.WebSite.Id))
            {
                var list = Sessions[context.WebSite.Id];

                int bodyhash = Lib.Security.Hash.ComputeIntCaseSensitive(scriptInnerHtml);

                lock (_locker)
                {
                    if (context.Request.IP != null)
                    {
                        var result = list.Find(o => o.BodyHash == bodyhash && o.IpAddress == context.Request.IP);
                        if (result != null)
                        {
                            result.ActiveTime = DateTime.Now;

                            return result;
                        }
                    }
                    else
                    {
                        var result = list.Find(o => o.BodyHash == bodyhash);
                        if (result != null)
                        {
                            result.ActiveTime = DateTime.Now;
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        public static void RemoveSession(RenderContext context, DebugSession session)
        {
            RemoveSession(context, session.CodeId, session.IpAddress);
        }

        public static void RemoveSession(RenderContext context, Guid codeId, string ip)
        {
            if (context.WebSite == null)
            {
                return;
            }

            if (Sessions.ContainsKey(context.WebSite.Id))
            {
                var list = Sessions[context.WebSite.Id];
                lock (_locker)
                {
                    var find = list.Find(o => o.CodeId == codeId && o.IpAddress == ip);
                    if (find != null)
                    {
                        find.EndOfSession = true;
                        list.Remove(find);
                    }
                }
            }
        }
    }
}
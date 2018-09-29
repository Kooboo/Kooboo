//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Lib.Reflection;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Kooboo.Sites.Diagnosis
{
    public class Manager
    {

        private static Dictionary<Guid, DiagnosisSession> sessions = new Dictionary<Guid, DiagnosisSession>();


        public static List<DiagnosisChecker> ListCheckers(RenderContext context)
        {
            List<DiagnosisChecker> result = new List<DiagnosisChecker>();

            List<Type> diagnostics = AssemblyLoader.LoadTypeByInterface(typeof(IDiagnosis));

            foreach (var item in diagnostics)
            {
                if (item == typeof(CodeDiagnosis))
                {
                    var instance = Activator.CreateInstance(item) as IDiagnosis;
                    string groupname = instance.Group(context);

                    var sitedb = context.WebSite.SiteDb();

                    var allcode = sitedb.Code.ListByCodeType(Models.CodeType.Diagnosis);

                    foreach (var code in allcode)
                    {
                        var checker = new DiagnosisChecker();
                        checker.Group = groupname;
                        checker.Name = code.Name;
                        checker.IsCode = true;
                        result.Add(checker);
                    }
                }
                else
                {
                    var instance = Activator.CreateInstance(item) as IDiagnosis;
                    var checker = new DiagnosisChecker();
                    checker.Group = instance.Group(context);
                    checker.Name = instance.Name(context);
                    checker.Type = item;
                    result.Add(checker);
                }
            }

            return result;
        }

        public static Guid StartSession(List<Guid> checkers, RenderContext context)
        {
            Guid sessionid = System.Guid.NewGuid();

            DiagnosisSession session = new DiagnosisSession();
            session.context = context;

            var allcheckers = ListCheckers(context);

            List<DiagnosisChecker> selected = new List<DiagnosisChecker>();

            foreach (var item in allcheckers)
            {
                if (checkers.Contains(item.Id))
                {
                    selected.Add(item);
                }
            }

            session.AllCheckers = selected;

            CheckerTask task = new CheckerTask();
            task.Session = session;

            Thread thread = new Thread(task.Exe);
            thread.Start(); 
            
            sessions.Add(sessionid, session);

            return sessionid;

        }


        public static SessionStatus CheckStatus(Guid sessionid)
        {
            if (sessions.ContainsKey(sessionid))
            {
                var session = sessions[sessionid];
                SessionStatus status = new SessionStatus();
                status.InfoCount = session.informationCount;
                status.CriticalCount = session.CriticalCount;
                status.WarningCount = session.WarningCount;
                status.Messages = session.FlushMessage();
                status.IsFinished = session.IsFinished;
                status.HeadLine = session.Headline;

                return status;
            }
            else
            {
                return new SessionStatus() { IsFinished = true };
            }

        }

    }



}

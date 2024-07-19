//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.


namespace Kooboo.Web.Api.Implementation
{
    //public class DiagnosisApi : IApi
    //{ 
    //    public string ModelName
    //    {
    //        get
    //        {
    //            return "Diagnosis";
    //        }
    //    }

    //    public bool RequireSite
    //    {
    //        get
    //        {
    //            return true;
    //        }
    //    }

    //    public bool RequireUser
    //    {
    //        get
    //        {
    //            return true;
    //        }
    //    }

    //    public List<object> List(ApiCall call)
    //    {
    //        List<CheckerInfo> result = new List<CheckerInfo>();
    //        var all = Kooboo.Sites.Diagnosis.Manager.ListCheckers(call.Context);
    //        foreach (var item in all)
    //        {
    //            CheckerInfo info = new CheckerInfo();
    //            info.Name = item.Name;
    //            info.Group = item.Group;
    //            info.Id = item.Id;
    //            result.Add(info); 
    //        }

    //        return result.ToList<object>(); 
    //    }


    //    public class CheckerInfo
    //    {
    //        public string Group { get; set; } 
    //        public string Name { get; set; }
    //        public Guid Id { get; set; }
    //    }

    //    public Guid StartSession(List<Guid> checkers, ApiCall call)
    //    {
    //        return Kooboo.Sites.Diagnosis.Manager.StartSession(checkers, call.Context); 
    //    }

    //    public Kooboo.Sites.Diagnosis.SessionStatus Status(Guid sessionid, ApiCall call)
    //    {
    //        return Kooboo.Sites.Diagnosis.Manager.CheckStatus(sessionid); 
    //    }

    //    public void Cancel(Guid sessionid, ApiCall call)
    //    {

    //    }


    //    //#region OLD

    //    //public List<TypeEntry<DisplayMetaInfo, ISiteDiagnostic>> Items(ApiCall apiCall)
    //    //{
    //    //    var context = apiCall.Context;
    //    //    var siteDiagnostics = OptionsService.Get<DiagnosticOptions>().SiteDiagnostics(context);
    //    //    return siteDiagnostics;
    //    //}

    //    //public  void Scan(ApiCall apiCall)
    //    //{
    //    //    var itemsJson = apiCall.Context.Request.GetValue("items");
    //    //    var items = Lib.Helper.JsonHelper.Deserialize<string[]>(itemsJson);

    //    //     Tasks.Scan(apiCall.WebSite.SiteDb(), apiCall.Context, items);
    //    //}

    //    //public object Progress(ApiCall apiCall)
    //    //{
    //    //    DiagnosticTask task;
    //    //    if (!Tasks.TryGetTask(apiCall.WebSite.SiteDb().Id, out task))
    //    //    {
    //    //        return new
    //    //        {
    //    //            Messages = Enumerable.Empty<DiagnosticMessage>(),
    //    //            Finished = true
    //    //        };
    //    //    }

    //    //    var messages = task.FlushMessages();
    //    //    return new
    //    //    {
    //    //        Messages = messages,
    //    //        Finished = messages.Any() ? false : task.Finished
    //    //    };
    //    //}

    //    //public void Cancel(ApiCall apiCall)
    //    //{
    //    //    DiagnosticTask task;
    //    //    if (!Tasks.TryGetTask(apiCall.WebSite.SiteDb().Id, out task))
    //    //        return;

    //    //    task.Cancel();
    //    //}

    //    //#endregion
    //} 

}

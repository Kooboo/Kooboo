using System.Linq;
using Kooboo.Api;
using Kooboo.IndexedDB;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Service;

public static class LogService
{
    public static List<LogEntry> Filter(List<LogEntry> all, ApiCall call)
    {
        var actionType = call.GetValue("actionType");
        var storeName = call.GetValue("storeName");
        var name = call.GetValue("name");
        var user = call.GetValue("userId");
        var startDate = call.GetValue("startDate");
        var endDate = call.GetValue("endDate");
        var siteDb = call.WebSite.SiteDb();




        if (Enum.TryParse<EditType>(actionType, out var editType))
        {
            all = all.Where(w => w.EditType == editType).ToList();
        }

        if (Guid.TryParse(user, out var userId))
        {
            all = all.Where(w => w.UserId == userId).ToList();
        }

        if (!string.IsNullOrWhiteSpace(storeName))
        {
            if (storeName == "Table")
            {
                all = all.Where(it => it.IsTable).ToList();
            }
            else
            {
                all = all.Where(w => w.StoreName == storeName).ToList();
            }
        }

        if (DateTime.TryParse(startDate, out var startDateObj))
        {
            all = all.Where(w => w.TimeTick >= startDateObj.Ticks).ToList();
        }

        if (DateTime.TryParse(endDate, out var endDateObj))
        {
            all = all.Where(w => w.TimeTick <= endDateObj.Ticks).ToList();
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var matched = new List<LogEntry>();

            foreach (var item in all)
            {
                var itemName = Sites.Service.LogService.GetLogDisplayName(siteDb, item);
                if (itemName?.Contains(name, StringComparison.CurrentCultureIgnoreCase) ?? false)
                {
                    matched.Add(item);
                }
            }

            all = matched;
        }

        return all;
    }


}
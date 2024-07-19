using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Language;
using Kooboo.Data.Storage;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Scripting.Global.SiteItem;
using Kooboo.Sites.Service;

namespace Kooboo.Web.Service
{
    public class DashBoardInfo
    {

        private RenderContext context { get; set; }

        private SiteDb sitedb { get; set; }


        public DashBoardInfo(RenderContext Context)
        {
            this.context = Context;
            this.sitedb = Context.WebSite.SiteDb();
        }

        public ResourceCount[] Resource
        {
            get
            {
                List<IRepository> main = new List<IRepository>();
                main.Add(this.sitedb.Pages);
                main.Add(this.sitedb.Views);
                main.Add(this.sitedb.Layouts);
                main.Add(this.sitedb.Code);
                main.Add(this.sitedb.TextContent);
                main.Add(this.sitedb.Images);
                main.Add(this.sitedb.Styles);
                main.Add(this.sitedb.Scripts);
                main.Add(this.sitedb.HtmlBlocks);
                main.Add(this.sitedb.Labels);

                List<ResourceCount> Result = new List<ResourceCount>();


                foreach (var item in main)
                {
                    var name = Hardcoded.GetValue(item.StoreName, this.context);

                    var counter = item.Store.Count();

                    string folder = item.Store.ObjectFolder;

                    var size = Kooboo.Lib.Helper.IOHelper.GetDirectorySize(folder);

                    ResourceCount model = new ResourceCount() { Name = name, Count = counter, Size = size };

                    Result.Add(model);
                }

                return Result.ToArray();
            }

        }


        public LastEdit[] EditLog
        {
            get
            {
                KEditLog editLog = new KEditLog(this.context);

                return editLog.LastLog(0, 10);
            }

        }


        public List<ResourceCount> Visitors
        {
            get
            {
                return Kooboo.Sites.Service.VisitorLogService.MonthlyVisitors(this.sitedb);
            }
        }

        public ClientInformation Info
        {

            get
            {
                var json = GetInfo(this.context.User.Language);

                if (json != null)
                {
                    return System.Text.Json.JsonSerializer.Deserialize<ClientInformation>(json);
                }
                return null;

            }
        }

        private string GetInfo(string lang)
        {
            string fileName = System.IO.Path.Combine(Kooboo.Data.AppSettings.TempDataPath, lang + ".info");
            if (System.IO.File.Exists(fileName))
            {
                var info = new System.IO.FileInfo(fileName);

                if (info.LastWriteTime > DateTime.Now.AddDays(-3))
                {
                    return System.IO.File.ReadAllText(fileName);
                }
            }

            return GetSetInfoFromRemote(lang).Result;

        }

        private async Task<string> GetSetInfoFromRemote(string lang)
        {
            var url = "http://www.kooboo.com/api/clientInfo?lang=" + lang + "&online=" + Kooboo.Data.AppSettings.IsOnlineServer.ToString();

            var json = await Kooboo.Lib.Helper.HttpHelper.GetString2Async(url);

            if (!string.IsNullOrWhiteSpace(json) && Kooboo.Lib.Helper.JsonHelper.IsJson(json))
            {
                string fileName = System.IO.Path.Combine(Kooboo.Data.AppSettings.TempDataPath, lang + ".info");
                System.IO.File.WriteAllText(fileName, json);
            }

            return json;

        }


        public TopSummary Top
        {
            get
            {
                return this.sitedb.VisitorLog.TopSummary;
            }
        }


        public SiteInfo Site
        {
            get
            {
                SiteInfo info = new SiteInfo();

                info.CreationDate = this.sitedb.WebSite.CreationDate;

                info.SiteName = this.sitedb.WebSite.DisplayName;

                var users = sitedb.SiteUser.All();

                foreach (var item in users)
                {

                    info.Users.Add(new SiteInfo.UserInfo() { UserName = item.Name, Role = item.SiteRole });
                }

                if (this.context.User != null)
                {
                    if (GlobalDb.Users.IsAdmin(this.sitedb.WebSite.OrganizationId, this.context.User.Id))
                    {

                        var find = info.Users.Find(o => o.UserName == this.context.User.UserName);

                        if (find == null)
                        {
                            var textValue = Kooboo.Data.Language.Hardcoded.GetValue("Owner");

                            info.Users.Add(new SiteInfo.UserInfo() { UserName = this.context.User.UserName, Role = textValue });
                        }

                    }
                }

                //types.Add("p", Data.Language.Hardcoded.GetValue("public", call.Context));
                //types.Add("o", Data.Language.Hardcoded.GetValue("organization", call.Context));
                //types.Add("m", Data.Language.Hardcoded.GetValue("site user", call.Context));
                //types.Add("u", Data.Language.Hardcoded.GetValue("login user", call.Context));

                switch (sitedb.WebSite.SiteType)
                {
                    case Data.Definition.WebsiteType.p:
                        info.Type = Data.Language.Hardcoded.GetValue("public", this.context);
                        break;
                    case Data.Definition.WebsiteType.o:
                        info.Type = Data.Language.Hardcoded.GetValue("organization", this.context);
                        break;
                    case Data.Definition.WebsiteType.m:
                        info.Type = Data.Language.Hardcoded.GetValue("site user", this.context);
                        break;
                    case Data.Definition.WebsiteType.u:
                        info.Type = Data.Language.Hardcoded.GetValue("login user", this.context);
                        break;
                    default:
                        break;
                }


                var defaultRoute = ObjectRoute.GetDefaultRoute(this.sitedb);

                if (defaultRoute != null)
                {
                    info.PreviewUrl = this.sitedb.WebSite.BaseUrl()?.TrimEnd('/') + defaultRoute.Name;
                }

                var bindings = Data.Config.AppHost.BindingService.GetBySiteId(this.sitedb.Id);

                foreach (var item in bindings)
                {
                    var domain = item.FullDomain;

                    info.Domains.Add(domain);
                }

                return info;

            }
        }

    }


    public class SiteInfo
    {

        public DateTime CreationDate { get; set; }

        public string Type { get; set; }

        public string SiteName { get; set; }

        public List<string> Domains { get; set; } = new List<string>();

        public List<UserInfo> Users { get; set; } = new List<UserInfo>();


        public string PreviewUrl { get; set; }

        public class UserInfo
        {
            public string UserName { get; set; }

            public string Role { get; set; }
        }


    }


    public class ClientInformation
    {
        public List<LatestNews> news { get; set; }

        public Banner banner { get; set; }

        public List<DocEntry> doc { get; set; }
    }

    public class LatestNews
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Image { get; set; }

        public string Url { get; set; }

        public string Date { get; set; }
    }

    public class Banner
    {
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Url { get; set; }

        public string LinkText { get; set; }
    }

    public class DocEntry
    {
        public string Name { get; set; }

        public string ICO { get; set; }

        public string URL { get; set; }
    }
}



/*
 * 
 * {
  "news": [
    {
      "Title": "kooboo 本地版",
      "Summary": "你现在也可以下载kooboo到你的电脑端，或是自行部署在私有服务器上",
      "Image": "https://www.kooboo.com/img/logo-1080-zh.png",
      "Url": "https://www.kooboo.com/zh/downloads",
      "Date": "2024-02-28",
      "id": "14b0cab7-f68e-55d6-2c30-2d1eb492f76c",
      "userKey": "kooboo-E69CACE59CB0E78988",
      "lastModified": "2024-02-29T02:58:46.8486748Z",
      "creationDate": "2024-02-29T02:58:46.8486285Z",
      "parentId": "00000000-0000-0000-0000-000000000000",
      "version": 1376,
      "sequence": 0
    },
 
    {
      "Title": "节省80%的开发费用",
      "Summary": "为了实现规模扩大网络开发团队并同时降低成本的目标，我们需要解决以下挑战。",
      "Image": null,
      "Url": "https://www.kooboo.com/zh/docs/Innovation/save_80_percent_cost",
      "Date": "2024-02-26",
      "id": "53b7e73b-dcec-1a2d-1ae0-ff3430bb6287",
      "userKey": "E88A82E79C8180-E79A84E5BC80E58F91E8B4B9E794A8",
      "lastModified": "2024-02-29T02:35:14.7363147Z",
      "creationDate": "2024-02-29T02:35:14.736283Z",
      "parentId": "00000000-0000-0000-0000-000000000000",
      "version": 1370,
      "sequence": 0
    },
 
  ],
  "banner": {
    "IsOnlineServer": "true",
    "Title": "本地应用",
    "Summary": "您也可以在本地使用Kooboo",
    "LinkText": "前往下载安装",
    "Url": "https://www.kooboo.com/downloads",
    "id": "bc2387f1-bf58-2bcd-201a-37c25c496690",
    "userKey": "E69CACE59CB0E5BA94E794A8",
    "lastModified": "2024-02-29T02:25:21.2929473Z",
    "creationDate": "2024-02-29T02:25:21.292906Z",
    "parentId": "00000000-0000-0000-0000-000000000000",
    "version": 1365,
    "sequence": 0
  },
  "doc": [
    {
      "Name": "企业邮局",
      "ICO": "https://www.kooboo.com/icon/mail.svg",
      "URL": "https://www.kooboo.com/zh/docs/domain-mailbox/multi-domain-email-address",
      "id": "604f2186-5fa7-a199-a8db-fcb16be7f0b6",
      "userKey": "E4BC81E4B89AE982AEE5B180",
      "lastModified": "2024-02-29T03:06:32.2606263Z",
      "creationDate": "2024-02-29T03:06:32.2605979Z",
      "parentId": "00000000-0000-0000-0000-000000000000",
      "version": 1385,
      "sequence": 0
    },
  
    {
      "Name": "Development",
      "ICO": "https://www.kooboo.com/document-api-icon.svg",
      "URL": "https://www.kooboo.com/zh/docs/Development/develoment_concept",
      "id": "6a4305b2-4eb5-df58-9b72-d5c2301555ff",
      "userKey": "Development1",
      "lastModified": "2024-02-29T03:03:42.3873774Z",
      "creationDate": "2024-02-29T03:03:42.3873395Z",
      "parentId": "00000000-0000-0000-0000-000000000000",
      "version": 1381,
      "sequence": 0
    },
    {
      "Name": "Content",
      "ICO": "https://www.kooboo.com/icon/content.svg",
      "URL": "https://www.kooboo.com/zh/docs/Content/content_type",
      "id": "aca3f983-5a52-50d7-4ca5-d6a3b17648bb",
      "userKey": "Content0",
      "lastModified": "2024-02-29T03:03:07.121334Z",
      "creationDate": "2024-02-29T03:03:07.121305Z",
      "parentId": "00000000-0000-0000-0000-000000000000",
      "version": 1380,
      "sequence": 0
    }
  ]
}
 * 
 * 
 */
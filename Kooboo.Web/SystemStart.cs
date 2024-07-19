//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using System.Reflection;
using System.Threading;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Reflection;
using Kooboo.Data.Server;
using Kooboo.Data.SSL;
using Kooboo.Jobs;
using Kooboo.Render;
using Kooboo.Sites.Extensions;
using Kooboo.Web.Api;
using Kooboo.Web.Frontend;
using Kooboo.Web.JsTest;
using Kooboo.Web.Spa;
using MimeKit;

namespace Kooboo.Web
{
    public static class SystemStart
    {
        private static object _locker = new object();

        public static WebHostServer WebServer { get; private set; }

        public static void Start(int port)
        {
            var newLineFormat = MimeKit.FormatOptions.Default.GetType().GetField("newLineFormat", BindingFlags.NonPublic | BindingFlags.Instance);
            newLineFormat.SetValue(MimeKit.FormatOptions.Default, NewLineFormat.Dos);

            ThreadPool.SetMinThreads(9600, 500);

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    Data.Log.Instance.Exception.WriteException(ex);
                }
            };

            // init implementation. 
            Lib.IOC.Service.AddSingleton<IObjectReader>(new Sites.Render.Reflection.ObjectReader());

            AppSettings.SetCustomSslCheck();

            Sites.DataSources.DataSourceHelper.InitIDataSource();

            Data.Events.EventBus.Raise(new Data.Events.Global.ApplicationStartUp());

            WebServer = new WebHostServer(port, Middleware);

            if (!Data.AppSettings.Proxy)
            {
                Data.Config.AppHost.BindingService.EnsureLocalBinding();

                var AllPorts = Data.Config.AppHost.BindingRepo.All.Select(o => o.Port).Distinct().ToList();

                // foreach (var item in Data.Config.AppHost.BindingRepo.All)
                foreach (var item in AllPorts)
                {
                    if (item > 0 && item != port)
                    {
                        if (!Lib.Helper.NetworkHelper.IsPortInUse(item))
                        {
                            WebServer.AddPort(item);
                        }
                    }
                }

                //#if !DEBUG

                var sslport = AppSettings.SslPort;

                if (AppSettings.IsOnlineServer || !Lib.Helper.NetworkHelper.IsPortInUse(sslport))
                {
                    WebServer.AddPort(sslport, true);
                    Console.WriteLine("SSL started " + sslport.ToString());
                }
                //#endif
            }

            WebServer.Start();

            JobWorker.Instance.Start();

            // Service.UpGradeService.UpgradeFix();
        }


        private static List<IKoobooMiddleWare> _middlewares;

        public static List<IKoobooMiddleWare> Middleware
        {
            get
            {
                if (_middlewares == null)
                {
                    lock (_locker)
                    {
                        if (_middlewares == null)
                        {
                            var middlewares = new List<IKoobooMiddleWare>();
                            middlewares.Add(new PwaMiddleware());
                            middlewares.Add(new FrontRequest.KoobooMiddleware());
                            middlewares.Add(new ApiMiddleware(new SiteApiProvider()));
                            middlewares.Add(new SpaMiddleWare(KoobooSpaViewOption()));
                            middlewares.Add(new SpaMultilingualMiddleware());
                            middlewares.Add(new RenderMiddleWare(KoobooBackEndViewOption()));
                            // middlewares.Add(new JsTestMiddleWare(KoobooJsTestOption()));
                            // middlewares.Add(new RenderMiddleWare(KoobooLolcaServerOption()));
                            middlewares.Add(new DefaultStartMiddleWare(KoobooBackEndViewOption()));
                            middlewares.Add(new SslCertMiddleWare());

                            middlewares.Add(new EndMiddleWare());
                            _middlewares = middlewares;
                        }
                    }
                }

                return _middlewares;
            }
        }

        // only call this before shut down the server. 
        public static void Stop(int port = 0)
        {
            // stop all web servers. 
            WebServer.Stop();

            // close all database. 
            //foreach (var item in Kooboo.Data.GlobalDb.WebSites.AllSites)
            //{
            //    item.Value.Published = false; //set to false in the memory only..
            //}

            foreach (var item in Data.Config.AppHost.SiteRepo.AllSites)
            {
                item.SiteDb().DatabaseDb.Close();
            }
        }

        private static RenderOption KoobooBackEndViewOption()
        {
            RenderOption option = new RenderOption();
            option.HtmlRaw = true;
            option.GetDiskRoot = GetRoot;
            option.StartPath = "/_Admin";
            option.ViewFolder = "view";
            option.LayoutFolder = "_layout";
            option.RequireUser = true;
            option.LoginPage = "/_Admin/login";
            option.PageAfterLogin = "/_Admin";
            option.RequireUserIgnorePath.Add("/_Admin");
            option.RequireUserIgnorePath.Add("/.well-known");

            option.RequireSpeicalSite = true;
            Dictionary<string, object> data = new Dictionary<string, object>();
            var value = Data.Language.LanguageSetting.CmsLangs;

            List<CmsLanguage> langvalue = new List<CmsLanguage>();
            foreach (var item in value)
            {
                langvalue.Add(new CmsLanguage() { Key = item.Key, Name = item.Value });
            }

            data.Add("cmslang", langvalue);
            data.Add("kooboosetting", Data.Helper.SettingHelper.GetKoobooSetting());

            option.InitData = data;

            option.EnableMultilingual = true;
            option.EnableRenderCache = true;
            option.MultilingualJsFile = "scripts/kooboo/text.js";

            return option;
        }

        private static SpaRenderOption KoobooSpaViewOption()
        {
            SpaRenderOption option = new SpaRenderOption();
            option.GetDiskRoot = GetRoot;
            option.StartPath = "/_admin";
            option.Prefix = "/_spa";
            option.ViewFolder = "view";
            option.LayoutFolder = "_layout";

            Dictionary<string, object> data = new Dictionary<string, object>();
            var value = Data.Language.LanguageSetting.CmsLangs;

            List<CmsLanguage> langvalue = new List<CmsLanguage>();
            foreach (var item in value)
            {
                langvalue.Add(new CmsLanguage() { Key = item.Key, Name = item.Value });
            }

            data.Add("cmslang", langvalue);
            data.Add("kooboosetting", Data.Helper.SettingHelper.GetKoobooSetting());

            option.InitData = data;
            option.EnableMultilingual = true;
            return option;
        }

        private static string GetRoot(RenderContext context)
        {
            return AppSettings.RootPath;
        }

        private static string LocalServerRoot(RenderContext context)
        {
            if (context.WebSite != null)
            {
                return context.WebSite.LocalRootPath;
            }

            return null;
        }

        public static bool LocalserverTryShouldHandle(RenderContext Context, RenderOption Options)
        {
            if (Context.WebSite != null && !string.IsNullOrEmpty(Context.WebSite.LocalRootPath))
            {
                return true;
            }

            return false;
        }

        private static RenderOption KoobooLolcaServerOption()
        {
            RenderOption option = new RenderOption();
            option.GetDiskRoot = LocalServerRoot;
            option.ShouldTryHandle = LocalserverTryShouldHandle;
            option.ViewFolder = "_view, view";
            option.LayoutFolder = "_layout, layout";
            return option;
        }

        private static JsTestOption KoobooJsTestOption()
        {
            JsTestOption option = new JsTestOption();
            option.RequestPrefix = "/_jstest";
            option.AssertJs.Add("expect.js");
            option.AssertJs.Add("mock.js");
            return option;
        }

        private static Kooboo.Api.IApiProvider _apiprovider;

        public static Kooboo.Api.IApiProvider CurrentApiProvider
        {
            get
            {
                if (_apiprovider == null)
                {
                    foreach (var item in Middleware)
                    {
                        if (item is Kooboo.Api.ApiMiddleware)
                        {
                            var apimiddle = item as Kooboo.Api.ApiMiddleware;

                            _apiprovider = apimiddle.ApiProvider;
                        }
                    }
                }

                return _apiprovider;
            }
        }

        public static void InitHeaders(RenderContext context)
        {
            context.Response.Headers["IsOnlineServer"] = Data.AppSettings.IsOnlineServer.ToString();
        }
    }

    public class CmsLanguage
    {
        public string Key { get; set; }

        public string Name { get; set; }
    }
}
//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Server;
using Kooboo.Jobs;
using Kooboo.Render;
using Kooboo.Web.Api;
using Kooboo.Web.Frontend;
using Kooboo.Web.JsTest;
using Kooboo.Web.Spa;
using System;
using System.Collections.Generic;

namespace Kooboo.Web
{
    public static class SystemStart
    {
        private static object _locker = new object();

        public static Dictionary<int, IWebServer> WebServers = new Dictionary<int, IWebServer>();

        public static void Start(int port)
        {

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                System.IO.File.AppendAllText("log.txt", "Unhandled exception: " + args.ExceptionObject);
            };

            // ensure that WindowsHost is working .   
            //foreach (var item in Data.GlobalDb.Dlls.All())
            //{
            //    AppDomain.CurrentDomain.Load(item.Content);
            //}

            Sites.DataSources.DataSourceHelper.InitIDataSource();


            Kooboo.Data.Events.EventBus.Raise(new Data.Events.Global.ApplicationStartUp());

            Data.GlobalDb.Bindings.EnsureLocalBinding();

            StartNewWebServer(port);

            foreach (var item in Kooboo.Data.GlobalDb.Bindings.All())
            {
                if (item.Port > 0 && item.Port != port)
                {
                    StartNewWebServer(item.Port);
                }
            }

            if (!WebServers.ContainsKey(443))
            {
                if (Data.AppSettings.IsOnlineServer)
                {
                    StartNewWebServer(443);
                }

                else if (!Lib.Helper.NetworkHelper.IsPortInUse(443))
                {
                    StartNewWebServer(443);
                }
            }

            JobWorker.Instance.Start();
        }


        public static void StartNewWebServer(int port)
        {
            if (!WebServers.ContainsKey(port))
            {
                var server = Kooboo.Data.Server.WebServerFactory.Create(port, Middleware);

                server.Start();
                WebServers[port] = server; 
            }
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
                            _middlewares = new List<IKoobooMiddleWare>();
                            _middlewares.Add(new FrontRequest.KoobooMiddleware());
                            _middlewares.Add(new ApiMiddleware(new SiteApiProvider()));

                            _middlewares.Add(new SpaMiddleWare(KoobooSpaViewOption()));

                            _middlewares.Add(new RenderMiddleWare(KoobooBackEndViewOption()));

                            _middlewares.Add(new JsTestMiddleWare(KoobooJsTestOption()));
                            _middlewares.Add(new RenderMiddleWare(KoobooLolcaServerOption()));

                            _middlewares.Add(new DefaultStartMiddleWare(KoobooBackEndViewOption()));

                            //_middlewares.Add(new Kooboo.Web.Frontend.SslCertMiddleWare()); 
                            _middlewares.Add(new EndMiddleWare());
                        }
                    }
                }
                return _middlewares;
            }
        }

        public static void Stop(int port = 0)
        {
            //if (port == 0)
            //{
            //    foreach (var item in servers)
            //    {
            //        item.Value.Stop();
            //        item.Value.Dispose();
            //    }
            //    servers.Clear();
            //}
            //else
            //{
            //    if (servers.ContainsKey(port))
            //    {
            //        var server = servers[port];
            //        if (server != null)
            //        {
            //            server.Stop();
            //            server.Dispose();
            //        }

            //        servers.Remove(port);
            //    }
            //}
        }

        private static RenderOption KoobooBackEndViewOption()
        {
            RenderOption option = new RenderOption();
            option.GetDiskRoot = GetRoot;
            option.StartPath = "/_admin";
            option.ViewFolder = "view";
            option.LayoutFolder = "_layout";
            option.RequireUser = true;
            option.LoginPage = "/_admin/account/login";
            option.PageAfterLogin = "/_Admin/Sites";
            option.RequireUserIgnorePath.Add("/_admin/account");
            option.RequireUserIgnorePath.Add("/_admin/scripts");
            option.RequireUserIgnorePath.Add("/_admin/styles");
            option.RequireUserIgnorePath.Add("/_admin/images");
            //option.RequireUserIgnorePath.Add(Kooboo.DataConstants.Default404Page);
            //option.RequireUserIgnorePath.Add(Kooboo.DataConstants.Default403Page);
            //option.RequireUserIgnorePath.Add(Kooboo.DataConstants.Default500Page);
            option.RequireUserIgnorePath.Add("/_admin/logo");
            option.RequireUserIgnorePath.Add("/_admin/error");
            option.RequireUserIgnorePath.Add("/_admin/kbtest");
            option.RequireUserIgnorePath.Add("/_admin/development/kscript");
            option.RequireUserIgnorePath.Add("/_admin/development/kview");

            option.RequireSpeicalSite = true;
            Dictionary<string, object> data = new Dictionary<string, object>();
            var value = Kooboo.Data.Language.LanguageSetting.CmsLangs;

            List<CmsLanguage> langvalue = new List<CmsLanguage>();
            foreach (var item in value)
            {
                langvalue.Add(new CmsLanguage() { Key = item.Key, Name = item.Value });
            }

            data.Add("cmslang", langvalue);
            data.Add("kooboosetting", Kooboo.Data.Helper.SettingHelper.GetKoobooSetting());

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
            var value = Kooboo.Data.Language.LanguageSetting.CmsLangs;

            List<CmsLanguage> langvalue = new List<CmsLanguage>();
            foreach (var item in value)
            {
                langvalue.Add(new CmsLanguage() { Key = item.Key, Name = item.Value });
            }

            data.Add("cmslang", langvalue);
            data.Add("kooboosetting", Kooboo.Data.Helper.SettingHelper.GetKoobooSetting());

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
            option.ViewFolder = "view";
            option.LayoutFolder = "_layout";
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

                            _apiprovider =  apimiddle.ApiProvider;  
                        }
                    } 
                } 
                return _apiprovider;  
            } 
        } 
    }

    public class CmsLanguage
    {
        public string Key { get; set; }

        public string Name { get; set; }
    }


}

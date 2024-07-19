//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Linq;
using System.Reflection;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Lib.Reflection;

namespace Kooboo.Web.DashBoard
{
    public static class DashBoardManager
    {
        static DashBoardManager()
        {
            List = new List<IDashBoard>();
            var types = AssemblyLoader.LoadTypeByInterface(typeof(IDashBoard));

            foreach (var item in types)
            {
                var instance = Activator.CreateInstance(item) as IDashBoard;
                if (instance != null)
                {
                    List.Add(instance);
                }
            }

            ItemTemplate = ReadEmbeddedResource("Kooboo.Web.DashBoard.DashBoardItem.html");
        }

        public static string ItemTemplate
        {
            get; set;
        }

        public static string ReadEmbeddedResource(string fullname)
        {
            string result = null;
            var assembly = Assembly.GetExecutingAssembly();
            var allnames = assembly.GetManifestResourceNames();
            if (!allnames.Contains(fullname, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }
            var stream = assembly.GetManifestResourceStream(fullname);
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            stream.Close();
            stream = null;
            return result;
        }

        public static string GetViewSource(DashBoardResponseModel response, IDashBoard dashboard)
        {
            if (!string.IsNullOrEmpty(response.ViewBody))
            {
                return response.ViewBody;
            }
            else
            {
                string viewname = response.ViewName;
                if (string.IsNullOrEmpty(viewname))
                {
                    viewname = dashboard.GetType().Name;
                }

                string result = null;
                if (viewname.Contains("."))
                {
                    result = ReadEmbeddedResource(viewname);
                }

                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }

                string fullname = dashboard.GetType().FullName;
                string embeddedresourcename = fullname;
                int index = fullname.LastIndexOf(".");
                if (index > 0)
                {
                    embeddedresourcename = fullname.Substring(0, index) + "." + viewname;
                }

                var assembly = Assembly.GetExecutingAssembly();
                var allnames = assembly.GetManifestResourceNames();

                foreach (var item in allnames)
                {
                    if (item.Contains(embeddedresourcename))
                    {
                        result = ReadEmbeddedResource(item);
                        if (!string.IsNullOrEmpty(result))
                        {
                            return result;
                        }
                    }
                }

                /// search all views... 
                string name = dashboard.GetType().Name + ".html";
                var basedir = AppDomain.CurrentDomain.BaseDirectory;

                var files = System.IO.Directory.GetFiles(basedir, name, SearchOption.AllDirectories);

                if (files != null && files.Count() > 0)
                {
                    foreach (var item in files)
                    {
                        string alltext = System.IO.File.ReadAllText(item);
                        return alltext;
                    }
                }
            }

            return null;
        }

        public static List<string> Render(RenderContext context)
        {
            List<string> results = new List<string>();
            foreach (var item in List)
            {
                var result = item.Render(context);
                if (result is DashBoardResponseHtml)
                {
                    var html = result as DashBoardResponseHtml;
                    string itemresult = ItemTemplate.Replace("{title}", item.DisplayName(context)).Replace("{body}", html.Body);
                    results.Add(itemresult);
                }
                else if (result is DashBoardResponseModel)
                {
                    var modelresponse = result as DashBoardResponseModel;
                    var viewsource = GetViewSource(modelresponse, item);
                    string body = Kooboo.Sites.Render.RenderHelper.ModelBind(modelresponse.Model, viewsource);

                    string itemresult = ItemTemplate.Replace("{title}", item.DisplayName(context)).Replace("{body}", body);
                    results.Add(itemresult);
                }
            }
            return results;
        }

        public static List<ViewModel.DashBoardItemHtml> ItemHtml(RenderContext context)
        {
            List<ViewModel.DashBoardItemHtml> results = new List<ViewModel.DashBoardItemHtml>();
            foreach (var item in List)
            {
                var result = item.Render(context);
                if (result is DashBoardResponseHtml)
                {
                    var html = result as DashBoardResponseHtml;
                    results.Add(new ViewModel.DashBoardItemHtml { Title = item.DisplayName(context), Body = html.Body, Link = html.Link });
                }
                else if (result is DashBoardResponseModel)
                {
                    var modelresponse = result as DashBoardResponseModel;
                    var viewsource = GetViewSource(modelresponse, item);
                    string body = Kooboo.Sites.Render.RenderHelper.ModelBind(modelresponse.Model, viewsource);
                    results.Add(new ViewModel.DashBoardItemHtml() { Title = item.DisplayName(context), Body = body, Link = modelresponse.Link });
                }
            }
            return results;
        }

        public static List<IDashBoard> List
        {
            get; set;
        }

    }

}

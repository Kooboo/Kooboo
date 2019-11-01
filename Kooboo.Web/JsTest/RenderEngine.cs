//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Render;
using Kooboo.Render.ObjectSource;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.JsTest
{
    public class RenderEngine
    {
        public static string ParseRelativeUrl(string rawRelativeUrl, JsTestOption option)
        {
            string relativeUrl = rawRelativeUrl;
            if (!string.IsNullOrEmpty(option.RequestPrefix))
            {
                if (relativeUrl.ToLower().StartsWith(option.RequestPrefix))
                {
                    relativeUrl = relativeUrl.Substring(option.RequestPrefix.Length);
                }
            }
            return relativeUrl;
        }

        private static string GenerateUrl(JsTestOption option, JsTest.JsTestCommand.JsCommand command, string folder, string file, string function = null)
        {
            string url = option.RequestPrefix + "/exe";

            Dictionary<string, string> para = new Dictionary<string, string>();
            string cmdname = command.ToString();

            if (!string.IsNullOrEmpty(cmdname))
            {
                para.Add("command", cmdname);
            }

            if (!string.IsNullOrEmpty(folder))
            {
                para.Add("folder", folder);
            }

            if (!string.IsNullOrEmpty(file))
            {
                para.Add("file", file);
            }

            if (!string.IsNullOrEmpty(function))
            {
                para.Add("function", function);
            }

            return Lib.Helper.UrlHelper.AppendQueryString(url, para);
        }

        private static string GeneratejsFile(JsTestOption option, string fileName, JsTest.JsTestCommand.JsCommand command = JsTestCommand.JsCommand.view, string function = null)
        {
            string prefix = option.RequestPrefix;
            if (!prefix.EndsWith("/") && !prefix.EndsWith("\\"))
            {
                prefix = prefix + "/";
            }

            if (fileName.StartsWith("/") || fileName.StartsWith("\\"))
            {
                fileName = fileName.Substring(1);
            }

            string name = Lib.Helper.UrlHelper.Combine(prefix, fileName);

            name += "?command=" + command.ToString();
            if (!string.IsNullOrEmpty(function))
            {
                name += "&function=" + function;
            }

            return name;
        }

        public static RenderRespnose Render(RenderContext context, JsTestOption option)
        {
            string root = option.GetDiskRoot(context);

            string relativeurl = RenderEngine.ParseRelativeUrl(context.Request.RawRelativeUrl, option);
            RenderRespnose response = new RenderRespnose();

            var command = JsTestHelper.ParseCommand(relativeurl);

            if (command.IsJs)
            {
                RenderJs(context, option, root, response, command);
            }
            else
            {
                response.ContentType = "text/html";

                if (command.Command == JsTestCommand.JsCommand.view)
                {
                    RenderTestView(context, option, response, command);
                }
                else if (command.Command == JsTestCommand.JsCommand.run)
                {
                    RenderTestRun(context, option, response, command);
                }
            }
            return response;
        }

        private static void RenderTestRun(RenderContext context, JsTestOption option, RenderRespnose response, JsTestCommand command)
        {
            var starthtml = JsTestHelper.GetStartHtml(context, option);
            string html = "<div>";

            html += "\r\n<script src='/_admin/kbtest/expect.js'></script>";
            html += "\r\n<script src='/_admin/kbtest/mock.js'></script>";

            var references = JsTestHelper.GetReferenceJs(context, option, command.Folder);

            foreach (var item in references)
            {
                html += "\r\n<script src='" + item + "'></script>";
            }

            if (string.IsNullOrEmpty(command.Folder) || command.Folder == "\\" || command.Folder == "/")
            {
                // run all.
                var allfiles = JsTestHelper.ListAllTestFiles(context, option, null);
                foreach (var file in allfiles)
                {
                    string fileurl = GeneratejsFile(option, file, JsTestCommand.JsCommand.run);
                    html += "\r\n<script src='" + fileurl + "'></script>";
                }
            }
            else if (string.IsNullOrEmpty(command.File))
            {
                // run folder.
                var allfiles = JsTestHelper.ListAllTestFiles(context, option, command.Folder);
                foreach (var file in allfiles)
                {
                    string fileurl = GeneratejsFile(option, file, JsTestCommand.JsCommand.run);
                    html += "\r\n<script src='" + fileurl + "'></script>";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(command.Function))
                {
                    // run file.
                    string filename = Lib.Helper.IOHelper.CombinePath(command.Folder, command.File);

                    string fileurl = GeneratejsFile(option, filename, JsTestCommand.JsCommand.run);
                    html += "\r\n<script src='" + fileurl + "'></script>";
                }
                else
                {
                    // run function.
                    string filename = Lib.Helper.IOHelper.CombinePath(command.Folder, command.File);

                    html += "<div><h4>You are running one unit test, open console to view any errors</h4></div>";

                    string fileurl = GeneratejsFile(option, filename, JsTestCommand.JsCommand.run, command.Function);
                    html += "\r\n<script src='" + fileurl + "'></script>";
                }
            }

            html += "</div>";
            response.Body = starthtml.Replace(JsTestHelper.PlaceHolder, html);
        }

        public static void RenderTestView(RenderContext context, JsTestOption option, RenderRespnose response, JsTestCommand command)
        {
            if (string.IsNullOrEmpty(command.Folder) || command.Folder == "\\" || command.Folder == "/")
            {
                // find the start html.
                var starthtml = JsTestHelper.GetStartHtml(context, option);
                string url = GenerateUrl(option, JsTestCommand.JsCommand.run, null, null);

                var folders = JsTestHelper.ListTestFolders(context, option);
                var count = JsTestHelper.CountTest(context, option, folders.ToList());
                string html = "<h3><a href='" + url + "'>run all tests</a>  (" + count.ToString() + " tests)</h3>\r\n";

                html += "<ul>";
                foreach (var item in folders)
                {
                    var files = JsTestHelper.ListAllTestFiles(context, option, item);
                    int testcount = JsTestHelper.CountTest(context, option, item);

                    if (testcount > 0)
                    {
                        html += "<li>";

                        url = GenerateUrl(option, JsTestCommand.JsCommand.run, item, null);
                        html += "<a href='" + url + "'>run</a> || ";

                        url = GenerateUrl(option, JsTestCommand.JsCommand.view, item, null);
                        html += "<a href='" + url + "'>view</a> || ";

                        html += files.Count.ToString() + " files || ";
                        html += testcount.ToString() + " tests || Folder: ";
                        html += item;
                        html += "</li>";
                    }
                }
                html += "</ul>";

                string output = starthtml.Replace(JsTestHelper.PlaceHolder, html);

                string info = JsTestHelper.GetInfoHtml(context, option);

                output = output.Replace("<div id=\"information\"></div>", info);

                response.Body = output;
            }
            else
            {
                if (string.IsNullOrEmpty(command.File))
                {
                    // view folder...
                    var starthtml = JsTestHelper.GetStartHtml(context, option);

                    var files = JsTestHelper.ListFolderFiles(context, option, command.Folder);

                    var count = JsTestHelper.CountTest(context, option, command.Folder);

                    string url = GenerateUrl(option, JsTestCommand.JsCommand.run, command.Folder, null);
                    string html = "<h3><a href='" + url + "'>run all tests</a>  (" + count.ToString() + " tests)</h3>\r\n";

                    html += "<ul>";

                    foreach (var item in files)
                    {
                        var functions = JsTestHelper.ListFileFunctions(context, option, item.Folder, item.file);

                        html += "<li>";

                        url = GenerateUrl(option, JsTestCommand.JsCommand.run, item.Folder, item.file);
                        html += "<a href='" + url + "'>run</a> || ";

                        url = GenerateUrl(option, JsTestCommand.JsCommand.view, item.Folder, item.file);
                        html += "<a href='" + url + "'>view</a>  || ";

                        html += functions.Count.ToString() + " tests || File: " + System.IO.Path.Combine(item.Folder, item.file);

                        html += "</li>";
                    }
                    html += "</ul>";

                    response.Body = starthtml.Replace(JsTestHelper.PlaceHolder, html);
                }
                else
                {
                    //view file.
                    var starthtml = JsTestHelper.GetStartHtml(context, option);
                    string url = GenerateUrl(option, JsTestCommand.JsCommand.run, command.Folder, command.File);

                    var functions = JsTestHelper.ListFileFunctions(context, option, command.Folder, command.File);

                    string html = "<h3><a href='" + url + "'>run all tests</a>  (" + functions.Count.ToString() + " tests)</h3>\r\n";

                    html += "<ul>";

                    foreach (var item in functions)
                    {
                        url = GenerateUrl(option, JsTestCommand.JsCommand.run, command.Folder, command.File, item);
                        html += "<li><a href='" + url + "'>run</a> || Test: " + item + "</li>";
                    }
                    html += "</ul>";
                    response.Body = starthtml.Replace(JsTestHelper.PlaceHolder, html);
                }
            }
        }

        public static void RenderJs(RenderContext context, JsTestOption option, string root, RenderRespnose response, JsTestCommand command)
        {
            response.ContentType = "application/javascript";

            if (!string.IsNullOrEmpty(command.JsPath))
            {
                string filename = command.JsPath.Replace("/", "\\");

                if (filename.IndexOf("?") > -1)
                {
                    filename = filename.Substring(0, filename.IndexOf("?"));
                }

                if (filename.StartsWith("\\"))
                {
                    filename = filename.Substring(1);
                }

                string fullname = root;
                string prepath = option.FolderPath(context);

                if (!string.IsNullOrEmpty(prepath))
                {
                    fullname = IOHelper.CombinePath(root, prepath);
                }

                fullname = IOHelper.CombinePath(fullname, filename);

                if (!System.IO.File.Exists(fullname))
                {
                    // This is to make sure the render of assert js...
                    foreach (var item in option.AssertJs)
                    {
                        if (filename.EndsWith(item))
                        {
                            fullname = System.IO.Path.Combine(Kooboo.Data.AppSettings.RootPath, "/_admin/kbtest/" + item);
                        }
                    }
                }

                if (System.IO.File.Exists(fullname))
                {
                    string baserelarive = GetRelative(prepath, command.JsPath);

                    if (command.Command == JsTestCommand.JsCommand.run)
                    {
                        string retryurl = string.Empty;
                        string rawfolder = filename;
                        int lastslash = rawfolder.LastIndexOf("\\");
                        if (lastslash > -1)
                        {
                            string folder = rawfolder.Substring(0, lastslash);
                            string jsfilename = rawfolder.Substring(lastslash + 1);
                            retryurl = GenerateUrl(option, JsTestCommand.JsCommand.run, folder, jsfilename);
                        }

                        // TODO: Render the k commands.
                        var alltext = IOHelper.ReadAllText(fullname);
                        alltext = RenderServerSide(alltext, root, context, baserelarive);

                        response.Body = RenderJs(option, alltext, command.Function, retryurl);
                    }
                    else
                    {
                        var alltext = IOHelper.ReadAllText(fullname);
                        alltext = RenderServerSide(alltext, root, context, baserelarive);

                        response.Body = alltext;
                    }
                }
            }
        }

        private static string GetRelative(string prepath, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "/";
            }

            path = path.Replace("\\", "/");

            int questionmark = path.IndexOf("?");
            if (questionmark > -1)
            {
                path = path.Substring(0, questionmark);
            }

            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            if (string.IsNullOrWhiteSpace(prepath))
            {
                prepath = "/";
            }
            if (!prepath.EndsWith("/"))
            {
                prepath += "/";
            }

            if (!string.IsNullOrWhiteSpace(prepath))
            {
                path = Lib.Helper.UrlHelper.Combine(prepath, path);
            }

            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            return path;
        }

        public static string RenderJs(JsTestOption option, string js, string function, string retryurl)
        {
            int start = js.IndexOf(option.FunctionBlockStart);
            int end = js.IndexOf(option.FunctionBlockEnd);
            if (start > -1 && end > -1 && end > start)
            {
                string block = js.Substring(start + option.FunctionBlockStart.Length, end - start - option.FunctionBlockStart.Length);

                string beginstring = js.Substring(0, start);
                string endstring = js.Substring(end + option.FunctionBlockEnd.Length + 1);

                var renderblock = RenderBlockRun(block, function, retryurl);
                return beginstring + renderblock + endstring;
            }
            else
            {
                return RenderBlockRun(js, function, retryurl);
            }
        }

        public static string RenderBlockRun(string jsBlock, string function, string rawUrl)
        {
            if (Lib.Helper.JintHelper.IsRequireJs(jsBlock))
            {
                return RenderRequireJsBlockRun(jsBlock, function, rawUrl);
            }

            if (string.IsNullOrEmpty(function))
            {
                string append = string.Empty;

                var functionnames = JintHelper.ListFunctionNames(jsBlock);

                foreach (var item in functionnames)
                {
                    string retryUrl = rawUrl + "&function=" + item;
                    append += "\r\ntry{" + item + "(); appendPass(\"" + item + "\"); } catch(ex) { appendFailed(\"" + item + "\", \"" + retryUrl + "\"); };";
                }

                return jsBlock + append;
            }
            else
            {
                // string funcbody = JintHelper.GetFuncBody(jsBlock, function);
                string funcbody = jsBlock;

                funcbody += "\r\n" + function + "();";
                return funcbody;
            }
        }

        public static string RenderRequireJsBlockRun(string jsBlock, string function, string rawUrl)
        {
            string append = string.Empty;

            List<Jint.Parser.Ast.FunctionDeclaration> allfunctions = null;

            if (string.IsNullOrEmpty(function))
            {
                allfunctions = JintHelper.ListRequireJsFuncs(jsBlock);

                List<string> names = new List<string>();
                foreach (var item in allfunctions)
                {
                    names.Add(item.Id.Name);
                }
                foreach (var item in names)
                {
                    string retryUrl = rawUrl + "&function=" + item;
                    append += "\r\ntry{" + item + "(); appendPass(\"" + item + "\"); } catch(ex) { appendFailed(\"" + item + "\", \"" + retryUrl + "\"); };";
                }
            }
            else
            {
                append += "\r\n" + function + "();";
            }

            return Lib.Helper.JintHelper.AppendRequireJsBlock(jsBlock, append, allfunctions);
        }

        public static string RenderServerSide(string source, string rootfolder, RenderContext context, string baseRelativeUrl)
        {
            Func<RenderContext, string> rootfun = (contextx) =>
                {
                    return rootfolder;
                };

            var tasks = Kooboo.Render.ServerSide.ServerEngine.GetJsRenderPlan(source);

            RenderOption option = new RenderOption {GetDiskRoot = rootfun};

            CommandDiskSourceProvider sourceprvoider = new CommandDiskSourceProvider(option);

            string result = string.Empty;

            foreach (var task in tasks)
            {
                result += task.Render(sourceprvoider, option, context, baseRelativeUrl);
            }
            return result;
        }
    }
}
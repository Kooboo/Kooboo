//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Esprima;
using Esprima.Ast;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;

namespace Kooboo.Web.JsTest
{
    public class JsTestHelper
    {
        public static string PlaceHolder = "{content}";

        private static string _defaultFolder = "/_admin";

        public static HashSet<string> ListTestFolders(RenderContext context, JsTestOption options)
        {
            HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var root = options.GetDiskRoot(context);

            if (!string.IsNullOrEmpty(options.TestFolder))
            {
                var allsubs = System.IO.Directory.GetDirectories(root, options.TestFolder, System.IO.SearchOption.AllDirectories);

                foreach (var item in allsubs)
                {
                    System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(item);

                    string fullname = info.FullName;
                    fullname = fullname.Substring(root.Length);
                    fullname = fullname.Replace("\\", "/");

                    var path = options.FolderPath(context);

                    if (!string.IsNullOrEmpty(path) && fullname.ToLower().StartsWith(path))
                    {
                        fullname = fullname.Substring(path.Length + 1);
                    }
                    result.Add(fullname);
                }
            }

            return result;
        }

        public static HashSet<JsFilePath> ListFolderFiles(RenderContext context, JsTestOption option, string Folder)
        {
            HashSet<JsFilePath> result = new HashSet<JsFilePath>();

            var root = option.GetDiskRoot(context);

            var path = option.FolderPath(context);

            if (!string.IsNullOrEmpty(path))
            {
                root = Lib.Helper.IOHelper.CombinePath(root, path);
            }

            if (!string.IsNullOrEmpty(Folder))
            {
                var fullfolder = Lib.Helper.IOHelper.CombinePath(root, Folder);

                if (System.IO.Directory.Exists(fullfolder))
                {
                    var allfiles = System.IO.Directory.GetFiles(fullfolder, "*.js", System.IO.SearchOption.AllDirectories);

                    foreach (var item in allfiles)
                    {
                        System.IO.FileInfo info = new System.IO.FileInfo(item);

                        if (!option.AssertJs.Contains(info.Name))
                        {
                            var jsPath = new JsFilePath();
                            if (!string.IsNullOrEmpty(Folder))
                            {
                                var index = info.FullName.IndexOf(Folder.Replace("/", "\\"), StringComparison.OrdinalIgnoreCase);
                                if (index > -1)
                                {
                                    var relativePath = info.FullName.Substring(index);

                                    jsPath.Folder = System.IO.Path.GetDirectoryName(relativePath);
                                    jsPath.file = System.IO.Path.GetFileName(relativePath);
                                }
                                else
                                {
                                    jsPath.Folder = Folder;
                                    jsPath.file = info.Name;
                                }


                            }
                            else
                            {
                                jsPath.file = info.Name;
                            }
                            result.Add(jsPath);
                        }
                    }
                }

            }

            return result;
        }

        public static HashSet<string> ListAllTestFiles(RenderContext context, JsTestOption option, string folder)
        {
            HashSet<string> Result = new HashSet<string>();

            HashSet<string> Folders;
            if (string.IsNullOrEmpty(folder))
            {
                Folders = ListTestFolders(context, option);
            }
            else
            {
                Folders = new HashSet<string>();
                Folders.Add(folder);
            }

            foreach (var item in Folders)
            {
                var folderfiles = ListFolderFiles(context, option, item);
                foreach (var file in folderfiles)
                {
                    string jsfile = Lib.Helper.IOHelper.CombinePath(file.Folder, file.file);
                    Result.Add(jsfile);
                }
            }
            return Result;
        }

        public static HashSet<string> ListFileFunctions(RenderContext context, JsTestOption option, string folder, string file)
        {
            var root = option.GetDiskRoot(context);

            string prepath = option.FolderPath(context);

            if (!string.IsNullOrEmpty(prepath))
            {
                root = Lib.Helper.IOHelper.CombinePath(root, prepath);
            }

            if (!string.IsNullOrEmpty(folder))
            {
                var fullfolder = Lib.Helper.IOHelper.CombinePath(root, folder);
                var fullfile = Lib.Helper.IOHelper.CombinePath(fullfolder, file);

                var alltext = System.IO.File.ReadAllText(fullfile);
                var block = GetBlockJs(option, alltext);
                return Lib.Helper.JintHelper.ListFunctionNames(block);

            }
            return new HashSet<string>();
        }

        public static int CountTest(RenderContext context, JsTestOption option, List<string> Folders)
        {
            int count = 0;

            foreach (var item in Folders)
            {
                count += CountTest(context, option, item);
            }
            return count;
        }

        public static int CountTest(RenderContext context, JsTestOption option, string folder)
        {
            int count = 0;
            var allfiles = ListFolderFiles(context, option, folder);
            foreach (var file in allfiles)
            {
                var funcs = ListFileFunctions(context, option, file.Folder, file.file);
                count += funcs.Count();
            }
            return count;
        }

        private static string GetBlockJs(JsTestOption option, string js)
        {
            int start = js.IndexOf(option.FunctionBlockStart);
            int end = js.IndexOf(option.FunctionBlockEnd);
            if (start > -1 && end > -1 && end > start)
            {
                return js.Substring(start + option.FunctionBlockStart.Length, end - start - option.FunctionBlockStart.Length);
            }
            return js;
        }

        private static bool IsJsFile(string url)
        {
            string lower = url.ToLower();
            if (lower.IndexOf("?") > -1)
            {
                lower = lower.Substring(0, lower.IndexOf("?"));
                return IsJsFile(lower);
            }

            if (lower.EndsWith(".js"))
            {
                return true;
            }

            return false;
        }

        public static JsTestCommand ParseCommand(string RelativeUrl)
        {
            var result = new JsTestCommand();

            if (string.IsNullOrEmpty(RelativeUrl) || RelativeUrl == "/" || RelativeUrl == "\\")
            {
                result.Command = JsTestCommand.JsCommand.view;
            }
            else if (IsJsFile(RelativeUrl))
            {
                result.IsJs = true;
                result.JsPath = RelativeUrl;
            }

            string querystring = RelativeUrl;
            if (querystring?.IndexOf("?") > -1)
            {
                querystring = querystring.Substring(querystring.IndexOf("?") + 1);
            }

            var query = System.Web.HttpUtility.ParseQueryString(querystring);

            var keys = query.AllKeys;

            if (keys.Contains("command"))
            {
                string command = query["command"];
                result.Command = (JsTestCommand.JsCommand)Enum.Parse(typeof(JsTestCommand.JsCommand), command);
            }

            if (keys.Contains("folder"))
            {
                result.Folder = query["folder"];
            }

            if (keys.Contains("file"))
            {
                result.File = query["file"];
            }

            if (keys.Contains("function"))
            {
                result.Function = query["function"];
            }


            return result;
        }

        public static string GetStartHtml(RenderContext context, JsTestOption option)
        {
            var approot = Kooboo.Data.AppSettings.RootPath;
            string root = Lib.Helper.IOHelper.CombinePath(approot, _defaultFolder);
            root = Lib.Helper.IOHelper.CombinePath(root, "kbtest");
            string FullFileName = Lib.Helper.IOHelper.CombinePath(root, "start.html");

            if (System.IO.File.Exists(FullFileName))
            {
                string html = System.IO.File.ReadAllText(FullFileName);
                return html.Replace("{home}", option.RequestPrefix);
            }
            return null;
        }

        public static string GetInfoHtml(RenderContext context, JsTestOption option)
        {
            var approot = Kooboo.Data.AppSettings.RootPath;
            string root = Lib.Helper.IOHelper.CombinePath(approot, _defaultFolder);
            root = Lib.Helper.IOHelper.CombinePath(root, "kbtest");
            string FullFileName = Lib.Helper.IOHelper.CombinePath(root, "info.html");

            if (System.IO.File.Exists(FullFileName))
            {
                return System.IO.File.ReadAllText(FullFileName);
            }
            return null;
        }

        public static HashSet<string> GetReferenceJs(RenderContext context, JsTestOption option, string folder)
        {
            if (string.IsNullOrWhiteSpace(folder) || folder == "\\" || folder == "/")
            {
                return GetReferenceJs(context, option);
            }

            HashSet<string> result = new HashSet<string>();

            string root = option.GetDiskRoot(context);

            System.IO.DirectoryInfo basedir = new System.IO.DirectoryInfo(root);

            var prepath = option.FolderPath(context);

            if (!string.IsNullOrEmpty(prepath))
            {
                root = Lib.Helper.IOHelper.CombinePath(root, prepath);
            }

            System.IO.DirectoryInfo rootdir = new System.IO.DirectoryInfo(root);

            folder = Lib.Helper.IOHelper.CombinePath(root, folder);

            var currentdir = new System.IO.DirectoryInfo(folder);

            while (currentdir.Exists && currentdir.FullName.Length > rootdir.FullName.Length)
            {
                var file = Lib.Helper.IOHelper.CombinePath(currentdir.FullName, option.JsReferenceFileName);
                if (System.IO.File.Exists(file))
                {
                    var alllines = System.IO.File.ReadAllLines(file);
                    foreach (var line in alllines)
                    {
                        if (line != null)
                        {
                            var lineresult = FindRelativeFileName(basedir, line, currentdir);

                            if (!string.IsNullOrEmpty(lineresult))
                            {
                                result.Add(lineresult);
                            }

                        }
                    }
                }

                currentdir = currentdir.Parent;
            }

            return result;
        }

        public static string FindRelativeFileName(System.IO.DirectoryInfo root, string relativefilename, System.IO.DirectoryInfo current)
        {
            if (string.IsNullOrWhiteSpace(relativefilename))
            {
                return null;
            }

            relativefilename = relativefilename.Trim();
            relativefilename = relativefilename.Trim('"');
            relativefilename = relativefilename.Trim('\'');
            relativefilename = relativefilename.Trim();


            relativefilename = relativefilename.Replace("/", "\\");
            if (relativefilename.StartsWith("\\"))
            {
                relativefilename = relativefilename.Substring(1);
            }

            // first check directly under root...
            string fullname = System.IO.Path.Combine(root.FullName, relativefilename);
            if (!System.IO.File.Exists(fullname))
            {
                // search up from current path... till find the file name. 
                fullname = System.IO.Path.Combine(current.FullName, relativefilename);

                while (!System.IO.File.Exists(fullname) && current.Parent != null)
                {
                    current = current.Parent;
                    fullname = System.IO.Path.Combine(current.FullName, relativefilename);
                }
            }

            if (System.IO.File.Exists(fullname))
            {
                string result = fullname.Substring(root.FullName.Length);

                result = result.Replace("\\", "/");
                if (!result.StartsWith("/"))
                {
                    result = "/" + result;
                }
                return result;
            }

            return null;

        }

        public static HashSet<string> GetReferenceJs(RenderContext context, JsTestOption option)
        {
            HashSet<string> result = new HashSet<string>();

            var allfolders = ListTestFolders(context, option);

            foreach (var folder in allfolders)
            {
                if (!string.IsNullOrWhiteSpace(folder) && folder != "\\" && folder != "/")
                {
                    var folderresult = GetReferenceJs(context, option, folder);
                    foreach (var item in folderresult)
                    {
                        if (!result.Contains(item.ToLower()))
                        {
                            result.Add(item.ToLower());
                        }
                    }
                }
            }
            return result;
        }

        public static List<FunctionDeclaration> ListRequireJsFuncs(string requireJsBlock)
        {
            var prog = new JavaScriptParser().ParseScript(requireJsBlock);

            if (prog != null && prog.Body.Count() > 0)
            {

                var item = prog.Body.First();

                if (item is ExpressionStatement)
                {
                    var expres = item as ExpressionStatement;

                    if (expres.Expression is CallExpression)
                    {
                        var call = expres.Expression as CallExpression;
                        if (call != null && call.Arguments.Count() == 2)
                        {
                            var requireargu = call.Arguments[1];

                            if (requireargu != null && requireargu is FunctionExpression)
                            {
                                var requireFunc = requireargu as FunctionExpression;

                                if (requireFunc != null)
                                {
                                    var functionDeclarations = JintHelper.GetFunctionDeclarations(requireFunc);
                                    return functionDeclarations.ToList();
                                }
                            }

                        }

                    }
                }
            }


            return new List<FunctionDeclaration>();
        }

    }

    public class JsFilePath
    {
        public string Folder { get; set; }

        public string file { get; set; }
    }
}
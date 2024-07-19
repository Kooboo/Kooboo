using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Render.Components
{
    public static class Source
    {
        static Source()
        {
            componentFolderNames = new List<string>();
            componentFolderNames.Add("_component");
            componentFolderNames.Add("component");

            LoadAllComponent();
        }

        private static object _lock = new object();

        private static Dictionary<string, Component> _list;
        public static Dictionary<string, Component> ComponentList
        {
            get
            {
                if (_list == null)
                {
                    _list = new Dictionary<string, Component>(StringComparer.OrdinalIgnoreCase);
                }
                return _list;
            }
            set { _list = value; }
        }

        public static List<string> componentFolderNames { get; set; }   // TODO: get from option setting.    


        public static void LoadAllComponent(string rootpath = null)
        {
            if (rootpath == null)
            {
                rootpath = Data.AppSettings.RootPath;
            }

            if (!rootpath.ToLower().Contains("_admin"))
            {
                rootpath = System.IO.Path.Combine(rootpath, "_admin");
            }

            foreach (var item in componentFolderNames)
            {
                string compRoot = System.IO.Path.Combine(rootpath, item);

                if (System.IO.Directory.Exists(compRoot))
                {
                    var rootdir = new System.IO.DirectoryInfo(compRoot);
                    var files = rootdir.GetFiles("*.html");

                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            var com = LoadComponent(file.FullName);
                            var key = GetComName(file.Name);
                            ComponentList[key] = com;
                        }
                    }

                }
            }

            string moduleroot = System.IO.Path.Combine(Data.AppSettings.RootPath, "modules");

            var moduledir = new System.IO.DirectoryInfo(moduleroot);

            if (System.IO.Directory.Exists(moduleroot))
            {
                // does not find from the root, try find them in the module. 
                foreach (var item in componentFolderNames)
                {
                    var allpath = moduledir.GetDirectories(item, SearchOption.AllDirectories);
                    if (allpath != null)
                    {
                        foreach (var dir in allpath)
                        {
                            var files = dir.GetFiles("*.html");

                            if (files != null && files.Any())
                            {
                                foreach (var file in files)
                                {
                                    var com = LoadComponent(file.FullName);
                                    var key = GetComName(file.Name);
                                    ComponentList[key] = com;
                                }
                            }
                        }
                    }
                }
            }

        }

        private static string GetComName(string filename)
        {
            if (filename.ToLower().EndsWith(".html"))
            {
                return filename.Substring(0, filename.Length - 5);
            }
            return filename;
        }

        public static Component LoadComponent(string fullFileName)
        {
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.FileInfo info = new FileInfo(fullFileName);

                var content = System.IO.File.ReadAllText(fullFileName);
                Component component = new Component();
                component.FullDiskPath = fullFileName;
                component.LastModified = info.LastWriteTime;
                var option = new Sites.Render.EvaluatorOption() { RenderHeader = false, RenderUrl = false };
                option.Evaluators = Kooboo.Render.Components.EvaluatorContainer.ListWithServerComponent;

                component.RenderTasks = Kooboo.Sites.Render.RenderEvaluator.Evaluate(content, option);

                return component;
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="rootFolder">rootfolder includes /_admin</param>
        /// <param name="componentName"></param>
        /// <returns></returns>
        public static string FindMapping(string rootFolder, string componentName)
        {
            foreach (var item in componentFolderNames)
            {
                string root = System.IO.Path.Combine(rootFolder, item);

                if (System.IO.Directory.Exists(root))
                {
                    var rootdir = new System.IO.DirectoryInfo(root);
                    var files = rootdir.GetFiles(componentName + ".*");

                    if (files != null && files.Any())
                    {
                        return GetRightFile(files);
                    }
                }
            }

            string moduleroot = System.IO.Path.Combine(Data.AppSettings.RootPath, "modules");

            var moduledir = new System.IO.DirectoryInfo(moduleroot);

            if (System.IO.Directory.Exists(moduleroot))
            {
                // does not find from the root, try find them in the module. 
                foreach (var item in componentFolderNames)
                {
                    var allpath = moduledir.GetDirectories(item, SearchOption.AllDirectories);
                    if (allpath != null)
                    {
                        foreach (var dir in allpath)
                        {
                            var files = dir.GetFiles(componentName + ".*");

                            if (files != null && files.Any())
                            {
                                return GetRightFile(files);
                            }
                        }
                    }
                }
            }

            return null;
        }


        internal static string GetRightFile(FileInfo[] allFiles)
        {

            if (allFiles.Count() == 1)
            {
                return allFiles[0].FullName;
            }
            else
            {
                foreach (var item in allFiles)
                {
                    var lower = item.Extension.ToLower();
                    if (lower.EndsWith("html") || lower.EndsWith("htm"))
                    {
                        return item.FullName;
                    }
                }
                return allFiles[0].FullName;
            }
        }



    }
}

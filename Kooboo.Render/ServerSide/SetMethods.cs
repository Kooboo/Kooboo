//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Render.ObjectSource;

namespace Kooboo.Render.ServerSide
{
    public class SetMethods : IServerTask
    {
        public List<string> paras { get; set; }
        public virtual string name { get { return "setMethods"; } }

        public string Render(CommandDiskSourceProvider sourceProvider, RenderOption option, RenderContext context, string baseRelativeUrl)
        {

            if (paras.Count() > 1)
            {
                string varname = paras[0];
                var relativeurl = paras[1];

                if (!string.IsNullOrWhiteSpace(varname) && !string.IsNullOrWhiteSpace(relativeurl))
                {

                    string scriptHeader = "if (typeof " + varname + " === \"undefined\" ) { " + varname + "= {};  } \r\n";

                    string body = string.Empty;

                    relativeurl = ServerHelper.EnsureRelative(relativeurl, baseRelativeUrl);

                    RenderFiles(sourceProvider, option, context, varname, relativeurl, ref scriptHeader, ref body);

                    return scriptHeader + "\r\n" + body;
                }

            }
            return null;
        }

        private void RenderFiles(CommandDiskSourceProvider sourceProvider, RenderOption option, RenderContext context, string varname, string relativePath, ref string ScriptHeader, ref string Body)
        {

            var root = sourceProvider.GetRoot(context);

            var folder = Lib.Helper.IOHelper.CombinePath(root, relativePath);

            if (System.IO.Directory.Exists(folder))
            {
                var allfiles = System.IO.Directory.GetFiles(folder, "*.*", System.IO.SearchOption.TopDirectoryOnly);

                foreach (var item in allfiles.OrderBy(o => o))
                {
                    if (item.ToLower().EndsWith(".js"))
                    {
                        string itemrelative = item.Substring(root.Length);

                        itemrelative = itemrelative.Replace("\\", "/");

                        var response = ServerEngine.RenderJs(sourceProvider, option, context, itemrelative);
                        if (response != null)
                        {
                            string text = null;

                            if (response.Body != null)
                            {
                                text = response.Body;
                            }
                            else if (response.BinaryBytes != null)
                            {
                                text = System.Text.Encoding.UTF8.GetString(response.BinaryBytes);
                            }

                            var functionlist = Lib.Helper.JintHelper.ListFunctions(text);

                            foreach (var func in functionlist)
                            {
                                if (func.Value != null && func.Value.Count > 0)
                                {
                                    ScriptHeader += varname + "." + func.Key + "=" + func.Key + ";\r\n";
                                }
                                else
                                {
                                    ScriptHeader += varname + "." + func.Key + "=" + func.Key + "(); \r\n";
                                }
                            }
                            Body += "\r\n\r\n" + text;
                        }
                    }
                }

                var allfolders = System.IO.Directory.GetDirectories(folder);

                foreach (var item in allfolders.OrderBy(o => o))
                {
                    string name = item;
                    if (name.ToLower().StartsWith(folder.ToLower()))
                    {
                        name = name.Substring(folder.Length);
                    }

                    if (name.StartsWith("/") || name.StartsWith("\\"))
                    {
                        name = name.Substring(1);
                    }

                    if (name.ToLower().StartsWith("kbtest"))
                    {
                        continue;
                    }

                    string subvarname = varname + "." + name;

                    ScriptHeader += "if (typeof " + subvarname + " === \"undefined\" ) { " + subvarname + "= {};  } \r\n";

                    string subpath = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.CombineRelativePath(relativePath, name);

                    RenderFiles(sourceProvider, option, context, subvarname, subpath, ref ScriptHeader, ref Body);
                }
            }
        }





    }


}

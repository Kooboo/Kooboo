//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Render.ObjectSource;

namespace Kooboo.Render.ServerSide
{
    public class LoadJsFolder : IServerTask
    {
        public List<string> paras { get; set; }
        public virtual string name { get { return "loadJsFolder"; } }

        public string Render(CommandDiskSourceProvider sourceProvider, RenderOption option, RenderContext context, string baseRelativeUrl)
        {
            string fulltext = null;

            if (paras.Count() > 0)
            {
                // load js, only accept one para, the relative url. 
                var relativeurl = paras.First();

                if (!string.IsNullOrWhiteSpace(relativeurl))
                {
                    relativeurl = ServerHelper.EnsureRelative(relativeurl, baseRelativeUrl);

                    var root = sourceProvider.GetRoot(context);

                    var folder = Lib.Helper.IOHelper.CombinePath(root, relativeurl);

                    if (System.IO.Directory.Exists(folder))
                    {
                        var allfiles = System.IO.Directory.GetFiles(folder, "*.*", System.IO.SearchOption.TopDirectoryOnly);

                        foreach (var item in allfiles)
                        {
                            if (item.ToLower().EndsWith(".js"))
                            {
                                string itemrelative = item.Substring(root.Length);

                                itemrelative = itemrelative.Replace("\\", "/");

                                var response = ServerEngine.RenderJs(sourceProvider, option, context, itemrelative);
                                if (response != null)
                                {
                                    if (response.Body != null)
                                    {
                                        fulltext += response.Body;
                                    }
                                    else if (response.BinaryBytes != null)
                                    {
                                        string text = System.Text.Encoding.UTF8.GetString(response.BinaryBytes);
                                        fulltext += text;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return fulltext;
        }
    }
}
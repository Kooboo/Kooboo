//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Render.ObjectSource;

namespace Kooboo.Render.ServerSide
{
    public class LoadJs : IServerTask
    {
        public List<string> paras { get; set; }
        public string name { get { return "loadJs"; } }

        public string Render(CommandDiskSourceProvider sourceProvider, RenderOption option, RenderContext context, string baseRelativeUrl)
        {
            if (paras.Count() > 0)
            {
                // load js, only accept one para, the relative url. 
                var relativeurl = paras.First();

                if (!string.IsNullOrWhiteSpace(relativeurl))
                {
                    relativeurl = ServerHelper.EnsureRelative(relativeurl, baseRelativeUrl);

                    var response = ServerEngine.RenderJs(sourceProvider, option, context, relativeurl);
                    if (response != null)
                    {
                        if (response.Body != null)
                        {
                            return response.Body;
                        }
                        else if (response.BinaryBytes != null)
                        {
                            string text = System.Text.Encoding.UTF8.GetString(response.BinaryBytes);
                            return text;
                        }
                    }
                }

            }
            return string.Empty;
        }
    }
}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Render.ObjectSource;

namespace Kooboo.Render.ServerSide
{
    public class StringTask : IServerTask
    {
        public string content { get; set; }
        public List<string> paras { get; set; }

        public string name => "string";

        public string Render(CommandDiskSourceProvider sourceProvider, RenderOption option, RenderContext context, string baseRelativeUrl)
        {
            if (content != null)
            {
                return content;
            }
            if (paras.Count() > 0)
            {
                return paras[0];
            }
            return null;
        }
    }
}

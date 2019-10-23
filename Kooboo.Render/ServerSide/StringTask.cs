//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Render.ObjectSource;
using System.Collections.Generic;
using System.Linq;

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
            return paras.Any() ? paras[0] : null;
        }
    }
}
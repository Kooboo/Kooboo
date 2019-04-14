using System;
using System.Collections.Generic;

using Kooboo.Data.Context;
using Kooboo.Sites.Render.Commands;

namespace Kooboo.Model
{
    public class ModelRenderContext
    {
        public RenderContext HttpContext { get; set; }

        public string RelativeUrl { get; set; }

        public ICommandSourceProvider SourceProvider { get; set; }
    }
}

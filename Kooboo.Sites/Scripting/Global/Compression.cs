using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Scripting.Global
{
    public class Compression
    {

        private RenderContext context { get; set; }

        public Compression(RenderContext context)
        {
            this.context = context;
        }

        public object Create()
        {
            return null; 
        } 

    }
}

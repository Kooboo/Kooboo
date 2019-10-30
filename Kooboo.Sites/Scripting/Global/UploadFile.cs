//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Sites.Scripting.Global
{
    public class UploadFile
    {
        private RenderContext Context { get; set; }

        public UploadFile(RenderContext context)
        {
            this.Context = context;
        }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public byte[] Bytes { get; set; }

        public FileInfo Save(string filename)
        {
            FileIO io = new FileIO(this.Context);
            return io.WriteBinary(filename, this.Bytes);
        }
    }
}
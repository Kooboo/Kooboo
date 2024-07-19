//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using Ude.Core;

namespace Ude
{
    public class CharsetDetector : UniversalDetector, ICharsetDetector
    {
        private string charset;

        private float confidence;

        public string Charset
        {
            get
            {
                return this.charset;
            }
        }

        public float Confidence
        {
            get
            {
                return this.confidence;
            }
        }

        public CharsetDetector() : base(31)
        {
        }

        public void Feed(Stream stream)
        {
            byte[] array = new byte[1024];
            int len;
            while ((len = stream.Read(array, 0, array.Length)) > 0 && !this.done)
            {
                this.Feed(array, 0, len);
            }
        }

        public bool IsDone()
        {
            return this.done;
        }

        public override void Reset()
        {
            this.charset = null;
            this.confidence = 0f;
            base.Reset();
        }

        protected override void Report(string charset, float confidence)
        {
            this.charset = charset;
            this.confidence = confidence;
        }
    }
}

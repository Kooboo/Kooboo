using System.Collections.Generic;
using MimeKit;

namespace Kooboo.Mail.Multipart
{
    // Change from String to Steam may increase performance.. 
    public class MultiPartInfo
    {
        public MultiPartInfo(string msgSource)
        {
            this.MsgSource = msgSource;
            this.length = this.MsgSource.Length;
        }

        internal string MsgSource { get; set; }
        public int length { get; set; }


        private HeaderList _header;

        public HeaderList Header
        {
            get
            {
                if (_header == null)
                {
                    if (this.MsgSource != null)
                    {
                        _header = this.ParseHeader();
                    }
                }
                return _header;

            }
            set
            {
                _header = value;
            }
        }

        private string _boundary;
        public string Boundary
        {
            get
            {
                if (_boundary == null)
                {
                    if (this.Header != null)
                    {
                        var bound = MultiPartHelper.ExtractBoundary(this.Header);
                        _boundary = bound;
                    }
                }
                return _boundary;
            }
            set
            {
                _boundary = value;
            }
        }


        private List<PartInfo> _parts;
        public List<PartInfo> Parts
        {
            get
            {
                if (_parts == null)
                {
                    this.ParseStructure();
                }

                return _parts;
            }
            set
            {
                _parts = value;
            }
        }

        private PartInfo _main;
        public PartInfo Main
        {
            get
            {
                if (_main == null)
                {
                    _main = new PartInfo(new LocationInfo() { StartPos = 0, EndPos = this.MsgSource.Length }, this);
                }
                return _main;
            }
        }

        public HeaderList ParseHeader()
        {
            var lineStartIndex = MultiPartHelper.FindDoubleLineIndex(this.MsgSource, 0);
            if (lineStartIndex < 0)
            {
                return null;
            }

            var LineEnd = this.MsgSource[lineStartIndex];

            if (LineEnd == 10 || LineEnd == 13)
            {
                // get first header.
                var header = this.MsgSource.Substring(0, lineStartIndex);
                var mo = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(header));
                MimeParser parser = new MimeParser(mo);
                return parser.ParseHeaders();
            }

            return null;

        }


        //parse the whole mail, and make all entity ready.
        public void ParseStructure()
        {
            var lineStartIndex = MultiPartHelper.FindDoubleLineIndex(this.MsgSource, 0);
            if (lineStartIndex < 0)
            {
                return;
            }

            var LineEnd = this.MsgSource[lineStartIndex];

            if (LineEnd == 10 || LineEnd == 13)
            {
                // get first header.
                var header = this.MsgSource.Substring(0, lineStartIndex);

                if (this._header == null)
                {
                    var mo = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(header));
                    MimeParser parser = new MimeParser(mo);
                    this._header = parser.ParseHeaders();
                }
            }

            if (!string.IsNullOrEmpty(this.Boundary))
            {
                var parts = MultiPartHelper.SplitParts(this.MsgSource, lineStartIndex, this.Boundary);

                List<PartInfo> partInfo = new List<PartInfo>();

                foreach (var item in parts)
                {
                    PartInfo info = new PartInfo(item, this);
                    partInfo.Add(info);
                }

                this._parts = partInfo;
            }

        }


    }
}

using System.Collections.Generic;
using MimeKit;

namespace Kooboo.Mail.Multipart
{
    public class PartInfo
    {
        public PartInfo(LocationInfo partInfo, MultiPartInfo ParentInfo)
        {
            this.Source = ParentInfo;
            if (partInfo != null)
            {
                this.partLocation = partInfo;
            }
        }

        private LocationInfo partLocation { get; set; }

        public MultiPartInfo Source { get; set; }

        private int HeaderLineIndex { get; set; }

        private HeaderList _header;
        public HeaderList HeaderList
        {
            get
            {
                if (_header == null)
                {
                    if (partLocation != null)
                    {
                        HeaderLineIndex = MultiPartHelper.FindDoubleLineIndex(this.Source.MsgSource, partLocation.StartPos, partLocation.EndPos);

                        if (HeaderLineIndex < 0)
                        {
                            return null;
                        }

                        var headerText = this.Source.MsgSource.Substring(this.partLocation.StartPos, HeaderLineIndex - this.partLocation.StartPos);

                        if (!string.IsNullOrWhiteSpace(headerText))
                        {

                            _header = MultiPartHelper.ParseHeaderList(headerText);
                        }
                        else
                        {
                            _header = new HeaderList();
                        }
                    }
                }

                return _header;
            }
        }

        private string _boundary;
        public string Boundary
        {
            get
            {
                if (_boundary == null)
                {
                    if (this.HeaderList != null)
                    {
                        var bound = MultiPartHelper.ExtractBoundary(this.HeaderList);
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

        public bool IsMultiPart
        {
            get
            {
                return !string.IsNullOrEmpty(this.Boundary);
            }

        }

        public string GetPartString()
        {
            if (this.partLocation != null)
            {
                var result = this.Source.MsgSource.Substring(this.partLocation.StartPos, this.partLocation.EndPos - this.partLocation.StartPos);
                return result == null ? string.Empty : result;
            }
            return string.Empty;
        }


        public string GetBodyPart()
        {
            if (this.HeaderLineIndex < 1)
            {
                this.HeaderLineIndex = MultiPartHelper.FindDoubleLineIndex(this.Source.MsgSource, this.partLocation.StartPos, this.partLocation.EndPos);
            }

            if (this.HeaderLineIndex > 0)
            {
                var result = MultiPartHelper.GetBodyString(this.Source.MsgSource, this.HeaderLineIndex, this.partLocation.EndPos);
                return result == null ? string.Empty : result;
            }
            return string.Empty;
        }


        public MimeEntity GetMimeEntity()
        {
            var partString = GetPartString();

            return MultiPartHelper.ParseEntity(partString);
        }

        private List<PartInfo> _parts;
        public List<PartInfo> Parts
        {
            get
            {
                if (_parts == null)
                {
                    if (this.IsMultiPart)
                    {
                        var parts = MultiPartHelper.SplitParts(this.Source.MsgSource, this.HeaderLineIndex, this.Boundary);

                        List<PartInfo> partInfo = new List<PartInfo>();

                        foreach (var item in parts)
                        {
                            PartInfo info = new PartInfo(item, this.Source);
                            partInfo.Add(info);
                        }

                        _parts = partInfo;
                    }
                    else
                    {

                    }
                }
                return _parts;

            }
            set
            {
                _parts = value;
            }
        }
    }

}

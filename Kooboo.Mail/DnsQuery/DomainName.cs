using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kooboo.Mail.Extension;

namespace Kooboo.Mail.DnsQuery
{
    public class DomainName : IEquatable<DomainName>, IComparable<DomainName>
    {
        private static readonly IdnMapping _idnParser = new() { UseStd3AsciiRules = true };
        private readonly string[] _labels;
        private int? _hashcode;
        private string? _toString;

        public DomainName(string[] labels)
        {
            _labels = labels;
        }

        internal DomainName(string label, DomainName parent)
        {
            _labels = new string[1 + parent.LabelCount];

            _labels[0] = label;
            Array.Copy(parent._labels, 0, _labels, 1, parent.LabelCount);
        }

        public static DomainName Root { get; } = new(Array.Empty<string>());

        public string[] Labels => _labels;

        public int LabelCount => _labels.Length;

        public DomainName GetParentName(int removeLabels = 1)
        {
            if (removeLabels < 0)
                throw new ArgumentOutOfRangeException(nameof(removeLabels));
            if (removeLabels > LabelCount)
                throw new ArgumentOutOfRangeException(nameof(removeLabels));
            if (removeLabels == 0)
                return this;
            string[] newLabels = new string[LabelCount - removeLabels];
            Array.Copy(_labels, removeLabels, newLabels, 0, newLabels.Length);
            return new DomainName(newLabels);
        }

        public bool IsEqualOrSubDomainOf(DomainName domainName)
        {
            if (Equals(domainName)) return true;
            if (domainName.LabelCount >= LabelCount) return false;
            return GetParentName(LabelCount - domainName.LabelCount).Equals(domainName);
        }

        public bool IsSubDomainOf(DomainName domainName)
        {
            if (domainName.LabelCount >= LabelCount) return false;
            return GetParentName(LabelCount - domainName.LabelCount).Equals(domainName);
        }

        public int CompareTo(DomainName other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(object other)
        {
            return Equals(other as DomainName);
        }

        public bool Equals(DomainName other)
        {
            return Equals(other, true);
        }

        public override int GetHashCode()
        {
            if (_hashcode.HasValue)
                return _hashcode.Value;

            int hash = LabelCount;

            for (int i = 0; i < LabelCount; i++)
            {
                unchecked
                {
                    hash = hash * 17 + _labels[i].ToLowerInvariant().GetHashCode();
                }
            }

            return (_hashcode = hash).Value;
        }

        public bool Equals(DomainName other, bool ignoreCase)
        {
            if (other is null)
                return false;

            if (LabelCount != other.LabelCount)
                return false;

            if (_hashcode.HasValue && other._hashcode.HasValue && _hashcode != other._hashcode)
                return false;

            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            for (int i = 0; i < LabelCount; i++)
            {
                if (!string.Equals(_labels[i], other._labels[i], comparison))
                    return false;
            }

            return true;
        }

        internal static DomainName Asterisk { get; } = new(new[] { "*" });

        internal bool IsRoot => _labels.Length == 0;

        internal int MaximumRecordDataLength
        {
            get
            {
                return LabelCount + _labels.Sum(x => x.Length);
            }
        }

        internal DomainName Add0x20Bits()
        {
            string[] newLabels = new string[LabelCount];
            for (int i = 0; i < newLabels.Length; i++)
            {
                newLabels[i] = Labels[i].Add0x20Bits();
            }

            return new DomainName(newLabels) { _hashcode = _hashcode };
        }

        public static DomainName Parse(string s)
        {
            if (TryParse(s, out var res))
                return res!;

            throw new ArgumentException("Domain name could not be parsed", nameof(s));
        }

        public static bool TryParse(string s, out DomainName? name)
        {
            if (string.IsNullOrEmpty(s))
            {
                name = null;
                return false;
            }

            if (s == ".")
            {
                name = Root;
                return true;
            }

            var labels = new List<string>();

            int lastOffset = 0;
            int nextOffset;

            while ((nextOffset = s.IndexOfWithQuoting('.', lastOffset)) != -1)
            {
                if (TryParseLabel(s[lastOffset..nextOffset], out string label))
                {
                    labels.Add(label!);
                    lastOffset = nextOffset + 1;
                }
                else
                {
                    name = null;
                    return false;
                }
            }

            if (s.Length == lastOffset)
            {
                // empty label --> name ends with dot
            }
            else if (TryParseLabel(s[lastOffset..], out string label))
            {
                labels.Add(label!);
            }
            else
            {
                name = default;
                return false;
            }

            if (labels.Sum(l => l.Length) + labels.Count > 255 || labels.Any(l => l.Length == 0))
            {
                name = null;
                return false;
            }

            name = new DomainName(labels.ToArray());
            return true;
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool asAbsoluteFqdn)
        {
            _toString ??= string.Join(".", _labels.Select(x => x.ToMasterfileLabelRepresentation(true)));
            return asAbsoluteFqdn ? _toString + "." : _toString;
        }

        private static bool TryParseLabel(string s, out string label)
        {
            try
            {
                s = s.FromMasterfileLabelRepresentation();

                if (s.Length > 63)
                {
                    label = null;
                    return false;
                }

                if (s.Any(c => c > 127))
                {
                    s = _idnParser.GetAscii(s);
                }

                label = s;
                return true;
            }
            catch
            {
                label = null;
                return false;
            }
        }
    }
}


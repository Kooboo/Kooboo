using System;
using System.Linq;
using Kooboo.IndexedDB.Query;

namespace Kooboo.IndexedDB.Condition.Expression
{
    public class ValueNode : Node
    {
        public ValueNode() { }
        public ValueNode(string value, bool sureToBeString)
        {
            Raw = value;
            if (value != null && !sureToBeString)
            {
                var mValue = value.ToLower();

                if (mValue == "true" || mValue == "false")
                {
                    Type = typeof(bool);
                    Value = bool.Parse(mValue);
                    return;
                }

                if (decimal.TryParse(value, out var number))
                {
                    Type = typeof(decimal);
                    Value = number;
                    return;
                }

                if (DateTime.TryParse(value, out var datetime))
                {
                    Type = typeof(DateTime);
                    Value = datetime;
                    return;
                }

                if (value.ToLower() == "null")
                {
                    Value = null;
                    return;
                }
            }

            Value = value;
            Type = typeof(string);
        }

        public override NodeType NodeType => NodeType.Value;

        public string Raw { get; set; }
        public object Value { get; set; }
        public byte[] ValueBytes { get; set; }
        public Type Type { get; set; }
        public int Length { get; set; }
        public DateTimeScope TimeScope { get; set; }

        public bool Boolean
        {
            get
            {
                if (ValueBytes != null)
                {
                    return ValueBytes.Any(a => a > 0);
                }

                return Value != default;
            }
        }
    }
}
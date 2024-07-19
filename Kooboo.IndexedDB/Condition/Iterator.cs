using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.IndexedDB.Condition.Expression;
using Kooboo.IndexedDB.Query;

namespace Kooboo.IndexedDB.Condition
{
    public class Iterator
    {
        static readonly char[] _trimStartChars = new[] { '\t', '\r', '\n', ' ' };
        static readonly string[] _orSymbols = new[] { "||", "or", "|", };
        static readonly string[] _andSymbols = new[] { "and", "&&", "&", };
        static readonly char[] _comparerStartSymbols = new char[] { '>', '=', '<', '!' };
        static readonly char[] _noWrapStringEndSymbols = new char[] { ' ', '>', '<', '=', '!', '&', '|', ')' };

        readonly static Dictionary<string, Comparer> _comparerMapping = new Dictionary<string, Comparer>
        {
            { "startwith",Comparer.StartWith },
            { "contains",Comparer.Contains },
            { ">=",Comparer.GreaterThanOrEqual },
            { "<=",Comparer.LessThanOrEqual },
            { "==",Comparer.EqualTo },
            { "!=",Comparer.NotEqualTo },
            { "<>",Comparer.NotEqualTo },
            { "=",Comparer.EqualTo },
            { ">",Comparer.GreaterThan },
            { "<",Comparer.LessThan },
        };

        public string Raw { get; private set; }

        public int Position { get; private set; }

        public char Current
        {
            get
            {
                if (Position > Raw.Length - 1) throw new ConditionParseException(Position);
                return Raw[Position];
            }
        }

        public bool End => Position >= Raw.Length;

        public bool HasLeftGap
        {
            get
            {
                if (Position == 0) return false;
                return Raw[Position - 1] == ' ';
            }
        }

        public Iterator(string raw)
        {
            Raw = raw;
        }

        public bool Next(int charNumber = 1)
        {
            Position += charNumber;
            return Position <= Raw.Length - 1;
        }

        public void TrimStart()
        {
            while (!End)
            {
                if (_trimStartChars.Any(a => a == Current))
                {
                    if (!Next()) break;
                }
                else break;
            }
        }

        public bool StartWith(string s)
        {
            if (s.Length + Position > Raw.Length) return false;

            for (int i = 0; i < s.Length; i++)
            {
                if (char.ToLower(Raw[Position + i]) != char.ToLower(s[i])) return false;
            }

            return true;
        }

        public bool IsOperand()
        {
            if (Current == '&' || Current == '|') return true;
            if (!HasLeftGap) return false;
            return StartWith("and ") || StartWith("or ");
        }

        public Operand ExtractOperand()
        {
            var or = _orSymbols.FirstOrDefault(f => StartWith(f));
            var and = or == null ? _andSymbols.FirstOrDefault(f => StartWith(f)) : null;
            Next((or ?? and).Length);
            TrimStart();
            return or == null ? Operand.And : Operand.Or;
        }

        public bool IsComparer()
        {
            if (_comparerStartSymbols.Any(a => Current == a)) return true;
            if (!HasLeftGap) return false;
            return StartWith("startwith ") || StartWith("contains ");
        }

        public Comparer ExtractComparer()
        {
            var kv = _comparerMapping.First(w => StartWith(w.Key));
            Next(kv.Key.Length);
            TrimStart();
            return kv.Value;
        }

        public bool IsValue() => !IsComparer() && !IsOperand();

        public ValueNode ExtractValue(bool sureToBeString = false)
        {
            char? wrapSymbol = null;

            var valueBuilder = new StringBuilder();

            if (Current == '\"')
            {
                wrapSymbol = '\"';
                Next();
            }

            if (Current == '\'')
            {
                wrapSymbol = '\'';
                Next();
            }

            while (true)
            {
                if (!wrapSymbol.HasValue && _noWrapStringEndSymbols.Any(a => Current == a)) break;

                if (wrapSymbol == Current && Raw[Position - 1] != '\\')
                {
                    Next();
                    break;
                }

                valueBuilder.Append(Current);

                if (!Next())
                {
                    if (wrapSymbol.HasValue) throw new ConditionParseException(Position);
                    break;
                }
            }

            TrimStart();
            return new ValueNode(valueBuilder.ToString(), wrapSymbol.HasValue || sureToBeString);
        }
    }
}

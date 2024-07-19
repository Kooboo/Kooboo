//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text;

namespace Kooboo.Mail.Imap.Commands.FetchCommand.ResponseItem
{
    public class StructureBuilder
    {
        private StringBuilder _builder = new StringBuilder();

        public StructureBuilder StartBracket()
        {
            _builder.Append("(");
            return this;
        }

        public StructureBuilder SpaceNBracket()
        {
            _builder.Append(" (");
            return this;
        }

        public StructureBuilder EndBracket()
        {
            _builder.Append(")");
            return this;
        }

        public StructureBuilder AppendQuoted(string str)
        {
            _builder.Append("\"").Append(str).Append("\"");
            return this;
        }

        public StructureBuilder SpaceNQuoted(string str)
        {
            return Append(" ").AppendQuoted(str);
        }

        public StructureBuilder Append(string str)
        {
            _builder.Append(str);
            return this;
        }

        public StructureBuilder Append(object obj)
        {
            _builder.Append(obj.ToString());
            return this;
        }

        public StructureBuilder AppendNil()
        {
            _builder.Append(" NIL");
            return this;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}

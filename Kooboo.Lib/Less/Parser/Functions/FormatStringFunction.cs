namespace dotless.Core.Parser.Functions
{
    using System;
    using System.Linq;
    using dotless.Core.Exceptions;
    using Infrastructure;
    using Infrastructure.Nodes;
    using Tree;

    public class FormatStringFunction : Function
    {
        protected override Node Evaluate(Env env)
        {
            WarnNotSupportedByLessJS("formatstring(string, args...)", null, @" You may want to consider using string interpolation (""@{variable}"") which does the same thing and is supported.");

            if (Arguments.Count == 0)
                return new Quoted("", false);

            Func<Node, string> unescape = n => n is Quoted ? ((Quoted)n).UnescapeContents() : n.ToCSS(env);

            var format = unescape(Arguments[0]);

            var args = Arguments.Skip(1).Select(unescape).ToArray();

            string result;

            try
            {
                result = string.Format(format, args);
            }
            catch (FormatException e)
            {
                throw new ParserException(string.Format("Error in formatString :{0}", e.Message), e);
            }

            return new Quoted(result, false);
        }
    }
}
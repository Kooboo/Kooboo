using System.Collections.Generic;
using System.Linq;
using Esprima.Ast;

namespace Kooboo.Lib.Utilities
{
    public class JsStringParser
    {

        public List<Esprima.Ast.Literal> GetLabels(string JsBlock)
        {
            var prog = new Esprima.JavaScriptParser().ParseScript(JsBlock);

            List<Esprima.Ast.Literal> result = new List<Esprima.Ast.Literal>();

            foreach (var item in prog.Body)
            {
                AppendLable(item.ChildNodes, result);
            }

            return result;
        }


        public List<Esprima.Ast.Literal> GetStringLabels(string JsBlock)
        {
            var lables = GetLabels(JsBlock);

            return lables.Where(o => o.TokenType == Esprima.TokenType.StringLiteral).ToList();
        }


        private void AppendLable(ChildNodes nodes, List<Esprima.Ast.Literal> result)
        {

            foreach (var item in nodes)
            {
                if (item == null)
                {
                    continue;
                }

                if (item is Esprima.Ast.Literal)
                {
                    var lable = item as Esprima.Ast.Literal;

                    if (lable.TokenType == Esprima.TokenType.StringLiteral)
                    {

                    }

                    result.Add(lable);
                }

                if (item.ChildNodes.Any())
                {
                    AppendLable(item.ChildNodes, result);
                }

            }
        }



    }
}

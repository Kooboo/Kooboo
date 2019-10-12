using Jint.Parser.Ast;
using System.Collections.Generic;

namespace Jint.Parser
{
    public interface IFunctionDeclaration : IFunctionScope
    {
        Identifier Id { get; }
        IEnumerable<Identifier> Parameters { get; }
        Statement Body { get; }
        bool Strict { get; }
    }
}
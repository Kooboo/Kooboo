﻿namespace dotless.Core.Parser.Tree
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure;
    using Infrastructure.Nodes;
    using Plugins;

    public class Expression : Node
    {
        public NodeList Value { get; set; }

        public Expression(IEnumerable<Node> value)
        {
            if (value is NodeList)
                Value = value as NodeList;
            else
                Value = new NodeList(value);
        }

        public override Node Evaluate(Env env)
        {
            if (Value.Count > 1)
                return new Expression(new NodeList(Value.Select(e => e.Evaluate(env)))).ReducedFrom<Node>(this);

            if (Value.Count == 1)
                return Value[0].Evaluate(env).ReducedFrom<Node>(this);

            return this;
        }

        public override void AppendCSS(Env env)
        {
            env.Output.AppendMany(Value, " ");
        }

        public override void Accept(IVisitor visitor)
        {
            Value = VisitAndReplace(Value, visitor);
        }
    }
}
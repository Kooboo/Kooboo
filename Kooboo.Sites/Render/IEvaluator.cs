//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;

namespace Kooboo.Sites.Render
{
    public interface IEvaluator
    {
        EvaluatorResponse Evaluate(Node node, EvaluatorOption options);
    }
}
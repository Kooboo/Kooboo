//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render
{
  public  interface IEvaluator
    {
        EvaluatorResponse Evaluate(Node node, EvaluatorOption options); 
 
    }
}

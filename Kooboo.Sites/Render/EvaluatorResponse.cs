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
    public class EvaluatorResponse
    {
        // continue next evaluator...
        public bool StopNextEvaluator { get; set; }

        public bool OmitTag { get; set; }

        public bool FakeHeader { get; set; }
  
        public List<IRenderTask> ContentTask { get; set; }

        public List<IRenderTask> AppendTask { get; set; }

        public List<IRenderTask> AttributeTask { get; set; }

        // place before open tag... 
        public List<IRenderTask> BindingTask
        {
            get; set;
        }

        public List<IRenderTask> EndBindingTask
        {
            get; set;
        }

        public void AddContent(IRenderTask task)
        {
            if (this.ContentTask == null)
            {
                this.ContentTask = new List<IRenderTask>(); 
            }
            this.ContentTask.Add(task); 
        } 
    }
}

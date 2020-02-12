//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.DataTrace;
using Kooboo.Sites.Render.RenderTask;
using Kooboo.Sites.ViewModel;

namespace Kooboo.Sites.Render
{
    public class BindingRenderTask : IRenderTask
    {

        readonly IDictionary<string, string> _addition;
        static char[] _nontraceableChars = new[] { '{', '\'', '"' };
        readonly string _path;
        readonly ITraceability _traceability;

        public BindingEndRenderTask BindingEndRenderTask { get; }


        BindingRenderTask(IDictionary<string, string> addition = null)
        {
            _addition = addition;
            BindingEndRenderTask = new BindingEndRenderTask();
        }

        public BindingRenderTask(string path, IDictionary<string, string> addition = null)
            : this(addition)
        {
            _path = path;
        }

        public BindingRenderTask(ITraceability traceability, IDictionary<string, string> addition = null)
           : this(addition)
        {
            _traceability = traceability;
        }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            string renderresult = this.Render(context);
            result.Add(new RenderResult() { Value = renderresult });
        }

        public virtual string Render(RenderContext context)
        {
            BindingEndRenderTask.Uid = Lib.Helper.StringHelper.GetUniqueBoundary();
            var traceability = _traceability ?? GetTraceabilityObject(context, out var fieldPath);
            var infoList = traceability.GetTraceInfo().Select(s => $"--{s.Key}={s.Value}");
            if (_addition != null) infoList = infoList.Union(_addition.Select(s => $"--{s.Key}={s.Value}"));
            return $@"
<!--#kooboo--source={traceability.Source.ToString()}{string.Join("", infoList)}--uid={BindingEndRenderTask.Uid}-->
";
        }


        public ITraceability GetTraceabilityObject(RenderContext context, out string fieldPath)
        {
            fieldPath = null;
            if (_path == null || _path.Any(a => _nontraceableChars.Contains(a))) return Nontraceable.Instance;
            var stacks = new Queue<string>(_path.Split('.'));

            if (stacks.Count > 0)
            {
                var obj = context.DataContext.GetValue(stacks.Dequeue());
                do
                {
                    if (obj is ITraceability)
                    {
                        fieldPath = string.Join(".", stacks);
                        return obj as ITraceability;
                    }
                    else if (obj is IDynamic && stacks.Count > 0)
                    {
                        obj = (obj as IDynamic).GetValue(stacks.Dequeue());
                        continue;
                    }
                    else if (obj is IDictionary<string, object> && stacks.Count > 0)
                    {
                        obj = (obj as IDictionary<string, object>)[stacks.Dequeue()];
                        continue;
                    }

                    break;
                } while (stacks.Count > 0);
            }

            return Nontraceable.Instance;
        }
    }
}

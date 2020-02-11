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
        readonly string _path;
        readonly string _uid = Guid.NewGuid().ToString("N");
        static char[] _nontraceableChars = new[] { '{', '\'', '"' };
        public BindingEndRenderTask BindingEndRenderTask;

        public BindingRenderTask(string path)
        {
            _path = path;
            BindingEndRenderTask = new BindingEndRenderTask(_uid);
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
            var traceability = GetTraceabilityObject(context, out var fieldPath);
            var infoList = traceability.GetTraceInfo().Select(s => $"--{s.Key}={s.Value}");
            return $@"
<!--#kooboo--source={traceability.Source}{string.Join("", infoList)}--uid={_uid}-->
";
        }


        public ITraceability GetTraceabilityObject(RenderContext context, out string fieldPath)
        {
            fieldPath = null;
            if (_path == null || _path.Any(a => _nontraceableChars.Contains(a))) return Nontraceable.Instance;
            var stacks = new Queue<string>(_path.Split('.'));

            if (stacks.Count > 0)
            {
                var jsValue = context.DataContext.GetValue(stacks.Dequeue());
                do
                {
                    if (jsValue == null) break;

                    if (jsValue is ITraceability)
                    {
                        fieldPath = string.Join(".", stacks);
                        return jsValue as ITraceability;
                    }

                    var prop = stacks.Dequeue();

                    if (jsValue is IDynamic)
                    {
                        jsValue = (jsValue as IDynamic).GetValue(prop);
                    }
                    else if (jsValue is IDictionary<string, object>)
                    {
                        jsValue = (jsValue as IDictionary<string, object>)[stacks.Dequeue()];
                    }
                    else break;

                } while (stacks.Count > 0);
            }

            return Nontraceable.Instance;
        }
    }
}

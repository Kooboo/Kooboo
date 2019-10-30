//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    internal class DataRenderTask : IRenderTask
    {
        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public Action<RenderContext> AssignData;

        private Dictionary<string, object> _data;

        public Dictionary<string, object> Data
        {
            get { return _data ?? (_data = new Dictionary<string, object>()); }
            set { _data = value; }
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            AssignData?.Invoke(context);
            if (_data != null)
            {
                context.DataContext.Push(_data);
            }
        }

        public string Render(RenderContext context)
        {
            AssignData?.Invoke(context);
            if (_data != null)
            {
                context.DataContext.Push(_data);
            }
            return null;
        }
    }
}
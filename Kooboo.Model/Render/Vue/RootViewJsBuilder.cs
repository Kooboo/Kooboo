using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Kooboo.Model.Render.Vue;

namespace Kooboo.Model.Render.Vue
{
    public class RootViewJsBuilder : IJsBuilder
    {
        private El _current;
        private List<El> _els = new List<El>();

        public RootViewJsBuilder(VueJsBuilderOptions options)
        {
            Options = options;
        }

        public VueJsBuilderOptions Options { get; }

        public IJsBuilder AddItem(object item)
        {
            var el = item as El;
            if (el != null)
            {
                _els.Add(el);
                _current = el;
                return this;
            }

            if (_current == null)
                throw new ViewParseException("Vue root element is required before building other elements");

            _current.Items.Add(item);

            return this;
        }

        public string Build()
        {
            var outer = new StringBuilder();

            foreach (El el in _els)
            {
                var builder = new InnerJsBuilder(outer);

                VueJsHelper.Build(builder, new object[] { el }.Union(el.Items), Options);

                if (outer.Length > 0)
                {
                    outer.AppendLine().AppendLine();
                }

                outer.Append("Vue.kExtend(")
                    .Append(builder.BuildWithBracket())
                    .Append(")");
            }

            outer.AppendLine().AppendLine().Append($"Vue.kExecute()");

            return outer.ToString();
        }
    }
}

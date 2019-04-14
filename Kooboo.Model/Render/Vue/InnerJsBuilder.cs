using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.Render.Vue
{
    public partial class InnerJsBuilder
    {
        public const string DirectRenderPrefix = "__";
        private const int IndentSpaces = 2;

        private Dictionary<string, List<Action<InnerJsBuilder>>> _items = new Dictionary<string, List<Action<InnerJsBuilder>>>();

        private int _indent;
        private bool _lineStart = true;
        private StringBuilder _outer;
        private StringBuilder _inner;

        public InnerJsBuilder(StringBuilder outer)
        {
            _outer = outer;
            _inner = new StringBuilder();
        }

        public ViewType ViewType { get; set; } = ViewType.Root;

        public InnerJsBuilder AddItem(string key, Action<InnerJsBuilder> render)
        {
            if (!key.StartsWith(DirectRenderPrefix))
            {
                if (!Renderers.ContainsKey(key))
                    throw new ArgumentException($"Renderer is not registered for \"{key}\"", nameof(key));
            }

            if (!_items.TryGetValue(key, out List<Action<InnerJsBuilder>> list))
            {
                list = new List<Action<InnerJsBuilder>>();
                _items.Add(key, list);
            }

            list.Add(render);

            return this;
        }

        public InnerJsBuilder Block(string js)
        {
            if (_outer.Length > 0)
            {
                _outer.AppendLine();
            }

            _outer.AppendLine(js);

            return this;
        }

        public InnerJsBuilder Indent()
        {
            _indent++;
            return this;
        }

        public InnerJsBuilder Append(string str)
        {
            if (_lineStart)
            {
                _inner.Append(String.Empty.PadRight(_indent * IndentSpaces));
            }

            _inner.Append(str);
            _lineStart = false;

            return this;
        }

        public InnerJsBuilder AppendLine()
        {
            _inner.AppendLine();
            _lineStart = true;

            return this;
        }

        public InnerJsBuilder AppendLine(string line)
        {
            return Append(line).AppendLine();
        }

        public InnerJsBuilder Unindent()
        {
            _indent--;
            return this;
        }

        public string Build()
        {
            InnerBuild();

            return _inner.ToString();
        }

        public string BuildWithBracket()
        {
            AppendLine("{").Indent();

            InnerBuild();

            Unindent().AppendLine().Append("}");

            return _inner.ToString();
        }

        private void InnerBuild()
        {
            int i = 0;
            foreach (var item in _items)
            {
                if (i > 0)
                {
                    this.AppendLine(",");
                }
                if (item.Key.StartsWith(DirectRenderPrefix))
                {
                    foreach (var render in item.Value)
                    {
                        render(this);
                    }
                }
                else
                {
                    Renderers[item.Key](this, item.Value);
                }
                i++;
            }
        }
    }
}

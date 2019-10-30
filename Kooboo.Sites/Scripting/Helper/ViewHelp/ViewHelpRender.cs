//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Scripting.Helper;
using System.Text;

namespace Kooboo.Sites.Scripting
{
    public class ViewHelpRender : RenderBase
    {
        public override string GetHeaderName()
        {
            return "kView";
        }

        public string Render(RenderContext context)
        {
            var tree = ViewHelpReader.GetTree();
            return Render(context, tree);
        }

        public override bool IsHelpDetail(RenderContext context)
        {
            var tag = context.Request.QueryString["kview"];
            return !string.IsNullOrEmpty(tag);
        }

        public override string RenderDetail(RenderContext context, Node node)
        {
            var kview = context.Request.QueryString["kview"];
            if (string.IsNullOrEmpty(kview) && node != null)
            {
                kview = node.Text;
            }
            StringBuilder builder = new StringBuilder();
            if (kview != null && ViewHelpReader.Models.ContainsKey(kview))
            {
                var model = ViewHelpReader.Models[kview];
                builder.Append(RenderDetailStart());
                builder.Append(RenderModelBase(model));
                builder.Append(RenderDetailEnd());
            }
            return builder.ToString();
        }

        public override string GetDefaultUrl(RenderContext context, Tree tree)
        {
            if (tree != null && tree.Nodes.Count > 0)
            {
                var node = tree.Nodes[0];
                var model = node.setting;
                return $"{GetBaseurl(context)}?kview={model.Name}";
            }
            return string.Empty;
        }
    }
}
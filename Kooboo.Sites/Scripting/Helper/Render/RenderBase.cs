//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using System.IO;
using Kooboo.Sites.Scripting.Helper;

namespace Kooboo.Sites.Scripting
{
    public class RenderBase
    {
        public virtual string GetHeaderName()
        {
            return string.Empty;
        }
        public virtual string Render(RenderContext context,Tree tree)
        {
            var node = GetFirstNode(tree);
            var detail = RenderDetail(context, node);

            return Render(context, tree, detail);

        }

        private Node GetFirstNode(Tree tree)
        {
            Node node = null;
            if (tree.Nodes.Count > 0)
            {
                node = tree.Nodes[0];
            }
            return node;
        }
        private string Render(RenderContext context, Tree tree,string detail)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetScript());

            builder.Append("<div class='head'>");
            builder.Append("<div class='head_innner'>");
            builder.Append("<div class='head_container'>");

            builder.AppendFormat("<h1 class='head_title'>{0}</h1>", GetHeaderName());
            builder.AppendFormat("<div id='menu_container' class='menu_container'><a class='menu_btn' href='javascript:;'><i></i></a><a id='open_in_tab' href='/_Admin/Development/KScript/Documentation' target='_blank'><i></i></a></div>");

            builder.Append("</div>");
            builder.Append("</div>");
            builder.Append("</div>");

            builder.Append("<div class='main_area'>");
            builder.Append("<div class='main_area_inner'>");

            builder.Append("<div class='container_box cell_layout side_l'>");

            //menu
            builder.Append("<div id='col_side' class='col_side'>");
            builder.AppendFormat("<div class='menu_box_primary' id='leftTree'></div>");
            builder.Append("</div>");

            //builder.AppendFormat("<div class='col_main'><iframe frameborder='0' src='{0}' id='iframe'></iframe></div>", GetDefaultUrl(context, tree));
            builder.AppendFormat("<div class='col_main'>{0}</div>", detail);

            builder.Append("</div>");

            builder.Append("</div>");
            builder.Append("</div>");

            var treeviewData = string.Format(@"
                data:{0},
                onNodeSelected: function(event, node) {{
                    var src='{1}'+node.url;
                    window.location.href=src;
                 }},
            ", JsonHelper.Serialize(tree.Nodes), GetBaseurl(context));

            builder.AppendFormat("<script>{0}</script>",
                string.Format("$('#leftTree').treeview({{{0}}});", treeviewData));

            builder.AppendFormat("<script>{0}</script>", File.ReadAllText(Path.Combine(GetHelpPath(), "helpMenu.js")));

            return builder.ToString();
        }
        public virtual string RenderDetail(RenderContext context,Node node)
        {

            return string.Empty;
        }
        public virtual bool IsHelpDetail(RenderContext context)
        {
            return false;
        }

        /// <summary>
        /// get default selected class detail's url
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tree"></param>
        /// <returns></returns>
        public virtual string GetDefaultUrl(RenderContext context, Tree tree)
        {
            return string.Empty;
        }
        protected string GetBaseurl(RenderContext context)
        {
            var baseUrl = context.Request.Url;
            if (baseUrl.IndexOf("?") > -1)
                baseUrl = baseUrl.Split('?')[0];
            return baseUrl;
        }
        #region kscrip help style and script
        private string GetPath()
        {
            StringBuilder builder = new StringBuilder();
            var path = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.CombinePath(Kooboo.Data.AppSettings.RootPath, @"_Admin\Scripts");
            return path;
        }

        public string GetHelpPath()
        {
            return Path.Combine(GetPath(), "HelpJs");
        }

        public string GetJqueryPath()
        {
            return Path.Combine(GetPath(), "lib");
        }

        private string GetScript()
        {
            var path = GetHelpPath();
            var jqueryPath = GetJqueryPath();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("<style>{0}</style>", File.ReadAllText(Path.Combine(path, "bootstrap-treeview.css")));
            builder.AppendFormat("<style>{0}</style>", File.ReadAllText(Path.Combine(path, "help.css")));

            builder.AppendFormat("<script>{0}</script>", File.ReadAllText(Path.Combine(jqueryPath, "jquery.min.js")));
            builder.AppendFormat("<script>{0}</script>", File.ReadAllText(Path.Combine(path, "bootstrap-treeview.js")));

            return builder.ToString();

        }
        #endregion

        protected string RenderModelBase(SettingBase model)
        {
            StringBuilder builder = new StringBuilder();
            //DisplayName
            builder.AppendFormat("<p><strong>{0}</strong></p>", model.Name);

            //sample
            // builder.AppendFormat("<p><strong>{0}</strong>:</p>", "Sample");
            if (!string.IsNullOrEmpty(model.Description))
            {
                builder.AppendFormat("<p>{0}</p>", model.Description);
            }
            if(model is ExampleSetting)
            {
                builder.AppendFormat("<textarea style='display:none' disabled='true' class='code'>{0}</textarea>", (model as ExampleSetting).Example);
                builder.AppendFormat("<pre><code class='language-markup'></code></pre>");
            }
            
            //builder.AppendFormat("<textarea class='language-JavaScript'>{0}</textarea>", model.Example);
            return builder.ToString();
        }
        protected string RenderDetailStart()
        {
            StringBuilder builder = new StringBuilder();
            var path = GetHelpPath();
            builder.AppendFormat("<style>{0}</style>", File.ReadAllText(Path.Combine(path, "prism.css")));
            builder.AppendFormat("<script>{0}</script>", File.ReadAllText(Path.Combine(path, "prism.js")));
            builder.AppendFormat("<script>{0}</script>", File.ReadAllText(Path.Combine(path, "prismfill.js")));

            builder.Append("<div class='main_bd'>");
            builder.Append("<div class='article_box'>");
            builder.Append("<div class='inner'>");
            builder.Append("<div id='content' class='news_content'>");
            return builder.ToString();
        }
        protected string RenderDetailEnd()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("</div>");
            builder.Append("</div>");
            builder.Append("</div>");
            builder.Append("</div>");
            return builder.ToString();
        }
    }

}

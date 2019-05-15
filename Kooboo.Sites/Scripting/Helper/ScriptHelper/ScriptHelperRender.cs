//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Sites.Scripting.Helper;
using System.IO;

namespace Kooboo.Sites.Scripting.Helper.ScriptHelper
{
    public class ScriptHelperRender : RenderBase
    {
        public override string GetHeaderName()
        {
            return "kScript";
        }
        public string Render(RenderContext context)
        {
            var tree = ScriptHelperReader.GetTree();
            return Render(context, tree);
        }
        public override bool IsHelpDetail(RenderContext context)
        {
            var name = context.Request.QueryString["name"];
            return !string.IsNullOrEmpty(name);
        }

        public override string RenderDetail(RenderContext context,Node node)
        {
            var name = context.Request.QueryString["name"];
            if(string.IsNullOrEmpty(name) && node != null)
            {
                name = node.Text;
            }
            var param = context.Request.QueryString["param"];
            var methodName= context.Request.QueryString["method"];
            if (name==null||!ScriptHelperReader.Settings.ContainsKey(name.ToLower()))
                return string.Empty;

            var setting = ScriptHelperReader.Settings[name.ToLower()] as KScriptSetting;
            if(string.IsNullOrEmpty(methodName))
               return RenderSettingHtml(context, setting,name);
            var method = setting.Methods.Find(m => m.IsSameMethod(methodName,param));
            
            return RenderMethodHtml(context, method);  
        }

        public string RenderMethodHtml(RenderContext context, Method model)
        {
            if (model == null) return string.Empty;
            StringBuilder builder = new StringBuilder();
            builder.Append(RenderDetailStart());
            builder.Append(RenderModelBase(model));
            builder.AppendFormat("<p><strong>{0}</strong></p>", "Parameter");
            if (model.Params.Count == 0)
            {
                builder.AppendFormat("<p>{0}</p>", "");
            }
            else
            {
                builder.AppendFormat("<table><thead><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr></thead><tbody>", "Name", "Type", "Desc");
                foreach (var para in model.Params)
                {
                    var type = para.Type;
                    if (para.Type!=null && ScriptHelperReader.Settings.ContainsKey(para.Type.ToLower()))
                    {
                        type = string.Format("<a href='{1}'>{0}</a>", para.Type,DocumentHelper.GetTypeUrl(para.Type));
                    }
                    builder.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", DocumentHelper.LowerCaseFirstChar(para.Name), type, para.Description);
                }
                builder.Append("</tbody></table>");
            }

            builder.AppendFormat("<p><strong>{0}</strong></p>", "Return Type");
            if (model.ReturnType!=null && ScriptHelperReader.Settings.ContainsKey(model.ReturnType.ToLower()))
            {
                var url = GetBaseurl(context)+DocumentHelper.GetTypeUrl(model.ReturnType);
                builder.AppendFormat("<a href='{0}'>{1}</a>",url ,GetTypeName(model.ReturnType));
            }
            else
            {
                builder.Append(model.ReturnType);
            }

            builder.Append(RenderDetailEnd());
            return builder.ToString();
        }

        public string RenderSettingHtml(RenderContext context, KScriptSetting setting,string name)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RenderDetailStart());

            builder.Append(RenderModelBase(setting));

            var baseUrl = GetBaseurl(context);
            
            if (setting.Props.Count > 0)
            {
                builder.AppendFormat("<p><strong>{0}</strong></p>", "Properties");
                builder.AppendFormat("<table><thead><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr></thead><tbody>", "Name", "Type", "Desc");

                foreach (var prop in setting.Props)
                {

                    var propStr = string.Empty;

                    if (prop.Type!=null && ScriptHelperReader.Settings.ContainsKey(prop.Type.ToLower()))
                    {
                        var url = DocumentHelper.GetTypeUrl(prop.Type);

                        var type = string.Format("<a href='{0}'>{1}</a>", url, GetTypeName(prop.Type));
                        propStr = string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", DocumentHelper.LowerCaseFirstChar(prop.Name), type, prop.Description);
                    }
                    else
                    {
                        propStr = string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", DocumentHelper.LowerCaseFirstChar(prop.Name),prop.Type, prop.Description);
                    }
                   
                    builder.Append(propStr);

                }
                builder.Append("</tbody></table>");
            }

            if (setting.Methods.Count > 0)
            {
                builder.AppendFormat("<p><strong>{0}</strong></p>", "Methods");
                builder.AppendFormat("<table><thead><tr><th>{0}</th><th>{1}</th><th>{2}</th></tr></thead><tbody>", "Name","Return", "Desc");

                var methods = setting.Methods.OrderBy(m => m.Name).ToList();
                foreach (var method in methods)
                {
                    var paramBuilder = new StringBuilder();
                    foreach(var param in method.Params)
                    {
                        paramBuilder.Append(param.Name);
                    }
                    //method rul
                    var url = baseUrl+ DocumentHelper.GetMethodUrl(name, method.Name, paramBuilder.ToString());
                    var methodWithParam = string.Format("<a href='{0}'>{1}</a>", url, GetMethodWithParams(method));

                    //return type url
                    var type = method.ReturnType;
                    if (method.ReturnType!=null && ScriptHelperReader.Settings.ContainsKey(method.ReturnType.ToLower()))
                    {
                        var returnTypeUrl = baseUrl + DocumentHelper.GetTypeUrl(method.ReturnType);
                        type=string.Format("<a href='{0}'>{1}</a>", returnTypeUrl, GetTypeName(method.ReturnType));
                    }

                    var str = string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td></tr>", methodWithParam, type, method.Description);
                    builder.Append(str);
                }
                builder.Append("</tbody></table>");
            }

            builder.Append(RenderDetailEnd());
            return builder.ToString();
        }
        private string GetMethodWithParams(Method method)
        {
            var paras = method.Params;
            StringBuilder builder = new StringBuilder();
            foreach (var para in paras)
            {
                if (!string.IsNullOrEmpty(para.Type))
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append(string.Format("{0} {1}", para.Type, para.Name));
                }
            }
            return string.Format("{0}({1})", DocumentHelper.LowerCaseFirstChar(method.Name), builder.ToString());
        }

        private string GetTypeName(string type)
        {
            if(type!=null && ScriptHelperReader.Settings.ContainsKey(type.ToLower()))
            {
                return ScriptHelperReader.Settings[type.ToLower()].Name;
            }
            return type;
        }

    }
}

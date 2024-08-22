using Kooboo.Data.Context;
using Kooboo.Mail.Extension;
using Kooboo.Sites.Render;
using Kooboo.Sites.Render.Evaluators;
using Kooboo.Web.Api.Implementation.Mails.Modules.Render;

namespace Kooboo.Sites.ScriptModules.Render.View
{
    public static class MailViewRender
    {

        public static string Render(MailModuleContext context, MailResourceType resType, ModuleFileInfo FileInfo)
        {

            if (FileInfo == null)
            {
                var file = context.RenderContext.Request.GetValue("file");

                if (file == null)
                {
                    FileInfo = GetStartView(context, resType);
                }
                else
                {
                    FileInfo = GetFileInfo(context, resType, file);
                }

            }

            if (FileInfo != null)
            {
                return Render(context, FileInfo);
            }

            return null;
        }


        public static string Render(MailModuleContext context, ModuleFileInfo fileinfo)
        {

            var options = GetViewOption(context.RenderContext);
            options.OwnerObjectId = context.Module.Id;
            options.Additional = fileinfo.FullDiskFileName;

            var renderplan = Getplan(fileinfo, options);

            if (renderplan == null)
            {
                return null;
            }

            try
            {
                var result = RenderHelper.Render(renderplan, context.RenderContext);

                if (result.IndexOf("<script", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    result = MergetScriptTag(context, result);
                }

                return result;
            }
            catch (Exception)
            {

            }

            return null;

        }

        public static string MergetScriptTag(MailModuleContext context, string result)
        {
            var scriptTag = MailModuleRenderTask.instance.GetScriptHeader(context);

            scriptTag = "<script>" + scriptTag + "</script>";

            var firstScript = result.IndexOf("<script", StringComparison.OrdinalIgnoreCase);

            if (firstScript != -1)
            {
                var firstPart = result.Substring(0, firstScript);

                var lastpart = result.Substring(firstScript);

                result = firstPart + scriptTag + lastpart;
            }

            return result;
        }

        private static Dictionary<Guid, VersionResul> versionResult { get; set; } = new Dictionary<Guid, VersionResul>();

        public static List<IRenderTask> Getplan(ModuleFileInfo fileInfo, EvaluatorOption option)
        {
            var text = System.IO.File.ReadAllText(fileInfo.FullDiskFileName);
            if (!string.IsNullOrEmpty(text))
            {
                return Kooboo.Sites.Render.RenderEvaluator.Evaluate(text, option);
            }
            return new List<IRenderTask>();
        }

        public static ModuleFileInfo GetStartView(MailModuleContext context, MailResourceType resType)
        {
            var disk = DiskHandler.FromMailModuleContext(context, resType.Name);

            List<string> startNames = new List<string>();

            var allfiles = disk.FolderFiles("");

            foreach (var item in allfiles)
            {
                var name = item.Name.ToLower();
                if (name.Contains("index.") || name.Contains("default.") || name.Contains("start.") || name.Contains("home.") || name.Contains("main."))
                {
                    return item;
                }
            }

            foreach (var item in allfiles)
            {
                var name = item.Name.ToLower();
                if (name.Contains("index") || name.Contains("default") || name.Contains("start") || name.Contains("home") || name.Contains("main"))
                {
                    return item;
                }
            }

            return null;
        }

        public static ModuleFileInfo GetFileInfo(MailModuleContext context, MailResourceType resType, string file)
        {
            if (file == null)
            {
                return null;
            }
            file = file.Replace("/", "\\");

            var diskRoot = DiskHandler.FromMailModuleContext(context, "");

            var info = diskRoot.GetFileInfo("", file);

            if (info != null)
            {
                return info;
            }

            var disk = DiskHandler.FromMailModuleContext(context, resType.Name);
            var fileInfo = disk.GetFileInfo("", file);
            return fileInfo;
        }

        public static EvaluatorOption GetViewOption(RenderContext context)
        {
            EvaluatorOption option = new EvaluatorOption();

            option.RenderHeader = false;
            option.Evaluators = EvaluatorList;

            return option;
        }

        private static List<IEvaluator> _list;
        public static List<IEvaluator> EvaluatorList
        {
            get
            {
                if (_list == null)
                {
                    _list = new List<IEvaluator>();

                    _list.Add(new EnvClientEvaluator());

                    // _list.Add(new PlaceHolderEvaluator());
                    // _list.Add(new SiteLayoutEvaluator());
                    _list.Add(new QueryEvaluator());
                    _list.Add(new DefineEvaluator());

                    _list.Add(new RepeaterEvaluator());
                    _list.Add(new ConditionEvaluator());
                    _list.Add(new SystemEvaluator());
                    // _list.Add(new ForEvaluator());
                    _list.Add(new AttributeEvaluator());

                    // _list.Add(new UrlEvaluator());
                    _list.Add(new MailModuleUrlEvaluator());
                    _list.Add(new MailModuleSrcEvaluator());

                    // _list.Add(new LocalCacheEvaluator());
                    _list.Add(new LabelEvaluator());
                    _list.Add(new HtmlBlockEvaluator());
                    _list.Add(new OmitTagEvaluator());
                    _list.Add(new OmitOuterTagEvaluator());
                    _list.Add(new ContentEvaluator());
                    _list.Add(new ContentBracketEvaluator());

                    _list.Add(new ComponentEvaluator());
                    _list.Add(new SlotEvaluator());
                    // _list.Add(new HeaderEvaluator()); 

                    _list.Add(new FormEvaluator());

                    _list.Add(new CommandEvaluator());
                    _list.Add(new KConfigContentEvaluator());
                    //_list.Add(new VersionEvaluator());
                }
                return _list;
            }
        }

    }

}

//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using dotless.Core;
using System.Collections.Generic;
using System;
using dotless.Core.Parser;
using dotless.Core.Importers;
using dotless.Core.Parser.Tree;
using Kooboo.Sites.Extensions;
using System.Collections.Concurrent;
using Kooboo.Data.Models;
using Kooboo.Data.Events;
using Kooboo.Events;
using System.IO;

namespace Kooboo.Sites.Engine
{
    public class Less : IEngine
    {
        public string Name { get { return "less"; } }

        public bool KeepTag { get { return true; } }

        public string Extension => "less";

        public bool IsScript => false;

        public bool IsStyle => true;

        readonly ConcurrentDictionary<WebSite, LessEngine> _lessEngines = new ConcurrentDictionary<WebSite, LessEngine>();

        // Less Css..   
        public string Execute(RenderContext context, string input)
        {
            return _lessEngines.GetOrAdd(context.WebSite, _ =>
            {
                var engine = new LessEngine();
                engine.Parser.Importer = new KLessImporter(context.WebSite);
                return engine;
            }).Parse(input ?? string.Empty);
        }

        public void RemoveCache(WebSite webSite)
        {
            _lessEngines.TryRemove(webSite, out _);
        }
    }

    class KLessImporter : IImporter
    {
        public List<string> Imports { get; set; } = new List<string>();

        public Func<Parser> Parser { get; set; }

        protected readonly List<string> _paths = new List<string>();
        readonly WebSite _webSite;

        public KLessImporter(WebSite webSite)
        {
            _webSite = webSite;
        }

        public string AlterUrl(string url, List<string> pathList)
        {
            return url;
        }

        public List<string> GetCurrentPathsClone()
        {
            return new List<string>(_paths);
        }

        public ImportAction Import(Import import)
        {
            if (string.IsNullOrWhiteSpace(import.Path)) return ImportAction.ImportNothing;
            var extension = Path.GetExtension(import.Path)?.ToLower() ?? "css";
            Imports.Add(import.Path);

            switch (extension)
            {
                case ".less":
                    return ImportLess(import);
                case ".css":
                    return ImportCss(import);
                case ".js":
                    return ImportJs(import);
                default:
                    return ImportAction.ImportNothing;
            }
        }

        private ImportAction ImportJs(Import import)
        {
            var code = _webSite.SiteDb().Code.GetByNameOrId(Path.GetFileNameWithoutExtension(import.Path));

            if (code == null)
            {
                try
                {
                    code = _webSite.SiteDb().Code.GetByUrl(Path.GetFileNameWithoutExtension(import.Path));
                }
                catch (Exception)
                {
                }
            }

            if (code == null) return ImportAction.LeaveImport;

            RenderContext context = new RenderContext
            {
                WebSite = _webSite
            };

            var source = Scripting.Manager.ExecuteCode(context, code.Body);
            return ImportLess(import, source);
        }

        private ImportAction ImportCss(Import import)
        {
            var style = GetStyle(import);
            if (style == null) return ImportAction.LeaveImport;
            import.InnerContent = style.Body;
            return ImportAction.ImportCss;
        }

        private ImportAction ImportLess(Import import)
        {
            var style = GetStyle(import);
            if (style == null) return ImportAction.LeaveImport;
            return ImportLess(import, style.Source);
        }

        private ImportAction ImportLess(Import import, string source)
        {
            _paths.Add(import.Path);

            try
            {
                import.InnerRoot = Parser().Parse(source, import.Path);
            }
            catch (Exception)
            {
                Imports.Remove(import.Path);
                return ImportAction.ImportNothing;
            }
            finally
            {
                _paths.RemoveAt(_paths.Count - 1);
            }

            return ImportAction.ImportLess;
        }

        private Models.Style GetStyle(Import import)
        {
            var style = _webSite.SiteDb().Styles.GetByNameOrId(import.Path);

            if (style == null)
            {
                try
                {
                    style = _webSite.SiteDb().Styles.GetByUrl(import.Path);
                }
                catch (Exception)
                {
                }
            }

            return style;
        }
    }

    class WebSiteChangeLessHandler : IHandler<WebSiteChange>
    {
        public void Handle(WebSiteChange theEvent, RenderContext context)
        {
            var lessEngine = Manager.Get("less");
            if (lessEngine == null) return;
            (lessEngine as Less).RemoveCache(theEvent.WebSite);
        }
    }

}

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

namespace Kooboo.Sites.Engine
{
    public class Less : IEngine
    {
        public string Name { get { return "less"; } }

        public bool KeepTag { get { return true; } }

        public string Extension => "less";

        public bool IsScript => false;

        public bool IsStyle => true;

        // Less Css..   
        public string Execute(RenderContext context, string input)
        {
            LessEngine.Instance.Parser.Importer = new KLessImporter(context);
            return LessEngine.Instance.Parse(input);
        }
    }

    class KLessImporter : IImporter
    {
        public List<string> Imports { get; set; } = new List<string>();

        public Func<Parser> Parser { get; set; }

        protected readonly List<string> _paths = new List<string>();
        readonly RenderContext _context;

        public KLessImporter(RenderContext context)
        {
            _context = context;
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
            var style = _context.WebSite.SiteDb().Styles.GetByNameOrId(import.Path);

            if (style == null)
            {
                try
                {
                    style = _context.WebSite.SiteDb().Styles.GetByUrl(import.Path);
                }
                catch (Exception)
                {
                }
            }

            if (style == null) return ImportAction.LeaveImport;

            var extension = style.Extension?.ToLower() ?? "css";
            Imports.Add(import.Path);

            switch (extension)
            {
                case "less":
                    _paths.Add(import.Path);

                    try
                    {
                        import.InnerRoot = Parser().Parse(style.Source, import.Path);
                    }
                    catch (Exception)
                    {
                        Imports.Remove(import.Path);
                    }
                    finally
                    {
                        _paths.RemoveAt(_paths.Count - 1);
                    }

                    return ImportAction.ImportLess;
                case "css":
                    import.InnerContent = style.Body;
                    return ImportAction.ImportCss;
                default:
                    return ImportAction.ImportNothing;
            }
        }
    }
}

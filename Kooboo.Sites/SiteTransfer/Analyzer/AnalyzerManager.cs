//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.SiteTransfer.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.SiteTransfer
{
    public static class AnalyzerManager
    {
        private static List<ITransferAnalyzer> _listofanalyzer;

        public static List<ITransferAnalyzer> getAnalyzers()
        {
            if (_listofanalyzer == null)
            {
                _listofanalyzer = new List<ITransferAnalyzer>();
                // _listofanalyzer.Add(new LinkAnalyzer());
                _listofanalyzer.Add(new CSSAnalyzer());
                _listofanalyzer.Add(new EmbeddedAnalyzer());
                _listofanalyzer.Add(new ImageAnalyzer());
                _listofanalyzer.Add(new ScriptAnalyzer());
                _listofanalyzer.Add(new InlineAnalyzer());
            }
            return _listofanalyzer;
        }

        /// <summary>
        /// Execute all the analyzer.
        /// </summary>
        public static AnalyzerContext Execute(AnalyzerContext context)
        {
            var allAnalyzer = getAnalyzers();

            foreach (var item in allAnalyzer)
            {
                item.Execute(context);
            }

            AppendRemoveBaseHrefChange(context);
            if (context.Changes.Count > 0)
            {
                context.HtmlSource = ParseChanges(context.HtmlSource, context.Changes);
            }
            return context;
        }

        private static void AppendRemoveBaseHrefChange(AnalyzerContext context)
        {
            var basetag = context.Dom.documentElement.getOneElementByTagName("base");

            if (basetag != null)
            {
                var endindx = basetag.location.openTokenEndIndex;
                if (basetag.location.endTokenEndIndex > endindx)
                {
                    endindx = basetag.location.endTokenEndIndex;
                }
                context.Changes.Add(new AnalyzerUpdate() { StartIndex = basetag.location.openTokenStartIndex, EndIndex = endindx, NewValue = string.Empty });
            }
        }

        public static string ParseChanges(string input, List<AnalyzerUpdate> changes)
        {
            changes = changes.Where(o => o != null && o.StartIndex > 0 && o.EndIndex > 0).OrderBy(o => o.StartIndex).ToList();

            string result = string.Empty;

            int currentindex = 0;
            int length = input.Length;
            StringBuilder sb = new StringBuilder();

            List<Replacer> replace = new List<Replacer>();

            foreach (var item in changes)
            {
                if (item.StartIndex == -1 && item.EndIndex == -1)
                {
                    result += "\r\n" + item.NewValue;
                }
                else
                {
                    if (item.IsReplace)
                    {
                        string sub = input.Substring(item.StartIndex, item.EndIndex - item.StartIndex);
                        replace.Add(new Replacer() { WholeString = sub, oldValue = item.OldValue, NewValue = item.NewValue });
                    }
                    else
                    {
                        sb.Append(input.Substring(currentindex, item.StartIndex - currentindex));
                        sb.Append(item.NewValue);
                        currentindex = item.EndIndex + 1;
                    }
                }
            }

            if (currentindex < length - 1)
            {
                sb.Append(input.Substring(currentindex, length - currentindex));
            }
            string returnresult = sb.ToString() + result;

            if (replace.Count() > 0)
            {
                foreach (var item in replace.GroupBy(o => o.WholeString))
                {
                    var oldtext = item.Key;
                    string newtext = oldtext;
                    foreach (var onereplace in item.ToList())
                    {
                        newtext = newtext.Replace(onereplace.oldValue, onereplace.NewValue);
                    }

                    returnresult = returnresult.Replace(oldtext, newtext);
                }
            }

            return returnresult;
        }

        public static AnalyzerContext Execute(string htmlSource, string baseUrl, Guid objectId, byte objectType, DownloadManager manager, string originalImortUrl = "")
        {
            AnalyzerContext context = GetContext(htmlSource, baseUrl, objectId, objectType, manager.SiteDb, originalImortUrl);
            context.DownloadManager = manager;
            return Execute(context);
        }

        public static AnalyzerContext GetContext(string htmlSource, string baseUrl, Guid objectId, byte objectType, Repository.SiteDb siteDb, string originalImortUrl = "")
        {
            string baseurl = baseUrl;

            AnalyzerContext context = new AnalyzerContext
            {
                SiteDb = siteDb, ObjectId = objectId, ObjectType = objectType, HtmlSource = htmlSource
            };

            string htmlbase = context.Dom.baseURI;

            if (!string.IsNullOrEmpty(htmlbase))
            {
                baseurl = htmlbase;
            }

            context.AbsoluteUrl = baseurl;

            if (!string.IsNullOrEmpty(originalImortUrl))
            {
                context.OriginalImportUrl = originalImortUrl;
            }

            return context;
        }
    }

    public class Replacer
    {
        public string WholeString { get; set; }

        public string oldValue { get; set; }

        public string NewValue { get; set; }
    }
}
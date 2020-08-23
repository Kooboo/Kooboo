using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Service;
using Kooboo.Sites.Sync;
using Kooboo.Sites.Sync.Disk;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Extension
{
    public class Converter : Kooboo.Data.Interface.IkScript
    {
        public string Name => "converter";

        public RenderContext context { get; set; }

        public byte[] HtmlToPdf(string htmlcode)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(htmlcode);

            var url = "http://159.138.83.215/_api/converter/HtmlToPdf";

            var result = Lib.Helper.HttpHelper.Post<string>(url, null, bytes);

            return Convert.FromBase64String(result);
        }

        public string officeToHTML(byte[] officebytes, string filename)
        {
            var sitedb = this.context.WebSite.SiteDb();

            string api = Data.AppSettings.ConvertApiUrl + "/_api/converter/Convert";

            var lastdot = filename.LastIndexOf(".");
            var extens = filename.Substring(lastdot);

            filename = Kooboo.Lib.Security.ShortGuid.GetNewShortId() + extens;

            string html = null;

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("filename", System.Net.WebUtility.UrlEncode(filename));
            var response = Lib.Helper.HttpHelper.ConvertKooboo(api, officebytes, header);

            if (response != null)
            {
                MemoryStream zipfile = new MemoryStream(response);

                var manager = new SyncManager(this.context.WebSite.Id);

                using (var archive = new ZipArchive(zipfile, ZipArchiveMode.Read))
                {

                    foreach (var entry in archive.Entries)
                    {
                        var target = entry.FullName;

                        if (!string.IsNullOrEmpty(entry.Name))
                        {
                            MemoryStream memory = new MemoryStream();
                            entry.Open().CopyTo(memory);
                            var bytes = memory.ToArray();

                            if (bytes != null && bytes.Length > 0)
                            {
                                var lower = entry.Name.ToLower();
                                if (lower.EndsWith(".htm") || lower.EndsWith(".html"))
                                {
                                    if (string.IsNullOrEmpty(html))
                                    {
                                        html = System.Text.Encoding.UTF8.GetString(bytes);
                                    }
                                }
                                else
                                {
                                    var mime = Lib.Helper.IOHelper.MimeType(target);

                                    var resultfilename = target;

                                    if (mime != null)
                                    {
                                        if (mime.ToLower().Contains("image") || mime.ToLower().Contains("css"))
                                        {
                                            resultfilename = System.IO.Path.Combine("officeconverter", target);
                                        }
                                    }

                                    manager.SyncToDb(resultfilename, sitedb, bytes);

                                }
                            }
                        }
                    }
                }
            }

            return CorrectHtmlLink(html);
        }

        private string CorrectHtmlLink(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            var dom = Kooboo.Dom.DomParser.CreateDom(input);

            List<SourceUpdate> updates = new List<SourceUpdate>();

            var imgurls = Kooboo.Sites.Service.DomUrlService.GetImageSrcs(dom);

            foreach (var item in imgurls)
            {
                if (!string.IsNullOrEmpty(item.Value) && !Kooboo.Lib.Utilities.DataUriService.isDataUri(item.Value))
                {

                    if (DomUrlService.IsExternalLink(item.Value))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(item.Value))
                    {
                        continue;
                    }

                    string RelativeUrl = AppendPath(item.Value);

                    if (item.Value != RelativeUrl)
                    {
                        string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item.Key);
                        string newstring = oldstring.Replace(item.Value, RelativeUrl);

                        updates.Add(new SourceUpdate()
                        {
                            StartIndex = item.Key.location.openTokenStartIndex,
                            EndIndex = item.Key.location.openTokenEndIndex,
                            NewValue = newstring
                        });
                    }

                }
            }


            // for external css link. 
            var list = dom.getElementsByTagName("link");

            foreach (var item in list.item)
            {
                //  < link rel = "stylesheet" href = " " >
                var rel = item.getAttribute("rel");
                if (rel != null && rel.ToLower() == "stylesheet")
                {
                    var href = item.getAttribute("href");
                    if (string.IsNullOrWhiteSpace(href))
                    {
                        continue;
                    }
                    else
                    {
                        string RelativeUrl = AppendPath(href);
                        if (href != RelativeUrl)
                        {
                            string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                            string newstring = oldstring.Replace(href, RelativeUrl);

                            updates.Add(new SourceUpdate()
                            {
                                StartIndex = item.location.openTokenStartIndex,
                                EndIndex = item.location.openTokenEndIndex,
                                NewValue = newstring
                            });
                        }
                    }

                }
            }


            var embeds = dom.getElementsByTagName("embed");
            //<embed src = "UpcE1I9h3kCZ1P2oBSKRpw_files/img_01.svg" type = "image/svg+xml" class="stl_03" />

            foreach (var item in embeds.item)
            {
                var type = item.getAttribute("type");
                if (type != null & type.ToLower().Contains("image"))
                {
                    var href = item.getAttribute("src");
                    if (string.IsNullOrWhiteSpace(href))
                    {
                        continue;
                    }
                    else
                    {
                        string RelativeUrl = AppendPath(href);
                        if (href != RelativeUrl)
                        {
                            var svgatt = new Dom.Attr();
                            svgatt.name = "pluginspage";
                            svgatt.value = "http://www.adobe.com/svg/viewer/install/";
                            item.attributes.Add(svgatt);
                            string oldstring = Kooboo.Sites.Service.DomService.ReSerializeOpenTag(item);
                            string newstring = oldstring.Replace(href, RelativeUrl);

                            updates.Add(new SourceUpdate()
                            {
                                StartIndex = item.location.openTokenStartIndex,
                                EndIndex = item.location.openTokenEndIndex,
                                NewValue = newstring
                            });
                        }
                    }
                }
            }


            var objs = dom.getElementsByTagName("object");
            //<embed src = "UpcE1I9h3kCZ1P2oBSKRpw_files/img_01.svg" type = "image/svg+xml" class="stl_03" />

            foreach (var item in objs.item)
            {
                var type = item.getAttribute("type");
                if (type == null || !type.ToLower().Contains("image"))
                {
                    continue;
                }

                var href = item.getAttribute("data");
                if (string.IsNullOrWhiteSpace(href) || !href.ToLower().EndsWith(".svg"))
                {
                    continue;
                }
                else
                {
                    string RelativeUrl = AppendPath(href);
                    if (href != RelativeUrl)
                    {
                        string oldstring = Kooboo.Sites.Service.DomService.ReSerializeOpenTag(item);
                        string newstring = oldstring.Replace(href, RelativeUrl);

                        updates.Add(new SourceUpdate()
                        {
                            StartIndex = item.location.openTokenStartIndex,
                            EndIndex = item.location.openTokenEndIndex,
                            NewValue = newstring
                        });
                    }
                }
            }

            if (updates.Any())
            {
                return Kooboo.Sites.Service.DomService.UpdateSource(input, updates);
            }
            return input;
        }

        private static string AppendPath(string urlpath)
        {
            string RelativeUrl = Kooboo.Lib.Helper.UrlHelper.RelativePath(urlpath);

            if (RelativeUrl.StartsWith("/") || RelativeUrl.Contains("/"))
            {
                RelativeUrl = "/officeconverter" + RelativeUrl;
            }
            else if (RelativeUrl.StartsWith("\\") || RelativeUrl.Contains("\\"))
            {
                RelativeUrl = "\\officeconverter" + RelativeUrl;
            }
            else
            {
                RelativeUrl = "/officeconverter/" + RelativeUrl;
            }

            return RelativeUrl;
        }

        public string OfficeToCleanHTML(byte[] officebytes, string filename)
        {
            var result = officeToHTML(officebytes, filename);

            return Kooboo.Data.Helper.DomHelper.CleanBodyStyle(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global
{
    public class DocumentObjectModel
    {
        public Kooboo.Dom.Document Document { get; set; }

        public DocumentObjectModel Parse(string html)
        {
            DocumentObjectModel result = new DocumentObjectModel();
            result.Document = Kooboo.Dom.DomParser.CreateDom(html);
            return result;
        }

        public bool HasImage
        {
            get
            {
                if (this.Document != null)
                {
                    foreach (var item in this.Document.images.item)
                    {
                        var src = item.getAttribute("src");
                        if (!string.IsNullOrEmpty(src))
                        {
                            return true;
                        }
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<DomImage> Images
        {
            get
            {
                List<DomImage> result = new List<DomImage>();

                foreach (var item in this.Document.images.item)
                {
                    var src = item.getAttribute("src");
                    if (!string.IsNullOrWhiteSpace(src))
                    {
                        var alt = item.getAttribute("alt");
                        DomImage img = new DomImage();
                        img.Src = src;
                        img.Alt = alt;
                        result.Add(img);
                    }
                }

                return result;
            }
        }
    }


    public class DomImage
    {
        public string Src { get; set; }

        public string Alt { get; set; }

    }
}

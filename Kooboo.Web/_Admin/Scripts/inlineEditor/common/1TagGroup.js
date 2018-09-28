function TagGroup(){
    var enumTagGroup={ Meta: "Meta", Link: "Link", Image: "Image", Section: "Section", Script: "Script", Table: "Table", Title: "Title", Embedded: "Embedded", SpecialContent: "SpecialContent", Style: "Style", Grouping: "Grouping", Forms: "Forms", Text: "Text", Undefined: "Undefined" };

    return {
        EnumTagGroup:enumTagGroup,
        GetGroup:function(element) {
            if (!element) {
                return this.EnumTagGroup.Undefined;
            }
            var tagName = element.tagName.toLowerCase();
    
            if (tagName == "a") {
                return this.EnumTagGroup.Link;
            } else if (tagName == "img") {
                return this.EnumTagGroup.Image;
            } else if (this.Text().indexOf(tagName) > -1) {
                return this.EnumTagGroup.Text;
            } else if (this.Table().indexOf(tagName) > -1) {
                return this.EnumTagGroup.Table;
            } else if (this.Embedded().indexOf(tagName) > -1) {
                return this.EnumTagGroup.Embedded;
            } else if (this.Forms().indexOf(tagName) > -1) {
                return this.EnumTagGroup.Forms;
            } else if (this.Title().indexOf(tagName) > -1) {
                return this.EnumTagGroup.Title;
            } else if (this.Section().indexOf(tagName) > -1) {
                return this.EnumTagGroup.Section;
            } else if (this.Meta().indexOf(tagName) > -1) {
                return this.EnumTagGroup.Meta;
            } else if (this.SpecialContent().indexOf(tagName) > -1) {
                return this.EnumTagGroup.Text;
            } else if (this.Grouping().indexOf(tagName) > -1) {
                return this.EnumTagGroup.Grouping;
            }
            return this.EnumTagGroup.Undefined;
        },
        Meta:function(element) {
            return [
                "head",
                "title",
                "base",
                "isindex",
                "link",
                "meta",
                "style"
            ];
        },
        Script:function(element) {
            return [
                "script",
                "noscript"
            ];
        },
        Table : function() {
            return [
                "table",
                "caption",
                "colgroup",
                "col",
                "tbody",
                "tfoot",
                "tr",
                "td",
                "th"
            ];
        },
        Section : function() {
            return [
                "section",
                "nav",
                "article",
                "aside",
                "header",
                "footer",
                "div"
            ];
        },
        Title : function() {
            return [
                "h1",
                "h2",
                "h3",
                "h4",
                "h5",
                "h6"
            ];
        },
        Embedded : function() {
            return [
                "iframe",
                "embed",
                "object",
                "param",
                "video",
                "audio",
                "source",
                "track",
                "canvas",
                "map",
                "area",
                "math",
                "svg",
                "applet",
                "frame",
                "frameset",
                "noframes",
                "bgsound",
                "noembed",
                "plaintext"
            ];
        },
        SpecialContent : function() {
            return [
                "address",
                "blockquote",
                "pre",
                "figure",
                "figcaption",
                "blockquote"
            ];
        },
        Style : function() {
            return ["style"];
        },
        Grouping : function() {
            return [
                "ol",
                "ul",
                "li",
                "dl",
                "dt",
                "dd"
            ];
        },
        Forms : function() {
            return [
                "form",
                "fieldset",
                "legend",
                "label",
                "input",
                "button",
                "select",
                "datalist",
                "optgroup",
                "option",
                "textarea",
                "keygen",
                "output",
                "progress",
                "meter"
            ];
        },
        Text : function() {
            return [
                "abbr",
                "acronym",
                "b",
                "basefont",
                "bdo",
                "big",
                "blink",
                "br",
                "cite",
                "code",
                "dfn",
                "em",
                "font",
                "i",
                "kbd",
                "listing",
                "mark",
                "marquee",
                "nextid",
                "nobr",
                "q",
                "rp",
                "rt",
                "ruby",
                "s",
                "samp",
                "small",
                "spacer",
                "span",
                "strike",
                "strong",
                "sub",
                "sup",
                "time",
                "tt",
                "u",
                "var",
                "wbr",
                "xmp",
                "p"
            ];
        }
    
    }
}
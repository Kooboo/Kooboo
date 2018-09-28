function DomResourceManager(){
    var emptyValues = ["none", "initial", "", null, undefined, "transparent"];
    var FontFamilies = [
        "Georgia",
        "serif",
        "Palatino Linotype",
        "Book Antiqua",
        "Palatino",
        "Times New Roman",
        "Times",
        "Arial",
        "Helvetica",
        "sans-serif",
        "Arial Black",
        "Gadget",
        "Comic Sans MS",
        "cursive",
        "Impact",
        "Charcoal",
        "Lucida Sans Unicode",
        "Lucida Grande",
        "Tahoma",
        "Geneva",
        "Trebuchet MS",
        "Verdana",
        "Courier New",
        "Courier",
        "monospace",
        "Lucida Console",
        "Monaco"
    ];
    var ImageAbbrMapping = {
        "background-image": "background",
        "cue-after": "cue",
        "cue-before": "cue",
        "list-style-image": "list-style"
    };
    var ColorProperties = [
        "background-color",
        "color",
        'border',
        // 'border-top',
        // 'border-right',
        // 'border-bottom',
        // 'border-left',
        // "border-color",
        // "border-top-color",
        // "border-bottom-color",
        // "border-left-color",
        // "border-right-color",
        //"outline",
        "outline-color", //Object.style.outlineColor=color-name|color-rgb|color-hex
        "text-shadow",
        "box-shadow"
    ];
    var ColorAbbrMapping = {
        "background-color": "background",
        "border-top-color": "border-color",
        "border-bottom-color": "border-color",
        "border-left-color": "border-color",
        "border-right-color": "border-color"
    };
    //from https://www.w3.org/TR/2011/REC-CSS2-20110607/propidx.html css2.2
    var NoImageProperties = [
        "azimuth",
        "background-attachment",
        "background-color",
        "background-position",
        "background-repeat",
        "border-collapse",
        "border-color",
        "border-spacing",
        "border-style",
        "border-top",
        "border-right",
        "border-bottom",
        "border-left",
        "border-top-color",
        "border-right-color",
        "border-bottom-color",
        "border-left-color",
        "border-top-style",
        "border-right-style",
        "border-bottom-style",
        "border-left-style",
        "border-top-width",
        "border-right-width",
        "border-bottom-width",
        "border-left-width",
        "border-width",
        "border",
        "bottom",
        "caption-side",
        "clear",
        "clip",
        "color",
        "counter-increment",
        "counter-reset",
        "direction",
        "display",
        "elevation",
        "empty-cells",
        "float",
        "font-family",
        "font-size",
        "font-style",
        "font-variant",
        "font-weight",
        "font",
        "height",
        "left",
        "letter-spacing",
        "line-height",
        "list-style-position",
        "list-style-type",
        "margin-right",
        "margin-left",
        "margin-top",
        "margin-bottom",
        "margin",
        "max-height",
        "max-width",
        "min-height",
        "min-width",
        "orphans",
        "outline-color",
        "outline-style",
        "outline-width",
        "outline",
        "overflow",
        "padding-top",
        "padding-right",
        "padding-bottom",
        "padding-left",
        "padding",
        "page-break-after",
        "page-break-before",
        "page-break-inside",
        "pause-after",
        "pause-before",
        "pause",
        "pitch-range",
        "pitch",
        "position",
        "quotes",
        "richness",
        "right",
        "speak-header",
        "speak-numeral",
        "speak-punctuation",
        "speak",
        "speech-rate",
        "stress",
        "table-layout",
        "text-align",
        "text-decoration",
        "text-indent",
        "text-transform",
        "top",
        "unicode-bidi",
        "vertical-align",
        "visibility",
        "voice-family",
        "volume",
        "white-space",
        "widows",
        "width",
        "word-spacing",
        "z-index"
    ];
    //baseUrl:site base url
    function getInlineStyleImages(doc, baseUrl, pageUrl) {
        var styleImages = [];

        var elements = $(doc).find("*");
        $.each(elements, function(i, element) {
            var koobooId = $(element).attr("kooboo-id");
            if (!koobooId) return true;
            var style = element.style;
            if (isImageFromContentAttr(element, true)) {
                return true; //continue cycle;
            }

            //todo merge same Image Url
            var images = getImagesByStyle(style, koobooId);
            $.each(images, function(i, image) {
                var imageUrl = getImageUrl(image.url, baseUrl, pageUrl);
                if (imageUrl) {
                    image.url = imageUrl;
                } else {
                    return true; //continue cycle;
                }
                image.els = [element];
                styleImages.push(image);
            });
        });
        return styleImages;
    }
     //页面上是否有受影响的元素
    function hasAffectedElement(cssRule, doc) {
        return cssRule && $(cssRule.selectorText, doc).length > 0;
    }
    
    function isDataImage(imageUrl) {
        return imageUrl.toLowerCase().indexOf("data:image") > -1;
    }
    function isImageFromContentAttr(el, isInline) {
        if (el) {
            var kattr = $(el).attr("k-attributes");
            if (!kattr) return false;
            var attrs = kattr.split(";");
            var isFromContentAttr = false;
            $.each(attrs, function(i, attr) {
                if (isInline) {
                    //k-attributes='style background-image:url(xx.xx)'
                    //filter content attribute of style
                    if (attr && attr.indexOf("style") > -1 &&
                        attr.indexOf(".style") == -1) {
                        isFromContentAttr = true;
                        return false;
                    }
                } else {
                    if (attr && attr.indexOf("src") > -1 &&
                        attr.indexOf(".src") == -1) {
                        isFromContentAttr = true;
                        return false;
                    }
                }

            });
            return isFromContentAttr;

        }
        return false;
    }
    function isAbbrProperty(key) {
        var isAbbrProperty = false;
        for (var imageProp in ImageAbbrMapping) {
            if (ImageAbbrMapping[imageProp] == key.toLowerCase()) {
                isAbbrProperty = true;
                break;
            }
        }
        return isAbbrProperty;
    }
    function getImagesByStyle(style, koobooId, styleSheetUrl, tagKoobooId, mediaRuleList) {
        var self = this;
        var images = [];
        if (style) {
            var index = 0;
            //has property will have index
            while (style[index]) {
                var propertyName = style[index];
                var propertyValue = style[propertyName];
                if (NoImageProperties.indexOf(propertyName.toLowerCase()) == -1 &&
                    !isAbbrProperty(propertyName)) { //非简写的属性如background，background的image，与background-image是一致的
                    var imageUrl = getStyleImageUrl(propertyValue);
                    if (imageUrl) {
                        images.push({
                            url: imageUrl,
                            cssRule: style,
                            cssProperty: propertyName,
                            koobooId: koobooId,
                            styleSheetUrl: styleSheetUrl,
                            styleTagKoobooId: tagKoobooId,
                            mediaRuleList: mediaRuleList
                        });
                    }
                }
                index++;
            }
        }
        return images;
    }
    //url("/moban2185/images/gg5.jpg")
    function getStyleImageUrl(propertyValue) {
        if (!propertyValue) {
            return "";
        }
        var matches = propertyValue.match(/url\((.*?)\)/);
        if (matches && matches.length > 1) {
            var url = matches[1].replace(/("|')/g, "");
            if (url) {
                return url;
            }
        }
        return "";
    }
    function doGetInternalStyleSheets(styleSheets, baseurl) {
        var internalStyleSheets = [];
        if (styleSheets && styleSheets.length > 0) {
            $.each(styleSheets, function(i, styleSheet) {
                var href = styleSheet.href;
                if (!baseurl || !href || href.toLowerCase().indexOf(baseurl.toLowerCase()) > -1) {
                    internalStyleSheets.push(styleSheet);
                }
            });
        }
        return internalStyleSheets;
    }
    function getBaseUrl() {
        if (window.__gl)
            return window.__gl.baseUrl;
        return "";
    }
    
    function getColorsByStyle(style) {
        var result = [];
        $.each(ColorProperties, function(i, prop) {
            var color = style.getPropertyValue(prop);
            if (color) {
                var temp = {};
                temp["cssProperty"] = prop;
                temp["value"] = color;
                temp["color"] = color;
                result.push(temp);
            }
        });
        return result;
    }
    // function getKoobooIdByStyleSheet(styleSheet) {
    //     var ownerNode = styleSheet.ownerNode;
    //     if (ownerNode && ownerNode.attributes && ownerNode.attributes["kooboo-id"]) {
    //         return ownerNode.attributes["kooboo-id"].value;
    //     }
    //     return "";
    // }
    function getImageAbsoluteUrl(baseUrl, url, imageUrl) {
        var absoluteUrl = imageUrl;
        if (absoluteUrl.indexOf("/") == 0 || absoluteUrl.indexOf("http") == 0 ||
            absoluteUrl.indexOf("https") == 0) return absoluteUrl;

        var containerUrl = url.replace(baseUrl, "");
        var containerUrlparts = containerUrl.split("/");
        containerUrlparts.pop();

        var urlparts = imageUrl.split("/");
        var needRemove = true;
        while (needRemove) {
            if (urlparts.length > 0) {
                var part = urlparts[0];
                if (part == "..") {
                    urlparts.shift();
                    containerUrlparts.pop();
                } else if (part == ".") {
                    urlparts.shift();
                } else {
                    needRemove = false;
                }
            }
            needRemove = false;
        }
        absoluteUrl = containerUrlparts.join("/") + "/" + urlparts.join("/");
        if (absoluteUrl.indexOf("/") != 0)
            absoluteUrl = "/" + absoluteUrl;
        return absoluteUrl;
    }
    function getImageUrl(url, baseUrl, pageUrl) {
        var absoluteUrl = "";
        if (isDataImage(url)) {
            absoluteUrl = url;
        } else if (isUrlImage(url)) {
            absoluteUrl = getImageAbsoluteUrl(baseUrl, pageUrl, url);
        }
        return absoluteUrl;
    }
    //maybe has external css.(in firefox,will throw error)
    function getInternalStyleSheets(styleSheets) {
        var baseUrl = getBaseUrl();
        return doGetInternalStyleSheets(styleSheets, baseUrl);
    }
    function isUrlImage(imageUrl) {
        //常见图片类型
        //get normal image file extension from https://en.wikipedia.org/wiki/Image_file_formats
        var imageTypes = [".jpg", ".jpeg", ".jp2", ".tiff", ".tif", ".gif", ".bmp", ".png", ".svg", ".ico"];
        var imageType = _.find(imageTypes, function(type) {
            return imageUrl.toLowerCase().indexOf(type) > -1;
        });
        if (imageType) return true;
        return false;

    }
    return {
        isImageFromContentAttr:isImageFromContentAttr,
        getImageUrl:getImageUrl,

        colorProperties: ColorProperties,
        emptyValues: emptyValues,
        noImageProperties: NoImageProperties,
        fontFamilies: FontFamilies,
        getContentImages: function(doc, baseUrl, pageUrl) {
            var self = this;
            var images = doc.images;
            var imagesWithSize = [];
            $.each(images, function(i, image) {
                if (!isImageFromContentAttr(image)) {
                    var url = $(image).attr("src"),
                        koobooId = $(image).attr("kooboo-id");
                    if (!koobooId) return true;
                    var absoluteUrl = getImageUrl(url, baseUrl, pageUrl);
                    if (!absoluteUrl) return true; //continue cycle

                    var existImage = _.find(imagesWithSize, function(imagewithSize) {
                        return imagewithSize.url && absoluteUrl &&
                            imagewithSize.url.toLowerCase() == absoluteUrl.toLowerCase();
                    });
                    if (!existImage || existImage.length == 0) {
                        var width = image.width,
                            height = image.height;
                        if (width == 0 || height == 0) {
                            var rect = image.getBoundingClientRect();
                            width = parseInt(rect.width);
                            height = parseInt(rect.height);
                        }
                        var els = [image];
                        imagesWithSize.push({
                            url: absoluteUrl,
                            width: width,
                            height: height,
                            els: els
                        });
                    } else {
                        existImage.els.push(image);
                    }
                }
            });
            return imagesWithSize;
        },
        getStyleImages: function(doc, baseUrl, pageUrl) {
            var styleImages = this.getStyleImageOfStyleSheet(doc, baseUrl, pageUrl);
            var inlineStyleImages = getInlineStyleImages(doc, baseUrl, pageUrl);
            var images = _.uniq(styleImages.concat(inlineStyleImages));
            return images;
        },
        getStyleImageOfStyleSheet:function(doc, baseUrl, pageUrl) {
            var styleImages = [],
                self=this;
            var sheets = doc.styleSheets;
            var styleSheets = getInternalStyleSheets(sheets);
            if (styleSheets && styleSheets.length > 0) {
                $.each(styleSheets, function(i, styleSheet) {
                    var cssRules=self.getCssRules(styleSheet);
                    if (cssRules && cssRules.length > 0) {
                        $.each(cssRules, function(i, cssRule) {
                            if (!hasAffectedElement(cssRule, doc)) {
                                return true; //跳过当次循环
                            }
                            var style = cssRule.style;
                            var tagKoobooId = self.getStyleTagKoobooId(cssRule);
                            var styleSheetUrl = styleSheet.href;
                            if (!styleSheetUrl && !tagKoobooId) return true;
                            var mediaRuleList = self.getMediaRuleList(cssRule);
                            var images = getImagesByStyle(style, "", styleSheetUrl, tagKoobooId, mediaRuleList);
    
                            var relativeUrl = styleSheetUrl ? styleSheetUrl : pageUrl;
                            $.each(images, function(i, image) {
                                var imageUrl = getImageUrl(image.url, baseUrl, relativeUrl);
                                if (imageUrl) {
                                    image.url = imageUrl;
                                } else {
                                    return true; //continue cycle;
                                }
    
                                image.selector = cssRule.selectorText;
                                var els = $(image.selector, Kooboo.InlineEditor.getIFrameDoc());
    
                                image.els = els;
                                styleImages.push(image);
                            });
    
                        });
                    }
                });
            }
    
            return styleImages;
        },
        
        isImage: function(url) {
            return isDataImage(url) || isUrlImage(url);
        },
        getImageFullUrl: function(baseUrl, containerUrl, imageUrl) {
            var fullurl = "";
            if (isDataImage(imageUrl)) {
                fullurl = imageUrl;
            } else if (isUrlImage(imageUrl)) {
                if (baseUrl) {
                    var absoluteUrl = this.getImageAbsoluteUrl(baseUrl, containerUrl, imageUrl);
                    var lastLash = baseUrl[baseUrl.length - 1];

                    if (lastLash != "\/")
                        baseUrl += "\/";
                    var firstLash = absoluteUrl[0];
                    if (firstLash == "\/") {
                        absoluteUrl = absoluteUrl.substr(1);
                    }
                    fullurl = baseUrl + absoluteUrl;
                } else {
                    fullurl = imageUrl;
                }

            }
            return fullurl;
        },
        getImageAbsoluteUrl: function(baseUrl, url, imageUrl) {
            var absoluteUrl = imageUrl;
            if (absoluteUrl.indexOf("/") == 0 || absoluteUrl.indexOf("http") == 0 ||
                absoluteUrl.indexOf("https") == 0) return absoluteUrl;

            var containerUrl = url.replace(baseUrl, "");
            var containerUrlparts = containerUrl.split("/");
            containerUrlparts.pop();

            var urlparts = imageUrl.split("/");
            var needRemove = true;
            while (needRemove) {
                if (urlparts.length > 0) {
                    var part = urlparts[0];
                    if (part == "..") {
                        urlparts.shift();
                        containerUrlparts.pop();
                    } else if (part == ".") {
                        urlparts.shift();
                    } else {
                        needRemove = false;
                    }
                }
                needRemove = false;
            }
            absoluteUrl = containerUrlparts.join("/") + "/" + urlparts.join("/");
            if (absoluteUrl.indexOf("/") != 0)
                absoluteUrl = "/" + absoluteUrl;
            return absoluteUrl;
        },
        //获取所有的颜色
        getDocColors: function(doc) {
            var self = this;
            var styleSheetColors = this.getColorOfStyleSheets(doc),
                inlineSheetColors = this.getColorOfInlineStyle(doc),
                colors = styleSheetColors.concat(inlineSheetColors);
            return colors;
        },
        convertRgbToHex: function(rgbColor) {
            var hexColor = new Kooboo.Color(rgbColor).toString();
            return hexColor;
        },
        getStyleTagKoobooId: function(cssRule) {
            var self = this;
            var parentStyleSheet;
            if (cssRule.parentStyleSheet) {
                parentStyleSheet = cssRule.parentStyleSheet;
            } else if (cssRule.parentRule && cssRule.parentRule.parentStyleSheet) {
                parentStyleSheet = cssRule.parentRule.parentStyleSheet;
            }
            if (!self.isEmbeddedStyleSheet(parentStyleSheet)) return "";

            if (parentStyleSheet &&
                parentStyleSheet.ownerNode &&
                parentStyleSheet.ownerNode.getAttribute) {
                return parentStyleSheet.ownerNode.getAttribute("kooboo-id");
            }

            return "";
        },
        getMediaRuleList: function(cssRule) {
            var self = this;
            var mediaRuleList = [];
            while (self.isMediaRule(cssRule)) {
                mediaRuleList.push(cssRule.parentRule.conditionText);
                cssRule = cssRule.parentRule;
            }
            return mediaRuleList.length > 0 ? mediaRuleList : null;
        },
        isEmbeddedStyleSheet: function(styleSheet) {
            return !styleSheet || !styleSheet.href;
        },
        isMediaRule: function(rule) {
            return rule && rule.parentRule && rule.parentRule.type === 4;
        },
        isNeedShowProperty: function(prop, computedStyle) {
            var self = this;
            if (prop.indexOf("border") > -1) {
                switch (prop) {
                    case "border-color":
                        if (self.isZeroWidth("borderTopWidth", computedStyle) &&
                            self.isZeroWidth("borderBottomWidth", computedStyle) &&
                            self.isZeroWidth("borderLeftWidth", computedStyle) &&
                            self.isZeroWidth("borderRightWidth", computedStyle))
                            return false;
                        break;
                    case "border-top-color":
                        return !self.isZeroWidth("borderTopWidth", computedStyle);
                    case "border-bottom-color":
                        return !self.isZeroWidth("borderBottomWidth", computedStyle);
                    case "border-left-color":
                        return !self.isZeroWidth("borderLeftWidth", computedStyle);
                    case "border-right-color":
                        return !self.isZeroWidth("borderRightWidth", computedStyle);
                }
            } else if (prop.indexOf("outline") > -1) {
                if (self.isZeroWidth("outlineWidth", computedStyle))
                    return false;
            }
            //text-shadow,box-shadow
            return true;
        },
        isZeroWidth: function(widthProperty, computedStyle) {
            var width = computedStyle[widthProperty];
            return !width || width.toLowerCase().indexOf("0px") > -1;
        },
        getCssRules:function(styleSheet){
            if(styleSheet){
                try{
                    //it can't get the stylesheet's cssRules with different domain
                    return styleSheet.cssRules;
                }catch(ex){

                }
            }
            return null;
        },
        getColorOfStyleSheets:function(doc) {
            var self = this;
            var colors = [];
            var sheets = doc.styleSheets;
    
            var styleSheets = self.getInternalStyleSheets(sheets);
            if (styleSheets && styleSheets.length > 0) {
    
                $.each(styleSheets, function(i, styleSheet) {
                    var cssRules = self.getCssRules(styleSheet),
                        styleSheetUrl = styleSheet.href;
                    if (cssRules && cssRules.length > 0) {
                        //var cssRuleColors = [];
                        $.each(cssRules, function(i, cssRule) {
                            if (cssRule.style) {
                                var tagKoobooId = self.getStyleTagKoobooId(cssRule),
                                    mediaRuleList = self.getMediaRuleList(cssRule),
                                    selectorText = cssRule.selectorText;
    
                                var declareColors = getColorsByStyle(cssRule.style);
                                $.each(declareColors, function(i, declareColor) {
                                    declareColor.color = self.convertRgbToHex(declareColor.color);
                                    declareColor.selector = selectorText;
                                    declareColor.cssRule = cssRule;
                                    declareColor.cssStyleRule = cssRule.style;
                                    declareColor.styleTagKoobooId = tagKoobooId;
                                    declareColor.mediaRuleList = mediaRuleList;
                                    declareColor.styleSheetUrl = styleSheetUrl;
                                    declareColor.important = cssRule.style.getPropertyPriority[declareColor.cssProperty];
                                    colors.push(declareColor);
                                });
                            }
    
                        });
                    }
                });
            }
            return colors;
        },
        getColorOfInlineStyle:function (doc) {
            var self = this;
            var inlineColors = [];
            var elements = $(doc).find("*");
            $.each(elements, function(i, el) {
                var style = el.style,
                    koobooId = $(el).attr("kooboo-id");
    
                if (koobooId) {
                    var declareColors = getColorsByStyle(style);
    
                    $.each(declareColors, function(j, declareColor) {
                        declareColor.color = self.convertRgbToHex(declareColor.color);
                        declareColor.cssStyleRule = style;
                        declareColor.important = style.getPropertyPriority[declareColor.cssProperty];
                        declareColor.koobooId = koobooId;
                        declareColor.el = el;
                        inlineColors.push(declareColor);
                    });
                }
            });
            return inlineColors;
        },
        getBaseUrl:getBaseUrl,
        getImageAbsoluteUrl:getImageAbsoluteUrl,
        getInternalStyleSheets:getInternalStyleSheets,
        getStyleImageUrl:getStyleImageUrl
    }
}
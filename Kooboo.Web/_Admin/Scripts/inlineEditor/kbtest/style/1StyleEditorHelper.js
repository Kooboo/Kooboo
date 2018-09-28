// require([
//     "siteEditor/widgets/floatMenu/helper/StyleEditorHelper"
// ], function(StyleEditorHelper) {

// });
var StyleEditorHelper = Kooboo.InlineEditor.StyleEditorHelper;
var doc = document.implementation.createHTMLDocument("");
var html = "<html><body><div id='testdiv'>test</div></body></html>";
doc.write(html);
var el = doc.getElementById("testdiv");
var styleHelper = Kooboo.style.StyleEditorHelper;

function createIframe(create) {
    if (!create) return;
    var iframe = document.createElement('iframe');
    iframe.id = "iframetest";
    iframe.setAttribute('id', 'iframetest');
    iframe.style.display = 'none';
    document.body.appendChild(iframe);
    return iframe;
}

function removeIframe(iframe) {
    if (iframe)
        document.body.removeChild(iframe);
}

function getInlineCSSRule() {
    var doc = document.implementation.createHTMLDocument("");
    var html = "<html><body><div id='testdiv' style='color:red'>test</div></body></html>";

    doc.write(html);
    var el = doc.getElementById("testdiv");
    var inlineStyle = styleHelper.getInlineCSSRule(el);
    expect(inlineStyle).not.to.eql(undefined);
    expect(inlineStyle.color).to.eql("red");


    doc = document.implementation.createHTMLDocument("");
    var html = "<html><body><font id='testdiv' style='color:red' color='blue'>test</font></body></html>";

    doc.write(html);
    el = doc.getElementById("testdiv");
    var inlineStyle = styleHelper.getInlineCSSRule(el);
    expect(inlineStyle).not.to.eql(undefined);
    expect(inlineStyle.color).to.eql("blue");

    doc = document.implementation.createHTMLDocument("");
    var html = "<html><body><font kooboo-id='2-13-3-11-1-1-5-5-1-0' id='testdiv' style='color: rgb(125, 156, 179);'>详细</font></body></html>";

    doc.write(html);
    el = doc.getElementById("testdiv");
    var inlineStyle = styleHelper.getInlineCSSRule(el);
    expect(inlineStyle).not.to.eql(undefined);
    expect(inlineStyle.color).to.eql("rgb(125, 156, 179)");
}

function isEqualValue() {
    var property = "color";
    var computeValue = "red";
    var value = "red";
    var isEqual = styleHelper.isEqualValue(property, computeValue, value);
    expect(isEqual).to.eql(true);

    var property = "font-weight";
    var computeValue = "400";
    var value = "normal";
    var isEqual = styleHelper.isEqualValue(property, computeValue, value);
    expect(isEqual).to.eql(true);

    var property = "font-weight";
    var computeValue = "400";
    var value = "bold";
    var isEqual = styleHelper.isEqualValue(property, computeValue, value);
    expect(isEqual).to.eql(false);

    var property = "font-weight";
    var computeValue = "700";
    var value = "bold";
    var isEqual = styleHelper.isEqualValue(property, computeValue, value);
    expect(isEqual).to.eql(true);

    var property = "color";
    var computeValue = "red";
    var value = "green";
    var isEqual = styleHelper.isEqualValue(property, computeValue, value);
    expect(isEqual).to.eql(false);

    var property = "background-image";
    var computeValue = "url('a.png')";
    var value = "url('a.png')";
    KMock(Kooboo.DomResourceManager).callFake("getBaseUrl", function() {
        return "";
    });
    KMock(styleHelper).callFake("getIframeUrl", function() {
        return "";
    })
    var isEqual = styleHelper.isEqualValue(property, computeValue, value);
    expect(isEqual).to.eql(true);

    var property = "background-image";
    var computeValue = "url('a.png')";
    var value = "url('A.png')";
    var isEqual = styleHelper.isEqualValue(property, computeValue, value);
    expect(isEqual).to.eql(true);

    var property = "background-image";
    var computeValue = "url('a.png')";
    var value = "url('b.png')";
    var isEqual = styleHelper.isEqualValue(property, computeValue, value);
    expect(isEqual).to.eql(false);

    KMock(styleHelper).flush();
    KMock(Kooboo.DomResourceManager).flush();

    var property = "background-image";
    var computeValue = "url('http://www.baidu.com/test/a.png')";
    var value = "url('test/a.png')";
    var styleSheetUrl = "http://www.baidu.com/";

    KMock(Kooboo.DomResourceManager).callFake("getBaseUrl", function() {
        return "http://www.baidu.com/";
    })

    var isEqual = styleHelper.isEqualValue(property, computeValue, value, styleSheetUrl);
    expect(isEqual).to.eql(true);

    KMock(Kooboo.DomResourceManager).flush();

}

function trimPseudoClass() {
    var selectorText = "a";
    var newSelector = styleHelper.trimPseudoClass(selectorText);
    expect(newSelector).to.eql("a");

    selectorText = ":root .fa-rotate-90, :root .fa-rotate-180, :root .fa-rotate-270, :root .fa-flip-horizontal, :root .fa-flip-vertical";
    newSelector = styleHelper.trimPseudoClass(selectorText);
    expect(newSelector).to.eql(selectorText);

    selectorText = "a:hover";
    newSelector = styleHelper.trimPseudoClass(selectorText);
    expect(newSelector).to.eql("a");

    selectorText = "a,.test:hover";
    newSelector = styleHelper.trimPseudoClass(selectorText);
    expect(newSelector).to.eql("a,.test");

    selectorText = "a,.test:hover,.div:focus";
    newSelector = styleHelper.trimPseudoClass(selectorText);
    expect(newSelector).to.eql("a,.test,.div");

    selectorText = "a,.test:hover,";
    newSelector = styleHelper.trimPseudoClass(selectorText);
    expect(newSelector).to.eql("a,.test");

    selectorText = "a,.test:nth-child(4n)";
    newSelector = styleHelper.trimPseudoClass(selectorText);
    expect(newSelector).to.eql("a,.test:nth-child(4n)");
}

function getFontSize() {
    var size = "0";
    var fontSize = styleHelper.getFontSize(size);
    expect(fontSize).to.eql("xx-small");

    var size = "1";
    var fontSize = styleHelper.getFontSize(size);
    expect(fontSize).to.eql("x-small");

    var size = "2";
    var fontSize = styleHelper.getFontSize(size);
    expect(fontSize).to.eql("small");

    var size = "3";
    var fontSize = styleHelper.getFontSize(size);
    expect(fontSize).to.eql("medium");

    var size = "4";
    var fontSize = styleHelper.getFontSize(size);
    expect(fontSize).to.eql("large");

    var size = "5";
    var fontSize = styleHelper.getFontSize(size);
    expect(fontSize).to.eql("x-large");

    var size = "6";
    var fontSize = styleHelper.getFontSize(size);
    expect(fontSize).to.eql("xx-large");
}

function getComputedStyle() {
    var iframe = createIframe(true);
    var iframeDoc = iframe.contentDocument;

    iframeDoc.body.innerHTML = "<div id='testdiv' style='color:rgb(255, 0, 0)'>test</div>";
    var el = iframeDoc.getElementById("testdiv");
    var computedStyle = styleHelper.getComputedStyle(el);
    expect(computedStyle).not.to.eql(undefined);
    var color = computedStyle.color;
    expect(color).to.eql("rgb(255, 0, 0)");
    removeIframe(iframe)
}

function filterMediaStyle() {
    var iframe = createIframe(true);
    var iframeDoc = iframe.contentDocument;

    var htmlArr = [];
    //content attribute
    htmlArr.push('<style media="screen,projection">div{color:red}</style>')
    htmlArr.push('<div id="test">11</div>');

    iframeDoc.body.innerHTML = htmlArr.join("");
    var el = iframeDoc.getElementById("test");
    
    //can't get stylesheet from document.implementation.createHTMLDocument
    var styleSheet = styleHelper.filterMediaStyle(el.ownerDocument.styleSheets);
    expect(styleSheet.length).to.eql(1);

    htmlArr = [];
    //content attribute
    htmlArr.push('<style media="print">div{color:red}</style>')
    htmlArr.push('<div id="test">11</div>');
    iframeDoc.body.innerHTML = htmlArr.join("");
    var el = iframeDoc.getElementById("test");
    var styleSheet = styleHelper.filterMediaStyle(el.ownerDocument.styleSheets);
    expect(styleSheet.length).to.eql(0);

    removeIframe(iframe);
}

function getMatchedCSSRules() {

    var iframe = createIframe(true);
    var iframeDoc = iframe.contentDocument;
    
    iframeDoc.body.innerHTML = "<div id='testdiv'>test</div>";
    var el = iframeDoc.getElementById("testdiv");

    var result = styleHelper.getMatchedCSSRules(el);
    expect(result.length).to.eql(0);

    iframeDoc.body.innerHTML = "<div id='testdiv'>test</div><style type='text/css'>div{color:blue;}</style>";

    el = iframeDoc.getElementById("testdiv");

    var result = styleHelper.getMatchedCSSRules(el);
    expect(result.length).to.eql(1);
    expect(result[0].style.color).to.eql("blue");

    iframeDoc.body.innerHTML = "<div id='testdiv'>test</div><style>body div{color:red;}</style> <style>div{color:blue;}</style>";

    el = iframeDoc.getElementById("testdiv");
    var result = styleHelper.getMatchedCSSRules(el);

    expect(result.length).to.eql(2);
    expect(result[0].style.color).to.eql("red");

    expect(result[1].style.color).to.eql("blue");

    iframeDoc.body.innerHTML = "<div id='testdiv'>test</div><style>body div{color:red;}</style> <style>div{color:blue;}</style> <style>#testdiv{color:green;}</style>";
    el = iframeDoc.getElementById("testdiv");
    var result = styleHelper.getMatchedCSSRules(el);

    expect(result.length).to.eql(3);
    expect(result[0].style.color).to.eql("red");
    expect(result[1].style.color).to.eql("blue");
    expect(result[2].style.color).to.eql("green");

    iframeDoc.body.innerHTML = "<link href='http://bdimg.share.baidu.com/static/api/css/share_style0_16.css?v=6aba13f0.css' rel='stylesheet' type='text/css' /> </head><body><div id='testdiv'></div>";
    el = iframeDoc.getElementById("testdiv");
    var result = styleHelper.getMatchedCSSRules(el);
    expect(result.length).to.eql(0);

    removeIframe(iframe);

}

function filterMatchedCSSRules() {
    var rules = {
        0: {
            selectorText: "h1"
        },
        1: {
            selectorText: "h1,h2.test"
        },
        2: {
            selectorText: "*,h2.test"
        },
        3: {
            selectorText: "h2.test"
        }
    }

    var matcheRules = styleHelper.filterMatchedCSSRules(rules);
    expect(matcheRules.length).to.eql(1);
    expect(matcheRules[0].selectorText).to.eql("h2.test");
}

function hasElementSelector() {
    var selector = "a";
    var hasElementSelector = styleHelper.hasElementSelector(selector);
    expect(hasElementSelector).to.eql(true);

    selector = "a,div.test";
    var hasElementSelector = styleHelper.hasElementSelector(selector);
    expect(hasElementSelector).to.eql(true);

    selector = "div.test";
    var hasElementSelector = styleHelper.hasElementSelector(selector);
    expect(hasElementSelector).to.eql(false);

    selector = "#test";
    var hasElementSelector = styleHelper.hasElementSelector(selector);
    expect(hasElementSelector).to.eql(false);
}


function isNear() {
    var iframe = createIframe(true);
    var iframeDoc = iframe.contentDocument;
    iframeDoc.body.innerHTML = "<div id='testdiv' style='width:30px;'><span id='testspan'>test</span></div><style>#testdiv{color:red}</style>";
    var el = iframeDoc.getElementById("testspan");
    var parentElement = iframeDoc.getElementById("testdiv");
    var rules = styleHelper.getMatchedCSSRules(el);
    var near = styleHelper.isNear(el, parentElement);

    expect(near).to.eql(true);
    removeIframe(iframe);
}

function removePseudoRules() {
    var rules = [{
        selectorText: "a"
    }, {
        selectorText: "a:hover"
    }]

    var remainRules = styleHelper.removePseudoRules(rules);

    expect(remainRules.length).to.eql(1);
    expect(remainRules[0].selectorText).to.eql("a");
}

function getPseudoRules() {
    var rules = [{
        selectorText: "div",
    }, {
        selectorText: "a:link,a",
    }, {
        selectorText: "a:focus",
    }];


    var rules = styleHelper.getPseudoRules(rules);
    expect(rules.length).to.eql(1);
    expect(rules[0].selectorText).to.eql("a:focus");
}



function filterPseudoClass() {
    var selector = "a:focus,a:visited,a";
    var matches = styleHelper.filterPseudoClass(selector);
    expect(matches.length).to.eql(2);
    expect(matches[0]).to.eql(":focus");
    expect(matches[1]).to.eql(":visited");

    var selector = "a";
    var matches = styleHelper.filterPseudoClass(selector);
    expect(matches.length).to.eql(0);

    var selector = "div:nth-child(2n)";
    var matches = styleHelper.filterPseudoClass(selector);
    expect(matches.length).to.eql(0);
}



function isLegalClassName() {
    var isLegal = styleHelper.isLegalClassName(".1aaa");
    expect(isLegal).to.be.eql(false);
    isLegal = styleHelper.isLegalClassName(".aaa");
    expect(isLegal).to.be.eql(true);
}

function isClassOrIdRule() {
    var rule = {
        selectorText: "#test"
    }
    var isClassOrIdRule = styleHelper.isClassOrIdRule(rule);

    expect(isClassOrIdRule).to.eql(true);

    rule = {
        selectorText: ".class"
    }
    isClassOrIdRule = styleHelper.isClassOrIdRule(rule);
    expect(isClassOrIdRule).to.eql(true);

    rule = {
        selectorText: "*"
    }
    isClassOrIdRule = styleHelper.isClassOrIdRule(rule);
    expect(isClassOrIdRule).to.eql(false);
}

function hasClassOrIdRules() {
    var matchedRules = [{
        selectorText: "#test"
    }];
    var hasClassOrIdRules = styleHelper.hasClassOrIdRules(matchedRules);
    expect(hasClassOrIdRules).to.eql(true);

    var matchedRules = [{
        selectorText: "*"
    }];
    var hasClassOrIdRules = styleHelper.hasClassOrIdRules(matchedRules);
    expect(hasClassOrIdRules).to.eql(false);

    var matchedRules = [{
        selectorText: "*"
    }, {
        selectorText: "#test"
    }];

    var hasClassOrIdRules = styleHelper.hasClassOrIdRules(matchedRules);
    expect(hasClassOrIdRules).to.eql(true);
}

function isEmptyNodeOrHtmlNode() {
    var node = null;
    var isEmptyNodeOrHtmlNode = styleHelper.isEmptyNodeOrHtmlNode(node);
    expect(isEmptyNodeOrHtmlNode).to.eql(true);

    node = {
        parentNode: null
    };
    isEmptyNodeOrHtmlNode = styleHelper.isEmptyNodeOrHtmlNode(node);
    expect(isEmptyNodeOrHtmlNode).to.eql(true);

    node = {
        parentNode: {
            tagName: "html"
        }
    };
    isEmptyNodeOrHtmlNode = styleHelper.isEmptyNodeOrHtmlNode(node);
    expect(isEmptyNodeOrHtmlNode).to.eql(true);

    node = {
        parentNode: {
            tagName: "div"
        }
    };
    isEmptyNodeOrHtmlNode = styleHelper.isEmptyNodeOrHtmlNode(node);
    expect(isEmptyNodeOrHtmlNode).to.eql(false);
}

function isMediaRule() {
    var rule = {
        parentRule: { type: 4 }
    }
    var isMediaRule = styleHelper.isMediaRule(rule);
    expect(isMediaRule).to.eql(true);
}

function isEmbeddedStyleSheet() {
    var styleSheet = { href: "test.css" };
    var isEmbeddedStyleSheet = styleHelper.isEmbeddedStyleSheet(styleSheet);
    expect(isEmbeddedStyleSheet).to.eql(false);
}

function getMediaRuleList() {
    var cssRule = {
        parentRule: {
            type: 4,
            conditionText: "xx"
        }
    };

    var mediaRuleList = styleHelper.getMediaRuleList(cssRule);
    expect(mediaRuleList.length).to.eql(1);
}

function getInlineCssJson() {
    KMock(styleHelper).callFake("getComputedStyle", function() {
        return {
            getPropertyValue: function() {
                return "red";
            },
            getPropertyPriority: function() {
                return "!important";
            }
        };
    });

    var doc = document.implementation.createHTMLDocument("");
    var html = "<html><body><div class='bg' id='testdiv' kooboo-id='1-1'></div><style>.bg{color:blue}</style></body></html>";

    doc.write(html);
    var el = $("#testdiv", doc)[0];

    var inlineCssJson = styleHelper.getInlineCssJson(el, "color");
    expect(inlineCssJson.cssProperty).to.eql("color");
    expect(inlineCssJson.value).to.eql("red");
    expect(inlineCssJson.color).to.eql("red");
    expect(inlineCssJson.selector).to.eql(null);
    expect(inlineCssJson.koobooId).to.eql("1-1");
    expect(inlineCssJson.important).to.eql(true);
    KMock(styleHelper).flush();
}

function findRuleBySelector() {
    var sheet = {
            rules: [{
                selectorText: "div",

            }, {
                selectorText: "table",

            }]
        },
        selector = "table";
    var rule = styleHelper.findRuleBySelector(sheet, selector);
    expect(rule).not.to.eql(null);
    expect(rule.selectorText).to.eql("table");
}

function isExistProperty() {
    var items = {};
    items["background-color"] = { title: "test" };
    var isExist = styleHelper.isExistProperty(items, "background-color");
    expect(isExist).to.eql(true);
}

function isInlineCssRule() {
    var cssRule = {
        parentRule: "1",
    }
    var isInline = styleHelper.isInlineCssRule(cssRule);
    expect(isInline, true);

    cssRule = {
        parentRule: null,
    }
    var isInline = styleHelper.isInlineCssRule(cssRule);
    expect(isInline, false);
}
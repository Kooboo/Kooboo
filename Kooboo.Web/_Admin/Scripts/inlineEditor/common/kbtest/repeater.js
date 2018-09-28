k.setHtml("simplehtml", "repeater.html");

parser = new DOMParser();
simpledoc = parser.parseFromString(simplehtml, "text/html");
var DomService = Kooboo.NewDomService,
    TagGroup = Kooboo.TagGroup,
    TemplateManager=Kooboo.Repeater.TemplateManager,
    RepeaterHelper=Kooboo.Repeater.RepeaterHelper;


var _valid= function(TestTrue, method) {
    expect(TestTrue).to.eql(true);
    console.log(method + " pass!");
}
var _validDivRepeater= function(items, method) {
    var TestTrue = false;
    if (items
        && items.length === 3
        && items[0].id === "div1"
        && items[1].id === "div2"
        && items[2].id === "div3") {
        TestTrue = true;
    }
    _valid(TestTrue, method);
}

var _validTableSubRepeater= function(items, method) {
    var TestTrue = false;
    if (items != null
        && items.length == 6
        && items[0].id == "tr1"
        && items[1].id == "tr2") {
        TestTrue = true;
    }
    _valid(TestTrue, method);
}
var _validTdSuperRepeater1= function (items, method) {
    var TestTrue = false;
    if (items != null
        && items.length == 5
        && items[0].id == "td11") {
        TestTrue = true;
    }
    _valid(TestTrue, method);
}
var _validTdSuperRepeater2= function (items, method) {
    var TestTrue = false;
    if (items != null
        && items.length == 6
        && items[0].id == "tr1"
        && items[1].id == "tr2") {
        TestTrue = true;
    }
    _valid(TestTrue, method);
}
function RepeaterShouldDeeper() {
    var container = simpledoc.getElementById("repeaterdeeper");
    var result = Kooboo.Repeater.findSubRepeater(container);
    expect(result).not.to.eql(null);
    expect(result.length).not.to.eql(2);

}

function RepeatShouldNoBeRepater() {

    var container = simpledoc.getElementById("NoRepeater1");

    var result = Kooboo.Repeater.findSubRepeater(container);

    expect(result).to.eql(null);

    container = simpledoc.getElementById("NoRepeater2");

    result = Kooboo.Repeater.findSubRepeater(container);

    expect(result).to.eql(null);
}

function RepeatAttributeTemplate() {
    var el = simpledoc.getElementById("FindSubRepeater").cloneNode(true);
    var repeater = DomService.GetChildElements(el);
    var data = {};
    TemplateManager.SetTemplateAttribte(repeater[0], repeater.slice(1), data, null);

    var htmle = repeater[0].outerHTML;

    expect(htmle.indexOf("k-attributes") > -1).to.eql(true); 
}

function RepeatTemplateSimple() {
    var el = simpledoc.getElementById("TemplateSimple");
    var repeater = DomService.GetChildElements(el);
    var template = TemplateManager.GetTemplate(repeater);

    var body = template.HtmlBody;

    expect(body.indexOf("k-content") == -1 && body.indexOf("k-repeat") == -1).to.eql(false);
}

function RepeatTemplateSimpleData() {
    var el = simpledoc.getElementById("TemplateSimple");
    var repeater = DomService.GetChildElements(el);
    var template = TemplateManager.GetTemplate(repeater);

    var data = template.Data;
    var convert = _.toArray(data);

    expect(convert.length).to.eql(3);
    expect(convert[0]["title"]).to.eql("title 111");
}

function RepeatTemplateMiddleContent() {
   var el = simpledoc.getElementById("TemplateWithMiddleContent");
    var repeater = DomService.GetChildElements(el);

    var template = TemplateManager.GetTemplate(repeater);
    expect(template.HtmlBody.indexOf("k-replace")>-1).to.eql(true);
}

function RepeatTemplateMiddleContentData() {
    var el = simpledoc.getElementById("TemplateWithMiddleContent");
    var repeater = DomService.GetChildElements(el);
    var template = TemplateManager.GetTemplate(repeater);
    var convert = _.toArray(template.Data);  
    expect(convert[0]["content"]).to.eql("<p><i>a</i>some text here</p>");
}

function RepeatTemplateWithAttributeContent() {
    var el = simpledoc.getElementById("TemplateWithAttributeContent");
    var repeater = DomService.GetChildElements(el);
    var template = TemplateManager.GetTemplate(repeater);
    var data = template.Data;
    var convert = _.toArray(data); 
    expect(convert[0]["content"]).to.eql("one");
    
}

function FindSubRepeater() {

    var element = simpledoc.getElementById("FindSubRepeater");
    var subrepeater = Kooboo.Repeater.findSubRepeater(element);
    _validDivRepeater(subrepeater, "FindSubRepeater");

    var elementWithoutClass = simpledoc.getElementById("FindSubRepeaterWithoutClass");

    var divsubs = Kooboo.Repeater.findSubRepeater(elementWithoutClass);

    expect(divsubs.length != 3 || divsubs[0].tagName != "DIV" || divsubs[1].tagName != "DIV" || divsubs[2].tagName != "DIV").to.eql(false);
}

function FindSuperRepeater() {

    var element = simpledoc.getElementById("FindSuperRepeater");

    var superRepeater = Kooboo.Repeater.findSuperRepeater(element);

    _validDivRepeater(superRepeater, "FindSuperRepeater");
}

function FindSubRepeaterShouldIgnoreScriptStyle() {
  
    var element = simpledoc.getElementById("SubRepeaterWithScript");
    var subrepeater = Kooboo.Repeater.findSubRepeater(element);

    expect(subrepeater.length).to.eql(3);

}

function FindTableSubRepeater() {

    var element = simpledoc.getElementById("FindSubTableRepaeter");

    var superRepeater = Kooboo.Repeater.findSubRepeater(element);

    _validTableSubRepeater(superRepeater, "FindSubTableRepaeter");
}

function FindTdSuperRepeater1() {

    var element = simpledoc.getElementById("FindSuperTableRepaeter1");

    var superRepeater = Kooboo.Repeater.findSuperRepeater(element);

    _validTdSuperRepeater1(superRepeater, "FindTdSuperRepeater1");
}

function FindTdSuperRepeater2() {

    var element = simpledoc.getElementById("FindSuperTableRepaeter2");

    var superRepeater = Kooboo.Repeater.findSuperRepeater(element);

    _validTdSuperRepeater2(superRepeater, "FindTdSuperRepeater2");
}

function GetSubRepeaterByPosition() {

    var element = simpledoc.getElementById("FindSubRepeater");

    var elementWithoutClass = simpledoc.getElementById("FindSubRepeaterWithoutClass");

    var one = RepeaterHelper.GetSubByPosition(element);
    var two = RepeaterHelper.GetSubByPosition(elementWithoutClass);

    expect(one.length != 3 || two.length != 3).to.eql(false);
}

function GetSubRepeaterByExamineTags() {

    var element = simpledoc.getElementById("FindSubRepeater");

    var result = RepeaterHelper.GetSubByExamineTags(element);

    expect(result.length != 3 || result[0].tagName.toLowerCase() != "div").to.eql(false);

}
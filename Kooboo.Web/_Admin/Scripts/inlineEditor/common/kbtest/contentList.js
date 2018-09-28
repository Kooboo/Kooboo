k.setHtml("simplehtml", "contentList.html");
parser = new DOMParser();
simpledoc = parser.parseFromString(simplehtml, "text/html");

function getRepeater_Intd_noInnerList(){
    var el = simpledoc.getElementById("selectorTd_noList");
    var result= Kooboo.converterTypes.ContentList.execute(el);
    expect(Object.keys(result.data).length).to.eql(2);
    expect(result.data[0].content).to.eql("123");
    expect(result.data[1].content).to.eql("abc");
}
function getRepeaterInTable_selectorTd_ul(){
    var el = simpledoc.getElementById("selectorTd_ul");
    var result= Kooboo.converterTypes.ContentList.execute(el);
    expect(Object.keys(result.data).length).to.eql(4);
    expect(result.data[0].content).to.eql("123");
}
function getRepeater_divintable_withUl(){
    var el = simpledoc.getElementById("selectordiv_intable_withul");
    var result= Kooboo.converterTypes.ContentList.execute(el);
    expect(Object.keys(result.data).length).to.eql(4);
    expect(result.data[0].content).to.eql("123");
}

function getRepeaterInTable_selectTr(){
    var el = simpledoc.getElementById("selectorTr");
    var result= Kooboo.converterTypes.ContentList.execute(el);
    expect(Object.keys(result.data).length).to.eql(3);
    expect(result.data[0].content).to.eql("abc");
    expect(result.data[1].content).to.eql("<ul><li>123</li><li>123</li><li>123</li><li>123</li></ul>");
    expect(result.data[2].content).to.eql("abcd");
}
function getRepeater_ul(){
    var el = simpledoc.getElementById("ullist");
    var result= Kooboo.converterTypes.ContentList.execute(el);
    expect(Object.keys(result.data).length).to.eql(4);
    expect(result.data[0].content).to.eql("123");
}

function getRepeater_div_withUl(){
    var el = simpledoc.getElementById("selectordiv_withul");
    var result= Kooboo.converterTypes.ContentList.execute(el);
    expect(Object.keys(result.data).length).to.eql(4);
    expect(result.data[0].content).to.eql("123");
}

function getRepeater_li_inUl(){
    var el = simpledoc.getElementById("selectorli");
    var result= Kooboo.converterTypes.ContentList.execute(el);
    expect(Object.keys(result.data).length).to.eql(4);
    expect(result.data[0].content).to.eql("123");
}

function removeTemplateAttributes(){
    var element = simpledoc.getElementById("template-attribute-li");
    
    var result = Kooboo.converterTypes.ContentList.execute(element); 
    expect(result.htmlBody.indexOf('class="ps"')==-1).to.eql(true);
}


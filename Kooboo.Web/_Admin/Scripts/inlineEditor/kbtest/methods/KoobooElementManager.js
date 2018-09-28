var koobooElementManager = Kooboo.KoobooElementManager;

function resetCloneElementsKoobooId() {
    var doc = document.implementation.createHTMLDocument("");
    doc.write("<html><body><div kooboo-id='1-1'>123</div><div kooboo-id='1-1' id='copy'>123</div><div kooboo-id='1-2'>123</div></body></html>");
    var divs = $("div", doc);
    var copyDiv = $("#copy", doc)[0];

    expect($(copyDiv).attr("kooboo-id")).to.eql("1-1");
    var cloneElements = koobooElementManager.resetCloneElementsKoobooId(divs);
    expect($(copyDiv).attr("kooboo-id")).to.eql("1-3");
}

function getCloneElements() {
    var doc = document.implementation.createHTMLDocument("");
    doc.write("<html><body><div kooboo-id='1-1'>123</div><div kooboo-id='1-1'>123</div><div kooboo-id='1-2'>123</div></body></html>");
    var divs = $("div", doc);
    var cloneElements = koobooElementManager.getCloneElements(divs);
    expect(cloneElements.length).to.eql(1);
    expect($(cloneElements[0]).attr("kooboo-id")).to.eql("1-1");
}

function resetNewElementsKoobooId() {
    var doc = document.implementation.createHTMLDocument("");
    doc.write("<html><body><div id='test' kooboo-id='1-1'><div id='childDiv'>123</div></div></body></html>");
    var el = $("#test", doc)[0];
    var childrens = el.children;
    koobooElementManager.resetNewElementsKoobooId(childrens);

    var koobooId = $("#childDiv", doc).attr("kooboo-id");
    expect(koobooId).to.eql("1-1-1");
}

function resetElementKoobooId() {
    var doc = document.implementation.createHTMLDocument("");
    doc.write("<html><body><div id='test' kooboo-id='1-1'>123</div></body></html>");
    var el = $("#test", doc)[0];
    var cloneEl = $(el).clone();
    $(cloneEl).insertAfter($(el));
    koobooElementManager.resetElementKoobooId(cloneEl[0]);
    var koobooId = $(cloneEl).attr("kooboo-id");
    expect(koobooId).to.eql("1-2");

    doc = document.implementation.createHTMLDocument("");
    doc.write("<html><body><div id='test' kooboo-id='1-1'><span kooboo-id='1-1-1'>123</span></div></body></html>");
    var el = $("#test", doc)[0];
    var cloneEl = $(el).clone();
    $(cloneEl).insertAfter($(el));
    koobooElementManager.resetElementKoobooId(cloneEl[0]);
    var koobooId = $(cloneEl).attr("kooboo-id");
    expect(koobooId).to.eql("1-2");

    var span = $(cloneEl).find("span")[0];
    var spankoobooId = $(span).attr("kooboo-id");
    expect(spankoobooId).to.eql("1-2-1");
}

function resetElementKoobooId_OnlyOneElement() {
    var doc = document.implementation.createHTMLDocument("");
    doc.write('<a kooboo-id="2-1" href="/index.aspx"><div id="test" style="height: 95px; width: 150px; color: black;">sfsfsfds</div></a>');
    var el = $("#test", doc)[0];
    var cloneEl = $(el).clone();
    $(cloneEl).insertAfter($(el));
    koobooElementManager.resetElementKoobooId(cloneEl[0]);
    var koobooId = $(cloneEl).attr("kooboo-id");
    expect(koobooId).to.eql("2-1-1");
}

function getNewKoobooId() {
    var koobooId = "";
    var newKoobooId = koobooElementManager.getNewKoobooId(koobooId);
    expect(newKoobooId).to.eql("");

    var koobooId = "1-1";
    var newKoobooId = koobooElementManager.getNewKoobooId(koobooId);
    expect(newKoobooId).to.eql("1-2");

    var koobooId = "1";
    var newKoobooId = koobooElementManager.getNewKoobooId(koobooId);
    expect(newKoobooId).to.eql("2");

    var koobooId = "1-2-3";
    var newKoobooId = koobooElementManager.getNewKoobooId(koobooId);
    expect(newKoobooId).to.eql("1-2-4");
}

function getMaxKoobooIdOfSiblingElement() {
    var doc = document.implementation.createHTMLDocument("");
    doc.write("<html><body><div id='test' kooboo-id='1-1'>123</div></body></html>");
    var el = $("#test", doc)[0];
    var cloneEl = $(el).clone();
    $(cloneEl).insertAfter($(el));

    var maxKooboId = koobooElementManager.getMaxKoobooIdOfSiblingElement(cloneEl[0]);
    expect(maxKooboId).to.eql("1-1");

    doc.write("<html><body><div id='test' kooboo-id='1-1'>123</div><div kooboo-id='1-2'></div></body></html>");
    var el = $("#test", doc)[0];
    var cloneEl = $(el).clone();
    $(cloneEl).insertAfter($(el));

    var maxKooboId = koobooElementManager.getMaxKoobooIdOfSiblingElement(cloneEl[0]);
    expect(maxKooboId).to.eql("1-2");

    doc.write("<html><body><div id='test' kooboo-id='1-1'>123</div><div kooboo-id='1-2'></div><div kooboo-id='1-3'></div></body></html>");
    var el = $("#test", doc)[0];
    var cloneEl = $(el).clone();
    $(cloneEl).insertAfter($(el));

    var maxKooboId = koobooElementManager.getMaxKoobooIdOfSiblingElement(cloneEl[0]);
    expect(maxKooboId).to.eql("1-3");
}


function ismatchNameAndType() {

    var isMatch = koobooElementManager.ismatchNameAndType("info", "view", "info", "view");
    expect(isMatch).to.eql(true);
    var isMatch = koobooElementManager.ismatchNameAndType("1-1", "page", "info", "view");
    expect(isMatch).to.eql(false);
}
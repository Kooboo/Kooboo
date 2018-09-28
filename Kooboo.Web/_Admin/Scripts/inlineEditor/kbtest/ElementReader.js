

var doc = document.implementation.createHTMLDocument();
var header = "#kooboo--objecttype='page'--nameorid='45b7adb2-9466-413a-9b6d-d648aab2f5ec'";
var page = "<!--#kooboo--objecttype='form'--nameorid='68951602-5262-04ab-8fe5-c77588b90dde'--boundary='115'-->" +
    "<div id='main' kooboo-id='1'>" +
    "<div id='header' kooboo-id='1-1'>" +
    "<p kooboo-id='1-1-1'></p>" +
    "</div>" +
    "</div>" +
    "<!--#kooboo--end='true'--objecttype='form'--boundary='115'-->";

var comment = doc.createComment(header);
doc.appendChild(comment);


doc.body.innerHTML += "<div id='frame'></div";
var frame = doc.getElementById('frame');
frame.innerHTML = page;

function elementReaderReadObject() {

    var result = Kooboo.elementReader.readObject(doc.getElementById('header'));
    expect(result.koobooId).to.be("1-1");
    expect(result.pageId).to.be("45b7adb2-9466-413a-9b6d-d648aab2f5ec");
    expect(result.koobooObjects.length).to.be(2);
}

function elementReaderGetUpResult() {
    
    var result = Kooboo.elementReader.getUpResult(doc.getElementById('header'));
    expect(result[0].type).to.be("form");
    expect(result[0].boundary).to.be("115");
}

function elementReaderGetDownResult() {
    
    var result = Kooboo.elementReader.getDownResult(doc.getElementById('header'));
    expect(result[0].type).to.be("form");
    expect(result[0].end).to.be("true");
}

function elementReaderGetPageId() {
    
    var result = Kooboo.elementReader.getPageId(doc.getElementById('header'));
    expect(result).to.be("45b7adb2-9466-413a-9b6d-d648aab2f5ec");
}
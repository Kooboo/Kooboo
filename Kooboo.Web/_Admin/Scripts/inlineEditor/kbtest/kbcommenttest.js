 
function commentManagerParsePage() {
 
    var comment_element = {
        data: "<!--#kooboo--objecttype='page'--nameorid='45b7adb2-9466-413a-9b6d-d648aab2f5ec'-->"
    }
    var result = Kooboo.commentManager.Parse(comment_element);
    expect(result.kooboo).to.be("kooboo");
    expect(result.nameorid).to.be("45b7adb2-9466-413a-9b6d-d648aab2f5ec");
    expect(result.objecttype).to.be("page");
}

function commentManagerParseFormStart() {
    
    var comment_element = {
        data: "<!--#kooboo--objecttype='form'--nameorid='68951602-5262-04ab-8fe5-c77588b90dde'--boundary='115'-->"
    }
    var result = Kooboo.commentManager.Parse(comment_element);
    expect(result.kooboo).to.be("kooboo");
    expect(result.nameorid).to.be("68951602-5262-04ab-8fe5-c77588b90dde");
    expect(result.objecttype).to.be("form");
    expect(result.boundary).to.be("115");
}

function commentManagerParseFormEnd() {
 
    var comment_element = {
        data: "<!--#kooboo--end='true'--objecttype='form'--boundary='115'-->"
    }
    var result = Kooboo.commentManager.Parse(comment_element);
    expect(result.kooboo).to.be("kooboo");
    expect(result.objecttype).to.be("form");
    expect(result.boundary).to.be("115");
    expect(result.end).to.be("true");
}
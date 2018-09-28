function directPageIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var a=doc.createElement("a");
    a.setAttribute("href","aa.html");
    doc.body.appendChild(a);
    var context=Kooboo.elementReader.readObject(a);
    
    var isShow= Kooboo.plugins.DirectPage.isShow(context);
    expect(isShow).to.be(true);
}
function directPageIsShow_ExtenalUrl(){
    var doc=document.implementation.createHTMLDocument();
    var a=doc.createElement("a");
    a.setAttribute("href","https://www.baidu.com");
    doc.body.appendChild(a);
    var context=Kooboo.elementReader.readObject(a);
    
    var isShow= Kooboo.plugins.DirectPage.isShow(context);
    expect(isShow).to.be(false);
}
function directPageIsShow_NoLink(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.DirectPage.isShow(context);
    expect(isShow).to.be(false);
}
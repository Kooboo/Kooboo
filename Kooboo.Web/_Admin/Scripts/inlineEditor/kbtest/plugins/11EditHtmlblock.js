function editHtmlblockIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var startComment=doc.createComment("#kooboo--objecttype='htmlblock'--nameorid='HtmlBlock'--boundary='106'");
    doc.body.appendChild(startComment);

    var div=doc.createElement("div");
    doc.body.appendChild(div);

    var endComment=doc.createComment("#kooboo--end='true'--objecttype='htmlblock'--boundary='106'");
    doc.body.appendChild(endComment);
    var context=Kooboo.elementReader.readObject(div);
    
    var isShow= Kooboo.plugins.EditHtmlblock.isShow(context);
    expect(isShow).to.be(true);
}
function editHtmlblockIsshow_hide(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.EditHtmlblock.isShow(context);
    expect(isShow).to.be(false);
}
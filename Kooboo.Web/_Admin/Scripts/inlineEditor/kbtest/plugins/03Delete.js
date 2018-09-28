function DeleteIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.Delete.isShow(context);
    expect(isShow).to.be(true);
}

function DeleteIsShow_isLabel(){
    var doc=document.implementation.createHTMLDocument();
   
    var comment=doc.createComment("#kooboo--objecttype='Label'--attributename='k-label'--bindingvalue='test'--koobooid='1-0-1'");
    doc.body.appendChild(comment);
    
    var h2=doc.createElement("h2");
    h2.setAttribute("kooboo-id","1-0-1");
    h2.setAttribute("k-label","test");
    doc.body.appendChild(h2);

    var element=$("h2",doc)[0];
    var context=Kooboo.elementReader.readObject(element);

    var isShow= Kooboo.plugins.Delete.isShow(context);
    expect(isShow).to.be(true);
}
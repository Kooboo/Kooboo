function EditLinkIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var a=doc.createElement("a");
    a.setAttribute("kooboo-id","1-1");

    doc.body.appendChild(a);
    
    var context=Kooboo.elementReader.readObject(a);
    var isShow= Kooboo.plugins.EditLink.isShow(context);
    expect(isShow).to.be(true);
}

function EditLinkIsShow_ParentIsLink(){
    var doc=document.implementation.createHTMLDocument();
    var a=doc.createElement("a");
    a.setAttribute("kooboo-id","1-1");

    var span=doc.createElement("span");
    span.setAttribute("kooboo-id","1-1-1");
    a.appendChild(span);
    doc.body.appendChild(a);

    var context=Kooboo.elementReader.readObject(span);
    var isShow= Kooboo.plugins.EditLink.isShow(context);
    expect(isShow).to.be(true);
}
function ConverterIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var pageComment=doc.createComment("#kooboo--objecttype='page'--nameorid='d12f60b6-4133-4e13-b4da-ecde9d987069'");
    doc.appendChild(pageComment);


    var p=doc.createElement("p");
    p.setAttribute("kooboo-id","1-1");
    doc.body.appendChild(p);
    var context=Kooboo.elementReader.readObject(p);
    
    var isShow= Kooboo.plugins.Converter.isShow(context);
    expect(isShow).to.be(true);
}

function ConverterIsShow_hide_inPageObject(){
    var doc=document.implementation.createHTMLDocument();
    var startComment=doc.createComment("#kooboo--objecttype='view'--nameorid='View0'--boundary='126'");
    doc.body.appendChild(startComment);

    var div=doc.createElement("div");
    div.setAttribute("kooboo-id","1-1");
    doc.body.appendChild(div);

    var endComment=doc.createComment("#kooboo--end='true'--objecttype='view'--boundary='126'");
    doc.body.appendChild(endComment);
    var context=Kooboo.elementReader.readObject(div);
    
    var isShow= Kooboo.plugins.Converter.isShow(context);
    expect(isShow).to.be(false);
}
function ConverterIsShow_hide_ContainComponent(){
    var doc=document.implementation.createHTMLDocument();

    var div=doc.createElement("div");
    div.setAttribute("kooboo-id","1-1");

    var startComment=doc.createComment("#kooboo--objecttype='htmlblock'--nameorid='HtmlBlock'--boundary='106'");
    div.appendChild(startComment);
    var span=doc.createElement("span");
    div.appendChild(span);
    var endComment=doc.createComment("#kooboo--end='true'--objecttype='htmlblock'--boundary='106'");
    div.appendChild(endComment);
    doc.body.appendChild(div);

    var context=Kooboo.elementReader.readObject(div);

    var isShow= Kooboo.plugins.Converter.isShow(context);
    expect(isShow).to.be(false);
}
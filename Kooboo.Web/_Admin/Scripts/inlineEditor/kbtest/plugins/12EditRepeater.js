function editRepeaterIsShow_isContenteAttribute(){
    var doc=document.implementation.createHTMLDocument();

    var comment=doc.createComment("#kooboo--objecttype='attribute'--nameorid='f4c32a0d-65fe-6079-11ff-c76348bea38e'--attributename='att'--bindingvalue='{ById.title}'--koobooid='1-0-1'");
    doc.body.appendChild(comment);

    var h2=doc.createElement("h2");
    h2.setAttribute("kooboo-id","1-0-1");
    h2.innerHTML="test";
    doc.body.appendChild(h2);

    var context=Kooboo.elementReader.readObject(h2);
    var isShow= Kooboo.plugins.EditRepeater.isShow(context);
    expect(isShow).to.be(true);
}
function editRepeaterIsShow_isContent(){
    var doc=document.implementation.createHTMLDocument();

    var contentComment=doc.createComment("#kooboo--objecttype='content'--nameorid='f4c32a0d-65fe-6079-11ff-c76348bea38e'--bindingvalue='ById.title'--fieldname='title'--koobooid='1-0-1'");
    doc.body.appendChild(contentComment);

    var h2=doc.createElement("h2");
    h2.setAttribute("kooboo-id","1-0-1");
    h2.innerHTML="test";
    doc.body.appendChild(h2);

    var context=Kooboo.elementReader.readObject(h2);
    var isShow= Kooboo.plugins.EditRepeater.isShow(context);
    expect(isShow).to.be(true);
}
function editRepeaterIsShow_isContentRepeater(){
    var doc=document.implementation.createHTMLDocument();
    var startComment=doc.createComment("#kooboo--objecttype='contentrepeater'--nameorid='de68543f-fb0b-11e1-586f-7649a3c8d115'--folderid='4011954e-1d43-3717-126e-4f221d3e7ac9'--bindingvalue='ByFolder_Item'--boundary='106'");
    doc.body.appendChild(startComment);

    var div=doc.createElement("div");
    div.setAttribute("kooboo-id","1-1");
    doc.body.appendChild(div);

    var endComment=doc.createComment("#kooboo--end=true--objecttype='contentrepeater'--boundary='106'");
    doc.body.appendChild(endComment);
    var context=Kooboo.elementReader.readObject(div);
    var isShow= Kooboo.plugins.EditRepeater.isShow(context);
    expect(isShow).to.be(true);
}

function editRepeaterIsShow_notshow(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.EditRepeater.isShow(context);
    expect(isShow).to.be(false);
}
function RemoveRepeaterIsShow_isRepeater(){
    var doc=document.implementation.createHTMLDocument();
    var startComment=doc.createComment("#kooboo--objecttype='contentrepeater'--nameorid='de68543f-fb0b-11e1-586f-7649a3c8d115'--folderid='4011954e-1d43-3717-126e-4f221d3e7ac9'--bindingvalue='ByFolder_Item'--boundary='106'");
    doc.body.appendChild(startComment);

    var div=doc.createElement("div");
    div.setAttribute("kooboo-id","1-1");
    doc.body.appendChild(div);

    var endComment=doc.createComment("#kooboo--end=true--objecttype='contentrepeater'--boundary='106'");
    doc.body.appendChild(endComment);
    var context=Kooboo.elementReader.readObject(div);
    var isShow= Kooboo.plugins.RemoveRepeater.isShow(context);
    expect(isShow).to.be(true);


    //content field
    var contentComment=doc.createComment("#kooboo--objecttype='content'--nameorid='de68543f-fb0b-11e1-586f-7649a3c8d115'--bindingvalue='ByFolder_Item.title'--fieldname='title'--koobooid='1-0-1'");
    div.appendChild(contentComment);
    
    var h2=doc.createElement("h2");

    h2.innerHTML="test";
    div.appendChild(h2);
    context=Kooboo.elementReader.readObject(h2);
    var isShow= Kooboo.plugins.RemoveRepeater.isShow(context);
    expect(isShow).to.be(true);
}

function RemoveRepeaterIsShow_notRepeater(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.RemoveRepeater.isShow(context);
    expect(isShow).to.be(false);
}
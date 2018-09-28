function EditColorIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var p=doc.createElement("p");
    p.setAttribute("kooboo-id","1-1");

    doc.body.appendChild(p);

    var context=Kooboo.elementReader.readObject(p);
    var isShow= Kooboo.plugins.EditColor.isShow(context);
    expect(isShow).to.be(true);
}

function EditColorIsShow_img_notShow(){
    var doc=document.implementation.createHTMLDocument();
    var img=doc.createElement("img");
    img.setAttribute("kooboo-id","1-1");

    doc.body.appendChild(img);

    var context=Kooboo.elementReader.readObject(img);
    var isShow= Kooboo.plugins.EditColor.isShow(context);
    expect(isShow).to.be(false);
}
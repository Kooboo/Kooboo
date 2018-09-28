function EditMenuIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var startComment=doc.createComment("#kooboo--objecttype='menu'--nameorid='Menu'--boundary='105'");
    doc.body.appendChild(startComment);

    var p=doc.createElement("p");
    p.innerHTML="test";
    doc.body.appendChild(p);

    var endComment=doc.createComment("#kooboo--end='true'--objecttype='menu'--boundary='105'");
    doc.body.appendChild(endComment);

    var context=Kooboo.elementReader.readObject(p);
    var isShow= Kooboo.plugins.EditMenu.isShow(context);
    expect(isShow).to.be(true);
}
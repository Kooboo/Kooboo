function ReplaceWithTextIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var img=doc.createElement("img");
    img.setAttribute("kooboo-id","1-1");
    doc.body.appendChild(img);

    var element=$("img",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.ReplaceWithText.isShow(context);
    expect(isShow).to.be(true);
}

function ReplaceWithTextIsShow_image(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.ReplaceWithText.isShow(context);
    expect(isShow).to.be(false);
}
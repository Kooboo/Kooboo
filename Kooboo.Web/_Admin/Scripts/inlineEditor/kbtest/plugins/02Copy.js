function CopyisShow(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.Copy.isShow(context);
    expect(isShow).to.be(true);
}

function CopyisShow_isBody(){
    var doc=document.implementation.createHTMLDocument();
    doc.body.setAttribute("kooboo-id","1-1");

    var element=doc.body;
   
    var context=Kooboo.elementReader.readObject(element);
    var isShow= Kooboo.plugins.Copy.isShow(context);
    expect(isShow).to.be(false);
}

function CopyisShow_isCanOperateType(){
    var doc=document.implementation.createHTMLDocument();
    var startComment=doc.createComment("#kooboo--objecttype='view'--nameorid='View'--boundary='104'");
    doc.body.appendChild(startComment);

    var div=doc.createElement("div");
    div.setAttribute("kooboo-id","1-1");
    doc.body.appendChild(div);

    var p=doc.createElement("p");
    p.setAttribute("kooboo-id","1-1-1");
    div.appendChild(p);


    var endcomment=doc.createComment("#kooboo--end='true'--objecttype='view'--boundary='104'");
    doc.body.appendChild(endcomment);

    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.Copy.isShow(context);
    expect(isShow).to.be(true);
}
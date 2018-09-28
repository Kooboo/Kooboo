function editTreeDataIsShow(){
    var doc=document.implementation.createHTMLDocument();

    var ul=doc.createElement("ul");
    ul.setAttribute("kooboo-id","1-1");

    var li1=doc.createElement("li");
    li1.setAttribute("kooboo-id","1-1-1");
    li1.innerHTML="li1";

    var li2=doc.createElement("li");
    li1.setAttribute("kooboo-id","1-1-2");
    li1.innerHTML="li1";

    ul.appendChild(li1);
    ul.appendChild(li2);

    doc.body.appendChild(ul);

    var context=Kooboo.elementReader.readObject(li1);

    var isShow= Kooboo.plugins.EditTreeData.isShow(context);
    
    expect(isShow).to.be(true);
}

function editTreeDataIsshow_notTree(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    var el=$(html)[0];
    doc.body.appendChild(el);

    var context=Kooboo.elementReader.readObject(el);
    var isShow= Kooboo.plugins.EditTreeData.isShow(context);
    expect(isShow).to.be(false);
}

function editTreeDataIsShow_noinnerHtml(){
    var doc=document.implementation.createHTMLDocument();

    var ul=doc.createElement("ul");
    ul.setAttribute("kooboo-id","1-1");

    var li1=doc.createElement("li");
    li1.setAttribute("kooboo-id","1-1-1");

    var li2=doc.createElement("li");
    li1.setAttribute("kooboo-id","1-1-2");


    ul.appendChild(li1);
    ul.appendChild(li2);

    doc.body.appendChild(ul);

    var context=Kooboo.elementReader.readObject(li1);

    var isShow= Kooboo.plugins.EditTreeData.isShow(context);
    
    expect(isShow).to.be(false);
}
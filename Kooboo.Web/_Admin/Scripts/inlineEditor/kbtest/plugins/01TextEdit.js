function TextEditIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
    
    var isShow= Kooboo.plugins.TextEdit.isShow(context);
    expect(isShow).to.be(true);
}

function TextEditIsShow_NoKoobooId(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p>123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);

    var isShow= Kooboo.plugins.TextEdit.isShow(context);
    expect(isShow).to.be(false);
}
function TextEditIsShow_NoNameIdContent(){
    var doc=document.implementation.createHTMLDocument();

    var comment= doc.createComment("#kooboo--objecttype='content'--bindingvalue='ListByCategoryId.Count()'--fieldname='Count()'--koobooid='1-1'");
    doc.body.appendChild(comment);
    var p=doc.createElement("p");
    p.setAttribute("kooboo-id","1-1");
    doc.body.appendChild(p);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);

    var isShow= Kooboo.plugins.TextEdit.isShow(context);
    expect(isShow).to.be(false);
}

function TextEditIsShow_isImage(){
    var doc=document.implementation.createHTMLDocument();
    var img=doc.createElement("img");
    img.setAttribute("kooboo-id","1-1");
    doc.body.appendChild(img);

    var element=$("img",doc)[0];
    var context=Kooboo.elementReader.readObject(element);

    var isShow= Kooboo.plugins.TextEdit.isShow(context);
    expect(isShow).to.be(false);
}

function TextEditIsShow_ContainDynamicData(){
    var doc=document.implementation.createHTMLDocument();
    
    var div=doc.createElement("div");
    div.setAttribute("kooboo-id","1-1");


    var startComment=doc.createComment("#kooboo--objecttype='htmlblock'--nameorid='HtmlBlock'--boundary='101'");
    div.appendChild(startComment);

    var p=doc.createElement("p");
    p.innerHTML="test";
    div.appendChild(p);

    var endComment=doc.createComment("#kooboo--end='true'--objecttype='htmlblock'--boundary='101'");
    div.appendChild(endComment);

    doc.body.appendChild(div);

    var element=$("div",doc)[0];
    var context=Kooboo.elementReader.readObject(element);
   
    var isShow= Kooboo.plugins.TextEdit.isShow(context);
    expect(isShow).to.be(false);
    
}

function textEditIsShow_BindVariable(){
    Kooboo.VariableDataManager.setModel("BindVariable");
 
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);

    var isShow= Kooboo.plugins.TextEdit.isShow(context);
    expect(isShow).to.be(false);

    Kooboo.VariableDataManager.setModel("EditHtml");
}

function textEditIsShow_element_withoutKoobooId_inLable(){
    var doc=document.implementation.createHTMLDocument();
    
    var div=doc.createElement("div");
    div.setAttribute("kooboo-id","1-1");


    var startComment=doc.createComment("<!--#kooboo--objecttype='Label'--attributename='k-label'--bindingvalue='home-title-2'--koobooid='1-0-1-1-1-1-1-3-1'-->");
    div.appendChild(startComment);

    var span=doc.createElement("span");
    span.setAttribute("kooboo-id","1-0-1-1-1-1-1-3-1");
    div.appendChild(span);

    var innerSpan=doc.createElement("span");
    span.innerHTML="Koobooabc";
    span.appendChild(innerSpan);

    var context=Kooboo.elementReader.readObject(innerSpan);
    var isShow= Kooboo.plugins.TextEdit.isShow(context);

    expect(isShow).to.be(true);
}

function textEditIsShow_element_withKoobooIdOfLabel(){
    var doc=document.implementation.createHTMLDocument();
    
    var div=doc.createElement("div");
    div.setAttribute("kooboo-id","1-1");


    var startComment=doc.createComment("<!--#kooboo--objecttype='Label'--attributename='k-label'--bindingvalue='home-title-2'--koobooid='1-0-1-1-1-1-1-3-1'-->");
    div.appendChild(startComment);

    var span=doc.createElement("span");
    span.setAttribute("kooboo-id","1-0-1-1-1-1-1-3-1");
    div.appendChild(span);

    var context=Kooboo.elementReader.readObject(span);
    var isShow= Kooboo.plugins.TextEdit.isShow(context);

    expect(isShow).to.be(true);
}

function textEditIsShow_element_withoutKoobooId_notLable(){
    var doc=document.implementation.createHTMLDocument();
    
    var div=doc.createElement("div");
    div.setAttribute("kooboo-id","1-1");


    var span=doc.createElement("span");
    div.appendChild(span);

    var context=Kooboo.elementReader.readObject(span);
    var isShow= Kooboo.plugins.TextEdit.isShow(context);

    expect(isShow).to.be(false);
}
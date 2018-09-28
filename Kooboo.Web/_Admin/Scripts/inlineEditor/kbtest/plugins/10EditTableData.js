function editTableIsShow(){
    var doc=document.implementation.createHTMLDocument();
    var html="<table kooboo-id='1-1'><tr kooboo-id='1-1'><td kooboo-id='1-1-1'>222</td></tr></table>";
    var el=$(html)[0];
    doc.body.appendChild(el);

    var context=Kooboo.elementReader.readObject(el);
    var isShow= Kooboo.plugins.EditTableData.isShow(context);
    
    expect(isShow).to.be(true);
}

function editTableIsShow_notTable(){
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    var el=$(html)[0];
    doc.body.appendChild(el);

    var context=Kooboo.elementReader.readObject(el);
    var isShow= Kooboo.plugins.EditTableData.isShow(context);
    expect(isShow).to.be(false);
}
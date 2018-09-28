function BindVariableIsShow(){
    Kooboo.VariableDataManager.setModel("BindVariable");
 
    var doc=document.implementation.createHTMLDocument();
    var html='<p kooboo-id="1-1">123</p>';
    doc.write(html);
    var element=$("p",doc)[0];
    var context=Kooboo.elementReader.readObject(element);

    var isShow= Kooboo.plugins.BindVariable.isShow(context);
    expect(isShow).to.be(true);

    Kooboo.VariableDataManager.setModel("EditHtml");
}
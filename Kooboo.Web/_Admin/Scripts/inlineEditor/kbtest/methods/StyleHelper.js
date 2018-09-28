//------background image start------
function getElementBackgroundImage(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' style='background-image:url(aa.png)'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);

    var backgroundImage=style.backgroundImage;
    expect(style.backgroundImage.property).to.eql("background-image");
    expect(style.backgroundImage.value.indexOf("aa.png")).to.be.greaterThan(-1);
}

function getElementBackgroundImage_noImage(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);

    var backgroundImage=style.backgroundImage;
    expect(style.backgroundImage.property).to.eql("background-image");
    expect(style.backgroundImage.value).to.eql("");
}
//------background image end------

//------color start------
function getRequireColorProperties(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    expect(style.colorItems.length).to.eql(2);
    expect(style.colorItems[0].property).to.eql("background-color");
    expect(style.colorItems[0].jsonRule.isNewRule).to.eql(undefined);//default background color is transparent.
    expect(style.colorItems[0].value).to.eql("");
    
    expect(style.colorItems[1].property).to.eql("color");
    expect(style.colorItems[1].jsonRule.isNewRule).to.eql(true);//new generate rule
    expect(style.colorItems[1].value).to.eql("rgb(0, 0, 0)");

}
function getElementBackgroundColor_inline(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li' style='background-color:blue'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);

    expect(style.colorItems.length).to.eql(2);
    expect(style.colorItems[0].property).to.eql("background-color");
    expect(style.colorItems[1].property).to.eql("color");
    var coloritem=style.colorItems[0];
    expect(coloritem.value).to.eql("blue");

}
function getElementBackgroundColor(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var style=iframeDoc.createElement("style");
    style.type= 'text/css';
    style.appendChild(iframeDoc.createTextNode("ul li.li{background-color:blue;}"));
    iframeDoc.head.appendChild(style);

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);

    expect(style.colorItems.length).to.eql(2);
    expect(style.colorItems[0].property).to.eql("background-color");
    expect(style.colorItems[1].property).to.eql("color");
    var coloritem=style.colorItems[0];
    expect(coloritem.value).to.eql("blue");
;
}

function getElementColor_inline(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li' style='color:blue'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);

    expect(style.colorItems.length).to.eql(2);
    expect(style.colorItems[0].property).to.eql("background-color");
    expect(style.colorItems[1].property).to.eql("color");
    var coloritem=style.colorItems[1];
    expect(coloritem.value).to.eql("blue");

}
function getElementColor(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var style=iframeDoc.createElement("style");
    style.type= 'text/css';
    style.appendChild(iframeDoc.createTextNode("ul li.li{color:blue;}"));
    iframeDoc.head.appendChild(style);

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);

    expect(style.colorItems.length).to.eql(2);
    expect(style.colorItems[0].property).to.eql("background-color");
    expect(style.colorItems[1].property).to.eql("color");
    var coloritem=style.colorItems[1];
    expect(coloritem.value).to.eql("blue");

}

//------color end------

//------font start------
function getElementFontFamily_inline(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li' style='font-family:sans-serif'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    var fontFamily=style.font.fontFamily;
    expect(fontFamily.property).to.eql("font-family");
    expect(fontFamily.value).to.eql("sans-serif");

}

function getElementFontFamily(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var s=iframeDoc.createElement("style");
    s.type= 'text/css';
    s.appendChild(iframeDoc.createTextNode("ul li.li{font-family:sans-serif}"));
    iframeDoc.head.appendChild(s);

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    var fontFamily=style.font.fontFamily;
    expect(fontFamily.property).to.eql("font-family");
    expect(fontFamily.value).to.eql("sans-serif");
}
function getElementFontSize_inline(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li' style='font-size:16px'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    var fontSize=style.font.fontSize;
    expect(fontSize.property).to.eql("font-size");
    expect(fontSize.value).to.eql("16px");
}
function getElementFontSize(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var s=iframeDoc.createElement("style");
    s.type= 'text/css';
    s.appendChild(iframeDoc.createTextNode("ul li.li{font-size:16px}"));
    iframeDoc.head.appendChild(s);

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    var fontSize=style.font.fontSize;
    expect(fontSize.property).to.eql("font-size");
    expect(fontSize.value).to.eql("16px");
}

function getElementFontWeight_inline(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li' style='font-weight:bold'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    var fontWeight=style.font.fontWeight;
    expect(fontWeight.property).to.eql("font-weight");
    expect(fontWeight.value).to.eql("bold");
}
function getElementFontWeight(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var s=iframeDoc.createElement("style");
    s.type= 'text/css';
    s.appendChild(iframeDoc.createTextNode("ul li.li{font-weight:bold;}"));
    iframeDoc.head.appendChild(s);

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    var fontWeight=style.font.fontWeight;
    expect(fontWeight.property).to.eql("font-weight");
    expect(fontWeight.value).to.eql("bold");
}

function getElementFontWeight_noStyle(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    var fontWeight=style.font.fontWeight;
    
    expect(fontWeight.property).to.eql("font-weight");
    expect(fontWeight.jsonRule.isNewRule).to.eql(true);
    
    //default fontweight is 400
    expect(Kooboo.style.StyleEditorHelper.isNormalFontWeight(fontWeight.value)).to.eql(true);
}

function getElementFontStyle_inline(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li' style='font-style:italic'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);

    var fontStyle=style.font.fontStyle;
    expect(fontStyle.property).to.eql("font-style");
    expect(fontStyle.value).to.eql("italic");
}

function getElementFontStyle(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var s=iframeDoc.createElement("style");
    s.type= 'text/css';
    s.appendChild(iframeDoc.createTextNode("ul li.li{font-style:italic;}"));
    iframeDoc.head.appendChild(s);

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    var fontStyle=style.font.fontStyle;
    expect(fontStyle.property).to.eql("font-style");
    expect(fontStyle.value).to.eql("italic");
}

function getElementFontStyle_noStyle(){
    var iframe=Kooboo.TestHelper.createIframe();
    var iframeDoc = iframe.contentDocument;
    var html="<ul kooboo-id='1-1'><li kooboo-id='1-1-1' class='li'>123</li></ul>";
    iframeDoc.body.innerHTML=html;

    var el=$("li",iframeDoc)[0];
    var style=Kooboo.StyleHelper.getStyle(el);
    
    var fontStyle=style.font.fontStyle;
    expect(fontStyle.property).to.eql("font-style");
    expect(fontStyle.jsonRule.isNewRule).to.eql(true);
    expect(fontStyle.value).to.eql("normal");
}
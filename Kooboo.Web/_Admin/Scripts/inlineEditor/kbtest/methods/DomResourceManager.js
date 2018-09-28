
var DomResourceManager = Kooboo.DomResourceManager;


// function _isDataImage() {
//     var url = "data:image";
//     var isDataImage = DomResourceManager._isDataImage(url);
//     expect(isDataImage).to.eql(true);
//     var url = "1.png";
//     var isDataImage = DomResourceManager._isDataImage(url);
//     expect(isDataImage).to.eql(false);
// }

// function isUrlImage() {
//     var imageUrl = "1.png";
//     var isUrlImage = DomResourceManager.isUrlImage(imageUrl);
//     expect(isUrlImage).to.eql(true);

//     imageUrl = "1.woff";
//     var isUrlImage = DomResourceManager.isUrlImage(imageUrl);
//     expect(isUrlImage).to.eql(false);
// }

function isImage_DataImage(){
    var url = "data:image";
    var isDataImage = DomResourceManager.isImage(url);
    expect(isDataImage).to.eql(true);
}

function isImage_UrlImage(){
    var imageUrl = "1.png";
    var isUrlImage = DomResourceManager.isImage(imageUrl);
    expect(isUrlImage).to.eql(true);

    
}
function isImage_notImage(){
    imageUrl = "1.woff";
    var isUrlImage = DomResourceManager.isImage(imageUrl);
    expect(isUrlImage).to.eql(false);
}

function _getInlineStyleImages() {
    // return;
    // var array = [];

    // var inlineStyleImagehtml = "<div  style=\"background-image:url(3.png)\" >3.png</div>" +
    //     "<div style=\"background-image:url(4.png)\" >4.png</div>" +
    //     "<div style=\"background-image:url(../5.png)\" >5.png</div>" +
    //     "<div style=\"background-image:url(./6.png)\" >6.png</div>" +
    //     "<div style=\"background-image:url(/7.png)\" >6.png</div>" +
    //     "<div style=\"background-image:url(4.png)\" >4.png</div>";
    // array.push("<html><body>");
    // //array.push(imageHtml);
    // array.push(inlineStyleImagehtml);
    // array.push("</body></html>");

    // var html = array.join(" ");
    // document.write(html);

    // debugger;
    // var styleImages = DomResourceManager.getStyleImages(document, "", "/page/test.html");
    // //删除tynymce图片的干扰
    // styleImages = _.filter(styleImages, function(image) { return image.indexOf("tinymce") == -1 })

    // expect(styleImages.length).toEqual(5);
    // expect(styleImages[0]).toEqual("/page/3.png");
    // expect(styleImages[1]).toEqual("/page/4.png");
    // expect(styleImages[2]).toEqual("/5.png");
    // expect(styleImages[3]).toEqual("/page/6.png");
    // expect(styleImages[4]).toEqual("/7.png");

    // $("body").empty();
}

function getStyleImageUrl() {
    var cssText = "background: #00FF00 url(bgimage.gif) no-repeat fixed top;";
    var imageurl = DomResourceManager.getStyleImageUrl(cssText);
    expect(imageurl).to.eql("bgimage.gif");
    cssText = "url(\"bgimage.gif\")";
    var imageurl = DomResourceManager.getStyleImageUrl(cssText);
    expect(imageurl).to.eql("bgimage.gif");

    cssText = "url('bgimage.gif')";
    var imageurl = DomResourceManager.getStyleImageUrl(cssText);
    expect(imageurl).to.eql("bgimage.gif");

    cssText = "url(bgimage.gif)";
    var imageurl = DomResourceManager.getStyleImageUrl(cssText);
    expect(imageurl).to.eql("bgimage.gif");

    cssText = "data:image"
    var imageurl = DomResourceManager.getStyleImageUrl(cssText);
    expect(imageurl).to.eql("");
    imageurl = DomResourceManager.getStyleImageUrl("");
    expect(imageurl).to.eql("");
}

function isImageFromContentAttr() {
    var html = "<html><body><img id='img1' src='a.png' /><img id='img2' k-attributes='src xx.xx' src='b.png' /><img id='img3' k-attributes='xx xx.src;' src='b.png' /><img id='img4' k-attributes='xx xx,src xx.xx;' src='b.png' />" +
        "<div id='testdiv'  k-attributes='style background-image:url(xx.xx)'>111</div></body></html>"
    var doc = document.implementation.createHTMLDocument('');
    doc.write(html);
    var img1 = $("#img1", doc)[0];
    var isContentAttr = DomResourceManager.isImageFromContentAttr(img1);
    expect(isContentAttr).to.eql(false);
    var img2 = $("#img2", doc)[0];
    var isContentAttr = DomResourceManager.isImageFromContentAttr(img2);
    expect(isContentAttr).to.eql(true);

    var img3 = $("#img3", doc)[0];

    var isContentAttr = DomResourceManager.isImageFromContentAttr(img3);
    expect(isContentAttr).to.eql(false);

    var img4 = $("#img4", doc)[0];
    var isContentAttr = DomResourceManager.isImageFromContentAttr(img4);
    expect(isContentAttr).to.eql(true);

    var div = $("#testdiv", doc)[0];
    var isContentAttr = DomResourceManager.isImageFromContentAttr(div, true);
    expect(isContentAttr).to.eql(true);

}

function getImageUrl() {
    var url = "data:image";
    expect(DomResourceManager.getImageUrl(url)).to.eql("data:image");
    var baseurl = "http://resume.kooboo:81";
    var page = "http://resume.kooboo:81/test/test.html";
    var imageUrl = "/images/a.png";
    expect(DomResourceManager.getImageUrl(imageUrl, baseurl, page)).to.eql("/images/a.png");

    var url = "11"; //not available imageUrl
    expect(DomResourceManager.getImageUrl(url)).to.eql("");

}

function getImageFullUrl() {
    var baseurl = "http://resume.kooboo:81";
    var styleSheetUrl = "http://resume.kooboo:81/test/style/test.css";
    var imageUrl = "../images/a.png";
    var url = DomResourceManager.getImageFullUrl(baseurl, styleSheetUrl, imageUrl);
    expect(url).to.eql("http://resume.kooboo:81/test/images/a.png");
}

function getImageAbsoluteUrl() {
    window._gl = {};
    var baseurl = "http://resume.kooboo:81";
    var styleSheetUrl = "http://resume.kooboo:81/test/style/test.css";
    var imageUrl = "../images/a.png";

    var url = DomResourceManager.getImageAbsoluteUrl(baseurl, styleSheetUrl, imageUrl);
    expect(url).to.eql("/test/images/a.png");

    var imageUrl = "./images/a.png";
    url = DomResourceManager.getImageAbsoluteUrl(baseurl, styleSheetUrl, imageUrl);
    expect(url).to.eql("/test/style/images/a.png");

    imageUrl = "images/a.png";
    url = DomResourceManager.getImageAbsoluteUrl(baseurl, styleSheetUrl, imageUrl);
    expect(url).to.eql("/test/style/images/a.png");

    imageUrl = "/images/a.png";

    url = DomResourceManager.getImageAbsoluteUrl(baseurl, styleSheetUrl, imageUrl);
    expect(url).to.eql("/images/a.png");

    imageUrl = "http://www.baidu.com/a.png";

    url = DomResourceManager.getImageAbsoluteUrl(baseurl, styleSheetUrl, imageUrl);
    expect(url).to.eql("http://www.baidu.com/a.png");

    imageUrl = "https://www.baidu.com/a.png";

    url = DomResourceManager.getImageAbsoluteUrl(baseurl, styleSheetUrl, imageUrl);
    expect(url).to.eql("https://www.baidu.com/a.png");
}

function convertRgbToHex() {
    var hexColor = DomResourceManager.convertRgbToHex("rgb(255, 255, 0)");
    expect(hexColor).to.eql("#ffff00");
}

function isNeedShowProperty() {
    var style = {
        borderTopWidth: "0px"
    };
    var isShow = DomResourceManager.isNeedShowProperty("border-top-color", style);

    expect(isShow).to.eql(false);
    style = {
        outlineWidth: "0px"
    };
    var isShow = DomResourceManager.isNeedShowProperty("outline-color", style);

    expect(isShow).to.eql(false);

    style = {
        outlineWidth: "2px"
    };
    var isShow = DomResourceManager.isNeedShowProperty("outline-color", style);

    expect(isShow).to.eql(true);
}
k.setHtml("simplehtml", "menutest.html");

parser = new DOMParser();
simpledoc = parser.parseFromString(simplehtml, "text/html");
 
var helper = Kooboo.MenuHelper; 

function TestMenuConvertResult() {
    
    var element = simpledoc.getElementById("regularConvert"); 

    var result = helper.GetClickConvertResult(element);

    expect(result.data.children.length).to.be(3);  
  
}

function ConvertResult_Should_Remove_KoobooId(){
    var element = simpledoc.getElementById("regularConvertWithId");
    var result = helper.GetClickConvertResult(element);
    expect(result.data.children[0].template.indexOf("kooboo")).to.be(-1);

}

function TestIsSubMenuOfParentByPosition() {
    var iframe=Kooboo.TestHelper.createIframe(true);
    var iframeDoc = iframe.contentDocument;
    iframeDoc.body.innerHTML=simpledoc.body.innerHTML;
    
    var container = iframeDoc.getElementById("submenucontainera");

    var sublink = iframeDoc.getElementById("submenudiv");

    var farlink = iframeDoc.getElementById("subfaraway");

    var SubMenuYes = helper.IsSubMenuOfParentByPosition(container, sublink);
    var SubMenuNO = helper.IsSubMenuOfParentByPosition(container, farlink);//error

    expect(SubMenuYes).to.eql(true);
    
    expect(SubMenuNO).to.eql(false);

    var parentlink = iframeDoc.getElementById("submenuaaa");
    
    var ParentSubYes = helper.IsSubMenuOfParentByPosition(parentlink.parentElement, sublink);

    expect(ParentSubYes).to.eql(true);

}

 function TestGetDirectSubLink() {
    var link = simpledoc.getElementById("directsublinkparent");
    var linkcontainer = simpledoc.getElementById("directsublinkcontainer");

    var sublinks = helper.GetDirectSubMenuLinks(link, linkcontainer);

    expect(sublinks.length).to.eql(2);

    var subone = sublinks[0];
    expect(["subone","subtwo"].indexOf(subone.innerText)>-1).to.eql(true);


    var subtwo = simpledoc.getElementById("directsubtwo");
    var subtwocontainer = simpledoc.getElementById("directsubtwocontainer");

    var subsublinks = helper.GetDirectSubMenuLinks(subtwo, subtwocontainer);

    expect(subsublinks.length).to.eql(2);
    var subsubone = subsublinks[0];

    expect(["subsubone","subsubtwo"].indexOf(subsubone.innerText)>-1).to.eql(true);

}

function TestSubMenuByPosition() {
    var iframe=Kooboo.TestHelper.createIframe(true);
    var iframeDoc = iframe.contentDocument;
    iframeDoc.body.innerHTML=simpledoc.body.innerHTML;

    var link = iframeDoc.getElementById("submenuaaa");
    var container = link.parentElement;
    var maincontainer = iframeDoc.getElementById("submenucontainera");
    var sublinks = helper.GetSubMenuLinksByPosition(link, container, maincontainer);

    expect(sublinks.length).to.eql(2);
    Kooboo.TestHelper.removeIframe(iframe);
}

function TestSubMenuByRelateValue() {

    var link = simpledoc.getElementById("relatedwithsub");
    var container = link.parentElement;
    var sublinks = helper.GetSubMenuLinksByRelated(link, container);

    expect(sublinks.length).to.eql(2);
    expect(sublinks[0].innerHTML).to.eql("one");
}

function TestPositionSubMenuLinks() {
    var iframe=Kooboo.TestHelper.createIframe(true);
    var iframeDoc = iframe.contentDocument;
    iframeDoc.body.innerHTML=simpledoc.body.innerHTML;

    var link = iframeDoc.getElementById("nosubmenulinks");
    var container = link.parentElement;
    var maincontainer = container.parentElement;

    var links = helper.GetSubMenuLinksByPosition(link, container, maincontainer);
    
    expect(links).to.eql(null);

    link = iframeDoc.getElementById("linkwithdirectsubmenu");
    container = link.parentElement;
    maincontainer = container.parentElement;
    
    links = helper.GetSubMenuLinksByPosition(link, container, maincontainer);
    
    expect(links.length).to.be.greaterThan(0);
    Kooboo.TestHelper.removeIframe(iframe);

}

function TestRegularMenu() {
    var element = simpledoc.getElementById("regularmenu");

    var menu = helper.GetMenu(element);
    expect(menu.chilren.length).to.eql(3);
    expect(menu.ItemContainer.tagName.toLowerCase()).to.eql("ul");
}

function TestMultipleDirectSubMenu_ul(){
    var element = simpledoc.getElementById("directsubmenu_ul");
    var menu = helper.GetMenu(element);

    expect(menu.ItemContainer.tagName.toLowerCase()).to.eql("ul");
    expect(menu.chilren.length).to.eql(3);

    var firstsubmenu;
    for(var i=0;i<menu.chilren.length;i++){
        var submenu=menu.chilren[i];
        if (submenu.LinkElement.innerText == "aaa") {
            firstsubmenu = submenu;
        }
    }
    expect(firstsubmenu.chilren.length).to.eql(2);

    var subsub = firstsubmenu.chilren[0];
    expect(subsub.chilren.length).to.eql(2);
}

// function TestMultipleDirectSubMenu() {

//     var element = simpledoc.getElementById("linkwithdirectsubmenu");
//     var menu = helper.GetMenu(element);

//     expect(menu.ItemContainer.tagName.toLowerCase()).to.eql("ul");
//     expect(menu.chilren.length).to.eql(3);

//     var firstsubmenu;
//     for(var i=0;i<menu.chilren.length;i++){
//         var submenu=menu.chilren[i];
//         if (submenu.LinkElement.innerText == "aaa") {
//             firstsubmenu = submenu;
//         }
//     }
//     expect(firstsubmenu.chilren.length).to.eql(2);

//     var subsub = firstsubmenu.chilren[0];
//     expect(subsub.chilren.length).to.eql(2);
// }


function TestContainerMenu() {
    var el = simpledoc.getElementById("menucontainer");

    var menu = helper.GetMenuWithinContainer(el);
    expect(menu.chilren.length).to.eql(3);
    expect(menu.chilren[0].chilren.length).to.eql(2);

}

function TestGetJsonData() {
    var el = simpledoc.getElementById("menucontainer");

    var menu = helper.GetMenuWithinContainer(el);

    var data = helper.ConvertToJson(menu);

    var items = data["children"];

    var array = _.toArray(items);

    expect(array.length).to.eql(3);

}

function TestMenuLinkNoCatNoHttp() {

    var el = simpledoc.getElementById("externallinks");

    var menu = helper.GetMenuWithinContainer(el);
    expect(menu).to.eql(null);

    el = simpledoc.getElementById("categorylinks");

    menu = helper.GetMenuWithinContainer(el);

    expect(menu).to.eql(null);

}

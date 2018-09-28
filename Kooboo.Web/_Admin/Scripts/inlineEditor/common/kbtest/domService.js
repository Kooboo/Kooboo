k.setHtml("simplehtml", "domService.html");

parser = new DOMParser();
simpledoc = parser.parseFromString(simplehtml, "text/html");
DomService = Kooboo.NewDomService,
    TagGroup = Kooboo.TagGroup;

function TestRemoveStringKoobooAttribute() {

    var value = "<div kooboo-id='someid'>some text</div>"; 
    var result = DomService.RemoveStringKoobooAttribute(value); 

    expect(result).to.eql("<div>some text</div>")
}

function TestContainCurrentUrl() { 
    KMock(DomService).callFake("getCurrentUrl",function(){
        return DomService.AbsoluteUrl("/domService.html").toLowerCase();
    });
    
    var div = simpledoc.getElementById("containurldiv"); 
    var a = simpledoc.getElementById("containurla"); 

    var divreuslt = DomService.ContainsCurrentUrl(div)
    var aresult = DomService.ContainsCurrentUrl(a);
    var no = simpledoc.getElementById("nocontainurldiv");
    var noresult = DomService.ContainsCurrentUrl(no); 

    expect(divreuslt).to.eql(true);
    expect(aresult).to.eql(true);
    expect(noresult).to.eql(false);
    KMock(DomService).flush();
}

function TestGetActiveClass() {
 
    var div = simpledoc.getElementById("activeclassdiv");
    var div2 = simpledoc.getElementById("activeclassdiv2"); 

    var divchildren = DomService.GetChildElements(div); 
    var div2children = DomService.GetChildElements(div2); 
    
    KMock(DomService).callFake("ContainsCurrentUrl",function(){
        return true;
    })
    var one = DomService.GetActiveClass(divchildren); 

    var two = DomService.GetActiveClass(div2children); 

    expect(one).to.eql("active");
    expect(two).to.eql("item active");
    KMock(DomService).flush();
}

function TestGetElementsByFilter() {
   // GetElementsByFilter
    var fitler = function (item) {
        return (item.id && item.id == "ClickTest");
    }; 

    var elements = DomService.GetElementsByFilter(simpledoc, fitler); 
    expect(elements.length).to.eql(1);
    expect(elements[0].tagName.toLowerCase()).to.eql("div");
}

function TestRemoveKoobooAttribute() {
    var el = simpledoc.getElementById("removekoobooattribute"); 
    removeattribute(el); 

    var a = el.getElementsByTagName("a")[0]; 

    var attribute = a.getAttribute("data-kb-fe-id"); 
    expect(attribute).to.eql(null);
    var img = el.getElementsByTagName("img")[0];

    attribute = img.getAttribute("data-kb-fe-id");
    expect(attribute).to.eql(null);
    
    attribute = img.getAttribute("kooboo-id");
    expect(attribute).to.eql(null);
    function removeattribute(element) {

        DomService.RemoveKoobooAttribute(element); 
        var subs = DomService.GetChildElements(element);
        for(var i=0;i<subs.length;i++){
            removeattribute(subs[i]); 
        }
    }
}

function TestWalkPath() {

    var root = simpledoc.getElementById("walkpath"); 

    var span = simpledoc.getElementById("walkpathspan"); 

    var chain = DomService.GetWalkPath(root, span);
    var back = DomService.GetElementByWalkPath(root, chain); 

    expect(span.isEqualNode(back)).to.eql(true);

    var thirddiv = DomService.GetChildElements(root)[2]; 
    var firsth5 = DomService.GetChildElements(thirddiv)[0]; 

    chain = DomService.GetWalkPath(root, firsth5); 
    back = DomService.GetElementByWalkPath(root, chain); 
     
    expect(firsth5.isEqualNode(back)).to.eql(true);

    var allsubs = DomService.GetChildElements(root); 
    var lastdiv = allsubs[allsubs.length - 1]; 
    var subh5 = DomService.GetChildElements(lastdiv)[0]; 

    chain = DomService.GetWalkPath(root, subh5);
    back = DomService.GetElementByWalkPath(root, chain);

    expect(subh5.isEqualNode(back)).to.eql(true);

}

function TestLinkGroup() {

    var link = simpledoc.getElementById("linkgroupli");

    var group = DomService.FindSameLinkGroup(link);

    expect(group.CommonParent.id).to.eql("ullink");
   
    expect(group.Links.length).to.eql(2);
}

function TestOrderLinkGroup() {

    var link = simpledoc.getElementById("orderedlinkgroup");

    var group = DomService.FindSameLinkGroup(link);
      
    expect(group.Links.length ==3).to.eql(true);
    var second = group.Links[1]; 

    expect(second.id).to.eql("orderedlinkgroup");
    
         
}

function TestPositionNearby() {
    var iframe=Kooboo.TestHelper.createIframe(true);
    var iframeDoc = iframe.contentDocument;
    iframeDoc.body.innerHTML=simpledoc.body.innerHTML;

    var positionone = iframeDoc.getElementById("positionone");
    var positiontwo = iframeDoc.getElementById("positiontwo");
    var positionthree = iframeDoc.getElementById("positionthree");

    var onetwo = DomService.IsNearByElement(positionone, positiontwo);
    var onethree = DomService.IsNearByElement(positionone, positionthree);

    var twothree = DomService.IsNearByElement(positiontwo, positionthree);

    expect(!onetwo || onethree || !twothree).to.eql(false);
    Kooboo.TestHelper.removeIframe(iframe);
}

function TestUpgradeContainer() {

    var upgrade = simpledoc.getElementById("upgrade");
    var nograde = simpledoc.getElementById("noupgrade");

    var one = DomService.UpgradeToContainer(upgrade);
    var two = DomService.UpgradeToContainer(nograde);

    expect(one.tagName.toLowerCase() != "div" || two.tagName.toLowerCase() != "a").to.eql(false);
}

function TestElementInBetween() {

    var linkone = simpledoc.getElementById("betweenone");
    var container = linkone.parentElement;
    var maincontainer = container.parentElement;

    var linktwo = simpledoc.getElementById("betweentwo");
    var containertwo = linktwo.parentElement;

    var inbetweenone = DomService.GetElementsInBetween(container, containertwo, maincontainer, "a");

    expect(inbetweenone.length != 1 && inbetweenone[0].innerText != "sublink").to.eql(false);



    inbetweenone = DomService.GetElementsInBetween(containertwo, null, maincontainer, "a");

    expect(inbetweenone.length != 1 && inbetweenone[0].innerText != "otherlink").to.eql(false);



}

function TestGetDistance() {

    // <p id="clientrectone" > <span id="cleintRectSubOne" > text < /span> between <span id="cleintRectSubTwo">text</span > </p>
    // < p id= "clientrecttwo" > <span id="cleintRectDownOne" > text < /span>between <span id="cleintRectDownTwo">text</span > </p>

    var xelement = simpledoc.getElementById("clientrectone");
    var yelement = simpledoc.getElementById("clientrecttwo");

    var distance = DomService.GetDistance(xelement, yelement);

    var isEqual=distance == (yelement.getBoundingClientRect().top - xelement.getBoundingClientRect().bottom)
    expect(isEqual).to.eql(true);
    

    var one = simpledoc.getElementById("cleintRectSubOne");
    var downone = simpledoc.getElementById("cleintRectDownOne");
    var two = simpledoc.getElementById("cleintRectSubTwo");
    var downtwo = simpledoc.getElementById("cleintRectDownTwo");

    var onetodown = DomService.GetDistance(one, downone);
    var onetwo = DomService.GetDistance(one, two);
    var onedonwtwo = DomService.GetDistance(one, downtwo);

    isEqual=onetodown == (downone.getBoundingClientRect().top - one.getBoundingClientRect().bottom);
    expect(isEqual).to.eql(true);
    isEqual=onetwo == (two.getBoundingClientRect().left - one.getBoundingClientRect().right);
    expect(isEqual).to.eql(true);

    expect(onedonwtwo < onetodown || onedonwtwo < onetwo || onedonwtwo > (onetodown + onetwo)).to.eql(false);

}

function TestGetHasClassId() {

   // id = "GetElementHasId" class="one two"

    var idelements = DomService.GetElementsHasId(simpledoc); 

    var idelement = DomService.FindElement(idelements, "id", "GetElementHasId"); 
    expect(idelement.id).to.eql("GetElementHasId");

    var classelements = DomService.GetElementsHasClass(simpledoc); 

    var classelement = DomService.FindElement(classelements, "class", "one two"); 
    expect(classelement.className).to.eql("one two");

}

function TestWhiteSpace() {

    var yes = simpledoc.getElementById("WhiteSpaceYes"); 
    var no = simpledoc.getElementById("WhiteSpaceNo"); 

    var testyes = DomService.IsWhiteSpace(yes.childNodes[0].nodeValue); 
    var testno = DomService.IsWhiteSpace(no.childNodes[0].nodeValue); 
    expect(testyes).to.eql(true);
    expect(testno).to.eql(false);

}

function TestPositionAlign() {
    var iframe=Kooboo.TestHelper.createIframe(true);
    var iframeDoc = iframe.contentDocument;
    iframeDoc.body.innerHTML=simpledoc.body.innerHTML;
    
    var ul = iframeDoc.getElementById("positionalign"); 
    var subs = DomService.GetChildElements(ul); 

    var another = iframeDoc.getElementById("notinposition"); 

    var yes = DomService.IsPositionAlign(subs); 
    subs.push(another); 

    var no = DomService.IsPositionAlign(subs); 

    expect(yes).to.eql(true);
    expect(no).to.eql(false);
    Kooboo.TestHelper.removeIframe(iframe);
 
}

function TestGetLinkPattern() {

    var values = []; 
    values.push("a/b/c/kk");
    values.push("a/b/c/kf");
    values.push("a/b/c/kg");
    values.push("a/b/c/kc?aaa=ss");
    var pattern = DomService.GetLinkPattern(values, "href"); 
    expect(pattern).to.eql("a/b/c/{href}");

    var withslash = []; 
    withslash.push("/a/b/sss"); 
    withslash.push("/a/b/222"); 

    pattern = DomService.GetLinkPattern(withslash, "href"); 
    expect(pattern).to.eql("/a/b/{href}")
    

}

function TestUrlPattern() {

    var values = [];
    values.push("/a/b/c/kk");
    values.push("/a/b/c/kf");
    values.push("/a/b/c/kg");
    values.push("/a/b/c/kc");
    var pattern = DomService.GetUrlPattern(values);
    expect(pattern.UrlPath).to.eql("/a/b/c/{key}");
    

    var datalist = _.toArray(pattern.Data); 
    expect(datalist[0].length).to.eql(4);
    expect(datalist[0][0]).to.eql("kk");

    var paravalues = [];
    paravalues.push("/a/b/c/kk?a=23");
    paravalues.push("/a/b/c/kk?a=25");
    paravalues.push("/a/b/c/kk?a=27"); 
    pattern = DomService.GetUrlPattern(paravalues);

    expect(pattern.FinalUrl()).to.eql("/a/b/c/kk?a={idx}");

    var datalist = _.toArray(pattern.Data);

    expect(datalist[0].length).to.eql(3);
    expect(datalist[0][0]).to.eql("23");
        
}

function TestUrlPatternWithUrlRewrite() {

    var paravalues = [];
    paravalues.push("news-2.aspx");
    paravalues.push("news-24.aspx");
    paravalues.push("news-25.aspx");
   var pattern = DomService.GetUrlPattern(paravalues);

   expect(pattern.FinalUrl()).to.eql("news-{idx}.aspx");

}

function TestFlatUrlPattern() {
    var values = [];
    values.push("news-2.aspx");
    values.push("news-24.aspx");
    values.push("news-25.aspx");

    var pattern = DomService.GetUrlPattern(values); 

    expect(pattern).not.to.eql(null);
    expect(pattern.Data["id"]).not.to.eql(null);

    _.remove(values, function (o) { return true; });

    values.push("news-2");
    values.push("news-24");
    values.push("news-25");

    var pattern = DomService.GetUrlFlatPattern(values);

    expect(pattern).not.to.eql(null);
    expect(pattern.Data["id"]).not.to.eql(null);

}


function TestUrlPattern_With_More_Para_for_One_Item() {
     
    var paravalues = [];
    paravalues.push("/a/b/c/kk?a=23&b=x");
    paravalues.push("/a/b/c/kk?a=25");
    paravalues.push("/a/b/c/kk?a=27");
    var pattern = DomService.GetUrlPattern(paravalues);
    expect(pattern.FinalUrl()).to.eql("/a/b/c/kk?a={idx}&b={key}");

    var datalist = _.toArray(pattern.Data);

    expect(datalist[0].length).to.eql(3);
    expect(datalist[0][0]).to.eql("23");
     
    var paravalues = [];
    paravalues.push("/a/b/c/kk?a=23&b=x");
    paravalues.push("/a/b/c/kk?a=25&b=c");
    paravalues.push("/a/b/c/kk?a=27&b=y");
    var pattern = DomService.GetUrlPattern(paravalues);
    expect(pattern.FinalUrl()).to.eql("/a/b/c/kk?a={idx}&b={key}");

    var datalist = _.toArray(pattern.Data);
    expect(datalist[0].length).to.eql(3);
    expect(datalist[0][0]).to.eql("23");
    expect(datalist[1][0]).to.eql("x");
    expect(datalist[1][1]).to.eql("c");
    expect(datalist[1][2]).to.eql("y");
}

function TestUrlPattern_Without_Segment() {

    var paravalues = [];
    paravalues.push("/bb/aaa.123.aspx");
    paravalues.push("/bb/aaa.124.aspx");
    paravalues.push("/bb/aaa.125.aspx");
    var pattern = DomService.GetUrlPattern(paravalues);
    expect(pattern.FinalUrl()).to.eql("/bb/aaa.{idx}.aspx");

    var datalist = _.toArray(pattern.Data); 
    expect(datalist[0].length).to.eql(3);
    expect(datalist[0][0]).to.eql("123");
}

function TestUrlPattern_With_UrlPath_AndSubSeg() {

    var paravalues = [];
    paravalues.push("/path/aaa_123");
    paravalues.push("/path/aaa_124");
    paravalues.push("/path/aaa_125");
    var pattern = DomService.GetUrlPattern(paravalues);

    expect(pattern.FinalUrl()).to.eql("/path/aaa_{idx}");

    var datalist = _.toArray(pattern.Data);
    expect(datalist[0].length).to.eql(3);
    expect(datalist[0][0]).to.eql("123");
}

function TestUrlPattern_non_pattern() {
    var paravalues = [];
    paravalues.push("/xyz/aaa_123");
    paravalues.push("/aa/aaa_124");
    paravalues.push("/ccc/aaa_125");
    var pattern = DomService.GetUrlPattern(paravalues);
    
    expect(pattern).to.eql(null);
     
    var paravalues = [];
    paravalues.push("/aa/aaa_123");
    paravalues.push("/aa/aaa_124");
    paravalues.push("/aa/aaa_125");
    pattern = DomService.GetUrlPattern(paravalues);
    
    expect(pattern).not.to.eql(null);
}

function TestPosition() {

    var one = simpledoc.getElementById("position1"); 
    var two = simpledoc.getElementById("position2"); 
    var three = simpledoc.getElementById("position3"); 

    var onetwo = DomService.CompareDomPosition(two, one); 
    expect(onetwo > 0).to.eql(true);

    var twothree = DomService.CompareDomPosition(two, three);

    expect(twothree).to.eql(0);

    var istrue = DomService.IsParentSubElement(two, three); 
    expect(istrue).to.eql(true);
}

function TestGetDocumentWidth() {
    var iframe=Kooboo.TestHelper.createIframe(true);
    var iframeDoc = iframe.contentDocument;
    iframeDoc.body.innerHTML=simpledoc.body.innerHTML;

    var width = DomService.GetDocumentWidth(iframeDoc.body);  
    var container = iframeDoc.getElementById("maincontainer"); 
    var oldcontainerwidth = $(container).width(); //width error

    $(container).width(oldcontainerwidth - 100); 
    var newcontainerwidth = $(container).width();

    expect(oldcontainerwidth > newcontainerwidth).to.eql(true);

    var newwidth = DomService.GetDocumentWidth(iframeDoc.body); 

    expect(width >= newwidth).to.eql(true);
    
    Kooboo.TestHelper.removeIframe(iframe);
}

function TestGetAttribute() {
    var el = simpledoc.getElementById("abcde");

    var attributes = DomService.GetExtraAttributes(el); 

    expect(attributes.length > 1).to.eql(true);
}

function TestGetTreeDistance() {
    var one = simpledoc.getElementById("aone"); 
    var two = simpledoc.getElementById("atwo"); 
    var pone = simpledoc.getElementById("paone"); 
    var plong = simpledoc.getElementById("pdistance"); 

    var disone = DomService.GetTreeDistance(one, two); 
    var distwo = DomService.GetTreeDistance(one, pone); 
    var distwop = DomService.GetTreeDistance(two, pone); 

    expect(disone).to.eql(1);
    expect(distwo).to.eql(10);
    expect(distwop).to.eql(9);
     
    var long = DomService.GetTreeDistance(one, plong); 

    var longtwo = DomService.GetTreeDistance(two, plong); 

    expect(long > longtwo).to.eql(true);

}

function TestGetSameChain() {

    var chain = []; 

    var one = [];
    one.push("one"); 
    one.push("two"); 
    one.push("x"); 
     
    var two = [];
    two.push("one");
    two.push("two");
    two.push("z"); 

    var three = [];
    three.push("one");
    three.push("two");
    three.push("t"); 

    chain.push(one); 
    chain.push(two); 
    chain.push(three); 

    var same = DomService.GetSameChain(chain); 

    expect(same.length).to.eql(2);
    expect(same[0]).to.eql("one");
    expect(same[1]).to.eql("two");

    _.remove(three, function (o) { return o == "two"; }); 

    same = DomService.GetSameChain(chain);

    expect(same.length).to.eql(1);
    expect(same[0]).to.eql("one");

}

function IsNVaulues(){
    var values=["1","1","1","1","2"];
    var isNValue=DomService.IsNVaulues(values);
    expect(isNValue).to.eql(false);

    values=["1","1","1","1","2","1","1","1","1","2","1","1","1","1","3"];
    var isNValue=DomService.IsNVaulues(values);
    expect(isNValue).to.eql(false);

    values=["1","1","1","1","2","1","1","1","1","2","1","1","1","2"];
    var isNValue=DomService.IsNVaulues(values);
    expect(isNValue).to.eql(false);

    values=["1","1","1","1","2","1","1","1","1","2"];
    var isNValue=DomService.IsNVaulues(values);
    expect(isNValue).to.eql(true);

    values=["1","1","1","1","2","1","1","1","1","2","1","1","1","1","2","1","1","1","1","2"];
    var isNValue=DomService.IsNVaulues(values);
    expect(isNValue).to.eql(true);
}
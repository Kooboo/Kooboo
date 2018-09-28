var shorthandPropertyManager = Kooboo.ShorthandPropertyManager;

function setPropertyKeyDic() {
    var jsonRule = {
        koobooId: "1-1"
    };
    var cssRule = {
        "font": "italic bold 12px/20px arial,sans-serif"
    };
    var property = "font-size";
    var shorthandProperty = shorthandPropertyManager.setPropertyKeyDic(jsonRule, cssRule, property);

    expect(shorthandProperty).to.be.eql("font");

    //2.cssRule变化，id一样,值不变化
    cssRule = {
        "font-size": "12px"
    }
    shorthandProperty = shorthandPropertyManager.setPropertyKeyDic(jsonRule, cssRule, property);

    expect(shorthandProperty).to.be.eql("font");

    //3.id 不一样， no shorthandProperty
    jsonRule = {
        koobooId: "1-2"
    };
    var cssRule = {
        "font-size": "12px"
    };
    shorthandProperty = shorthandPropertyManager.setPropertyKeyDic(jsonRule, cssRule, property);
    expect(shorthandProperty).to.be.eql("");

    //4.id 不一样， 多个属性 shorthandProperty
    jsonRule = {
        koobooId: "1-2-1"
    };
    var cssRule = {
        "font": "italic bold 12px/20px arial,sans-serif",
        "background": "url('aa.png')"
    };
    shorthandProperty = shorthandPropertyManager.setPropertyKeyDic(jsonRule, cssRule, "font-size");
    expect(shorthandProperty).to.be.eql("font");
    shorthandProperty = shorthandPropertyManager.setPropertyKeyDic(jsonRule, cssRule, "background-image");
    expect(shorthandProperty).to.be.eql("background");

}

function getShorthandPropertyValue() {
    var fontStyleObj = {
        "font-style": "italic",
        "font-variant": "normal",
        "font-weight": "bold",
        "font-stretch": "normal",
        "font-size": "12px",
        "line-height": "20px",
        "font-family": "arial, sans-serif"
    };
    var cssRule = {};

    KMock(cssRule).callFake("getPropertyValue", function(property) {
        switch (property) {
            case "font-style":
                return "italic";
            case "font-variant":
                return "normal";
            case "font-weight":
                return "bold";
            case "font-stretch":
                return "normal";
            case "font-size":
                return "12px";
            case "line-height":
                return "20px";
            case "font-family":
                return "arial, sans-serif";
        }


    });
    var shorthandProperty = "font";
    var shorthandPropertyValue = shorthandPropertyManager.getShorthandPropertyValue(cssRule, shorthandProperty);
    expect(shorthandPropertyValue).to.eql("italic normal bold normal 12px /20px arial, sans-serif")
}
var Color=Kooboo.Color;
function fromArray() {
    var rgbaArr = [255, 0, 0, 2];
    var color = Color.fromArray(rgbaArr);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(0);
    expect(color.b).to.eql(0);
    expect(color.a).to.eql(2);

    var rgbaArr = [255, 0, 0];
    var color = Color.fromArray(rgbaArr);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(0);
    expect(color.b).to.eql(0);
    expect(color.a).to.eql(1);
}

function fromHex() {
    var color = "#fff";
    color = Color.fromHex(color);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(255);
    expect(color.b).to.eql(255);
    expect(color.a).to.eql(1);

    color = "#ffffff";
    color = Color.fromHex(color);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(255);
    expect(color.b).to.eql(255);
    expect(color.a).to.eql(1);
}

function fromHex_error() {
    var color = "#ffff";
    color = Color.fromHex(color);
    expect(color).to.eql(null);

    var color = "ffff";
    color = Color.fromHex(color);
    expect(color).to.eql(null);
}

function fromRgb() {
    var rgba = "rgba    (255,0,0    ,1)";
    var color = Color.fromRgb(rgba);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(0);
    expect(color.b).to.eql(0);
    expect(color.a).to.eql(1);

    rgba = "rgba( 255, 0, 0, 1)";
    var color = Color.fromRgb(rgba);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(0);
    expect(color.b).to.eql(0);
    expect(color.a).to.eql(1);

    rgba = "rgba( 255, 0, 0, 0.5)";
    var color = Color.fromRgb(rgba);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(0);
    expect(color.b).to.eql(0);
    expect(color.a).to.eql(0.5);

    rgba = "rgba( 255, 0, 0, .5)";
    var color = Color.fromRgb(rgba);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(0);
    expect(color.b).to.eql(0);
    expect(color.a).to.eql(0.5);

    rgba = "rgba( 255, 0, 0, 60%)";
    var color = Color.fromRgb(rgba);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(0);
    expect(color.b).to.eql(0);
    expect(color.a).to.eql(0.6);

    rgba = "rgba( 100%, 0, 0, 60%)";
    var color = Color.fromRgb(rgba);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(0);
    expect(color.b).to.eql(0);
    expect(color.a).to.eql(0.6);

    rgba = "rgba( 100%, 0, 0, 2)";
    var color = Color.fromRgb(rgba);
    expect(color.r).to.eql(255);
    expect(color.g).to.eql(0);
    expect(color.b).to.eql(0);
    expect(color.a).to.eql(2);
}

function equals() {
    var str1 = "none";
    var str2 = "";
    var isEqual = Color.equals(str1, str2);
    expect(isEqual).to.eql(false);

    str1 = "rgb(0,0,0)";
    str2 = "rgb(0,0, 1)";

    isEqual = Color.equals(str1, str2);
    expect(isEqual).to.eql(false);

    str1 = "red";
    str2 = "red";

    isEqual = Color.equals(str1, str2);
    expect(isEqual).to.eql(true);

    str1 = "#fff";
    str2 = "#fff";

    isEqual = Color.equals(str1, str2);
    expect(isEqual).to.eql(true);

    str1 = "rgb(0,0,0)";
    str2 = "rgb(0,0, 0)";

    isEqual = Color.equals(str1, str2);
    expect(isEqual).to.eql(true);

    str1 = "white";
    str2 = "rgb(255,255, 255)";

    isEqual = Color.equals(str1, str2);
    expect(isEqual).to.eql(true);
}

function fromRgb_errorRgb() {
    var rgba = "rgbaxx( 255, 0, 0, 60%)";
    var color = Color.fromRgb(rgba);
    expect(color).to.eql(null);
}

function getRgbValueByPercent() {
    var value = "255";
    expect(Color.getRgbValueByPercent(value, false)).to.eql(255);

    var value = "100%";
    expect(Color.getRgbValueByPercent(value, false)).to.eql(255);

    var value = "60%";
    expect(Color.getRgbValueByPercent(value, true)).to.eql(0.6);
}

function searchString() {
    var color = "rgba(255,0,0,1)";
    var str = Color.searchString(color);
    expect(str).to.eql("rgba(255,0,0,1)");
    var color = "#fff";
    var str = Color.searchString(color);
    expect(str).to.eql("#fff");

    color = "#ffffff";

    var str = Color.searchString(color);
    expect(str).to.eql("#ffffff");

    //text-shadow
    color = "#CCC 1px 0 10px";
    var str = Color.searchString(color);
    expect(str).to.eql("#CCC");

    //5px 5px #558ABB
    color = "5px 5px #558ABB";
    var str = Color.searchString(color);
    expect(str).to.eql("#558ABB");

    //text-shadow: white 2px 5px;
    color = "white 2px 5px";
    var str = Color.searchString(color);
    expect(str).to.eql("white");

    color = "(initial)";
    var str = Color.searchString(color);
    expect(str).to.eql("initial");
}

function isRightHex() {
    var color = "#fff";
    expect(Color.isRightHex(color)).to.eql(true);
    var color = "#ffffff";
    expect(Color.isRightHex(color)).to.eql(true);

    var color = "#ff";
    expect(Color.isRightHex(color)).to.eql(false);

    var color = "#ffff";
    expect(Color.isRightHex(color)).to.eql(false);

    var color = "ffff";
    expect(Color.isRightHex(color)).to.eql(false);
}

function searchString_noColor() {
    var color = "2px 5px";
    var str = Color.searchString(color);
    expect(str).to.eql(null);

    var color = "5px";
    var str = Color.searchString(color);
    expect(str).to.eql(null);
}

function scan() {
    var Scanner = Kooboo.StringScanner;
    var rgb = "rgb(255,255,255)";

    var scanner = new Scanner(rgb, "rgb", "(", ")");
    var value = scanner.scan();
    expect(value).to.eql(value);
}

function scan_error() {
    var Scanner = Kooboo.StringScanner;
    var rgb = "rgb255,255,255)";
    var scanner = new Scanner(rgb, "rgb", "(", ")");
    var value = scanner.scan();
    expect(value).to.eql(null);
}
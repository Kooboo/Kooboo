function Color(){
    var Color = function(colorStr) {
        var self = this;
        var defaults = {
            r: 255,
            g: 255,
            b: 255,
            a: 1,
            colorStr: colorStr,
            init: function() {
                if (self.colorStr && self.colorStr.toLowerCase() != "initial") {
                    this.setColor(self.colorStr);
                } else {
                    this.setColor("transparent");
                }
            },
            setColor: function(color) {
                if (typeof color === "string") {
                    Color.fromString(color, self);
                } else if (_.isArray(color)) {
                    Color.fromArray(color, self);
                } else {
                    self._set(color.r, color.g, color.b, color.a);
                }
                return self;
            },
            _set: function(r, g, b, a) {
                self.r = r;
                self.g = g;
                self.b = b;
                self.a = a;
            },
            toRgb: function() {
                var valueArr = [self.r, self.g, self.b];
                return "rgb(" + valueArr.join(",") + ")";
            },
            toRgba: function() {
                var a = isNaN(self.a) ? 1 : self.a;
                var valueArr = [self.r, self.g, self.b, a];
                return "rgba(" + valueArr.join(",") + ")";
            },
            toHex: function() {
                var arr = _.map(["r", "g", "b"], function(x) {
                    var s = self[x].toString(16);
                    return s.length < 2 ? "0" + s : s;
                });
                return "#" + arr.join("");
            },
            toString: function() {
                if (this.a == 0) {
                    return "transparent";
                } else if (this.a < 1) {
                    return this.toRgba();
                }
                return this.toHex();
            }
        };
        $.extend(self, defaults);
        self.init();
        return self;
    };
    Color.named = {
        "black": [0, 0, 0],
        "navy": [0, 0, 128],
        "darkblue": [0, 0, 139],
        "mediumblue": [0, 0, 205],
        "blue": [0, 0, 255],
        "darkgreen": [0, 100, 0],
        "green": [0, 128, 0],
        "teal": [0, 128, 128],
        "darkcyan": [0, 139, 139],
        "deepskyblue": [0, 191, 255],
        "darkturquoise": [0, 206, 209],
        "mediumspringgreen": [0, 250, 154],
        "lime": [0, 255, 0],
        "springgreen": [0, 255, 127],
        "aqua": [0, 255, 255],
        "cyan": [0, 255, 255],
        "midnightblue": [25, 25, 112],
        "dodgerblue": [30, 144, 255],
        "lightseagreen": [32, 178, 170],
        "forestgreen": [34, 139, 34],
        "seagreen": [46, 139, 87],
        "darkslategray": [47, 79, 79],
        "limegreen": [50, 205, 50],
        "mediumseagreen": [60, 179, 113],
        "turquoise": [64, 224, 208],
        "royalblue": [65, 105, 225],
        "steelblue": [70, 130, 180],
        "darkslateblue": [72, 61, 139],
        "mediumturquoise": [72, 209, 204],
        "indigo": [75, 0, 130],
        "darkolivegreen": [85, 107, 47],
        "cadetblue": [95, 158, 160],
        "cornflowerblue": [100, 149, 237],
        "rebeccapurple": [102, 51, 153],
        "mediumaquamarine": [102, 205, 170],
        "dimgray": [105, 105, 105],
        "slateblue": [106, 90, 205],
        "olivedrab": [107, 142, 35],
        "slategray": [112, 128, 144],
        "lightslategray": [119, 136, 153],
        "mediumslateblue": [123, 104, 238],
        "lawngreen": [124, 252, 0],
        "chartreuse": [127, 255, 0],
        "aquamarine": [127, 255, 212],
        "maroon": [128, 0, 0],
        "purple": [128, 0, 128],
        "olive": [128, 128, 0],
        "gray": [128, 128, 128],
        "skyblue": [135, 206, 235],
        "lightskyblue": [135, 206, 250],
        "blueviolet": [138, 43, 226],
        "darkred": [139, 0, 0],
        "darkmagenta": [139, 0, 139],
        "saddlebrown": [139, 69, 19],
        "darkseagreen": [143, 188, 143],
        "lightgreen": [144, 238, 144],
        "mediumpurple": [147, 112, 219],
        "darkviolet": [148, 0, 211],
        "palegreen": [152, 251, 152],
        "darkorchid": [153, 50, 204],
        "yellowgreen": [154, 205, 50],
        "sienna": [160, 82, 45],
        "brown": [165, 42, 42],
        "darkgray": [169, 169, 169],
        "lightblue": [173, 216, 230],
        "greenyellow": [173, 255, 47],
        "paleturquoise": [175, 238, 238],
        "lightsteelblue": [176, 196, 222],
        "powderblue": [176, 224, 230],
        "firebrick": [178, 34, 34],
        "darkgoldenrod": [184, 134, 11],
        "mediumorchid": [186, 85, 211],
        "rosybrown": [188, 143, 143],
        "darkkhaki": [189, 183, 107],
        "silver": [192, 192, 192],
        "mediumvioletred": [199, 21, 133],
        "indianred": [205, 92, 92],
        "peru": [205, 133, 63],
        "chocolate": [210, 105, 30],
        "tan": [210, 180, 140],
        "lightgray": [211, 211, 211],
        "thistle": [216, 191, 216],
        "orchid": [218, 112, 214],
        "goldenrod": [218, 165, 32],
        "palevioletred": [219, 112, 147],
        "crimson": [220, 20, 60],
        "gainsboro": [220, 220, 220],
        "plum": [221, 160, 221],
        "burlywood": [222, 184, 135],
        "lightcyan": [224, 255, 255],
        "lavender": [230, 230, 250],
        "darksalmon": [233, 150, 122],
        "violet": [238, 130, 238],
        "palegoldenrod": [238, 232, 170],
        "lightcoral": [240, 128, 128],
        "khaki": [240, 230, 140],
        "aliceblue": [240, 248, 255],
        "honeydew": [240, 255, 240],
        "azure": [240, 255, 255],
        "sandybrown": [244, 164, 96],
        "wheat": [245, 222, 179],
        "beige": [245, 245, 220],
        "whitesmoke": [245, 245, 245],
        "mintcream": [245, 255, 250],
        "ghostwhite": [248, 248, 255],
        "salmon": [250, 128, 114],
        "antiquewhite": [250, 235, 215],
        "linen": [250, 240, 230],
        "lightgoldenrodyellow": [250, 250, 210],
        "oldlace": [253, 245, 230],
        "red": [255, 0, 0],
        "fuchsia": [255, 0, 255],
        "magenta": [255, 0, 255],
        "deeppink": [255, 20, 147],
        "orangered": [255, 69, 0],
        "tomato": [255, 99, 71],
        "hotpink": [255, 105, 180],
        "coral": [255, 127, 80],
        "darkorange": [255, 140, 0],
        "lightsalmon": [255, 160, 122],
        "orange": [255, 165, 0],
        "lightpink": [255, 182, 193],
        "pink": [255, 192, 203],
        "gold": [255, 215, 0],
        "peachpuff": [255, 218, 185],
        "navajowhite": [255, 222, 173],
        "moccasin": [255, 228, 181],
        "bisque": [255, 228, 196],
        "mistyrose": [255, 228, 225],
        "blanchedalmond": [255, 235, 205],
        "papayawhip": [255, 239, 213],
        "lavenderblush": [255, 240, 245],
        "seashell": [255, 245, 238],
        "cornsilk": [255, 248, 220],
        "lemonchiffon": [255, 250, 205],
        "floralwhite": [255, 250, 240],
        "snow": [255, 250, 250],
        "yellow": [255, 255, 0],
        "lightyellow": [255, 255, 224],
        "ivory": [255, 255, 240],
        "white": [255, 255, 255],
        "transparent": [0, 0, 0, 0]
    };
    Color.searchString = function(value) {
        if (!value) {
            return null;
        }
        //rgb/rgba will contain \s
        var color = Color.getRgbValue(value);
        if (color != null) {
            return color;
        };
        //multi text-->get color
        //hex:#fff and knowcolor:while will not contain \s
        var splitPropertyValue = value.split(/\s+/g),
            knownColors = Object.keys(Color.named);
        for (var i in splitPropertyValue) {
            var v = splitPropertyValue[i];
            if (v && knownColors.indexOf(v.toLowerCase()) > -1) {
                return v;
            }
            if (Color.isRightHex(v)) {
                return v;
            }
        }

        var match = value.match(/(initial)/);
        if (match)
            return match[0];
        return null;
    };
    Color.equals = function(str1, str2) {
        if (str1 == str2) {
            return true;
        }
        var colorValue1 = Color.searchString(str1),
            colorValue2 = Color.searchString(str2);
        if (colorValue1 == null && colorValue2 == null) {
            return false;
        }
        var color1 = new Color(colorValue1),
            color2 = new Color(colorValue2);
        return color1.toString() == color2.toString();
    };
    Color.fromString = function(str, colorObj) {
        var rgbaArr = Color.named[str] || Color.named[str.toLowerCase()];
        return rgbaArr && Color.fromArray(rgbaArr, colorObj) || Color.fromRgb(str, colorObj) || Color.fromHex(str, colorObj);
    };
    Color.fromArray = function(rgbaArr, colorObj) {
        var color = colorObj || new Color();
        color._set(Number(rgbaArr[0]), Number(rgbaArr[1]), Number(rgbaArr[2]), Number(rgbaArr[3]));
        if (isNaN(color.a)) {
            color.a = 1;
        }
        return color;
    };
    Color.isRightHex = function(color) {
        if (color.indexOf("#") == -1 || (color.length != 4 && color.length != 7)) return false;
        color = Number("0x" + color.substr(1));
        if (isNaN(color)) {
            return false;
        }
        return true;
    };
    Color.fromHex = function(color, colorObj) {
        if (!Color.isRightHex(color)) return null;
        var t = colorObj || new Color(),
            bits = (color.length == 4) ? 4 : 8,
            mask = (1 << bits) - 1;
        color = Number("0x" + color.substr(1));

        _.forEach(["b", "g", "r"], function(x) {
            var c = color & mask;
            color >>= bits;
            t[x] = bits == 4 ? 17 * c : c;
        });
        t.a = 1;
        return t;
    };
    //’rgb(’ followed by a comma-separated list of three numerical values (three integer values(0-255, 0-255, 0-255)) followed by ‘)‘
    //’rgb(’ followed by a comma-separated list of three numerical values (three percentage values(0%-100%, 0%-100%, 0%-100%)) followed by ‘)’.
    //’rgba(’ followed by a comma-separated list of three numerical values (three integer values), followed by an <alphavalue>, followed by ‘)’.
    //’rgba(’ followed by a comma-separated list of three numerical values (three percentage values), followed by an <alphavalue>, followed by ‘)’.
    Color.fromRgb = function(color, colorObj) {
        var valueStr = Color.getRgbValue(color);

        if (valueStr != null) {
            var leftBracketIndex = valueStr.indexOf('(');
            var rightBracketIndex = valueStr.indexOf(')');
            var rgbinnerValue = valueStr.substr(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
            var valueArr = rgbinnerValue.split(',');
            var rgbArr = [];
            for (var i = 0; i < valueArr.length; i++) {
                var value = valueArr[i].trim();
                var isAlphavalue = i == 3; //a
                value = Color.getRgbValueByPercent(value, isAlphavalue);
                rgbArr.push(value);
            }
            if (rgbArr.length < 3 || rgbArr.length > 4) return null;

            return Color.fromArray(rgbArr, colorObj);
        }
        return null;
    };
    Color.getRgbValue = function(color) {
        var scanner = new StringScanner(color, "rgb", "(", ")");
        var valueStr = scanner.scan();
        if (valueStr == null) {
            scanner = new StringScanner(color, "rgba", "(", ")");
            valueStr = scanner.scan();
        }
        return valueStr;
    };
    Color.getRgbValueByPercent = function(value, alphavalue) {
        if (value.indexOf && value.indexOf("%") == value.length - 1) {
            value = value.replace("%", "");

            if (!isNaN(value)) {
                //rgb
                if (alphavalue) {
                    value = Number(value) / 100.00;
                } else {
                    value = Number(value) / 100 * 255;
                }

            }
        }
        return value;
    };

    var StringScanner = function(content, token, keyBeforeValue, keyAfterValue) {
        this.content = content;
        this.index = 0;
        this.token = token;
        this.keyBeforeValue = keyBeforeValue;
        this.keyAfterValue = keyAfterValue;
        this.startIndex = -1;
        this.endIndex = -1;
        this.valueStartIndex = -1;
        this.valueEndIndex = -1;
        this.matchState = MatchState.none;
        this.scan = function() {
            while (!this.isEOF()) {
                var char = this.content[this.index];
                if (!this.token)
                    break;
                var isMatch = true;
                switch (this.matchState) {
                    case MatchState.none:
                        if (this.hasMatchToken(this.index)) {
                            this.startIndex = this.index;
                            this.moveAhead(this.token.length);
                            this.matchState = MatchState.start;
                        }
                        break;
                    case MatchState.start:
                        if (char == this.keyBeforeValue) {
                            this.valueStartIndex = this.index + 1;
                        } else if (char == this.keyAfterValue) {
                            this.endIndex = this.index;
                            this.valueEndIndex = this.index - 1;
                            this.matchState = MatchState.end;
                        } else if (this.valueStartIndex == -1) {
                            var value = char.trim();
                            if (value)
                                isMatch = false;
                        }
                        break;
                }
                if (!isMatch || this.matchState == MatchState.end)
                    break;
                this.index++;
            }
            return this.getValue();
        };
        this.getValue = function() {
            if (this.startIndex < 0) return null;
            if (this.endIndex > this.content.length - 1) return null;
            if (this.startIndex > this.endIndex) return null;
            var count = this.endIndex - this.startIndex + 1;
            return this.content.substr(this.startIndex, count);
        };
        this.hasMatchToken = function(startIndex) {
            var count = this.token.length;
            if (startIndex + count > this.content.length) {
                return false;
            }
            var str = this.content.substr(startIndex, count);
            return this.token.toLowerCase() == str.toLowerCase();
        };
        this.reset = function() {
            this.index = 0;
        };
        this.isEOF = function() {
            return this.index >= this.content.length;
        };
        this.moveAhead = function(count) {
            this.index = this.index + count - 1;
        }
    }
    var MatchState = {
        none: 0,
        start: 1,
        end: 2
    }
    Kooboo.StringScanner = StringScanner;

    return Color;
}
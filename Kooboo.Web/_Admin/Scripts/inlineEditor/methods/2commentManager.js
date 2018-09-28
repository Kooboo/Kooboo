function commentManager()
{  
    var ParseState = {
        beforeKooboo: 0,
        inKooboo: 1,
        beforeName: 2,
        InName : 3,
        AfterName: 4,
        BeforeValue: 5,
        InValue: 6
    } 

    //only comment that start  with <!-- #kooboo-- are parsed. 
    var ParseComment = function(comment) {
        var result = ParseCommentLine(comment.data); 
        if (result && !result["node"]) {
            result["node"] = comment;
        }
        return result;
    };
  
    var ParseCommentLine = function(data) {
        var name;
        var value;
        var quote = "";
        var state = ParseState.beforeKooboo;
        var result = {};
        for (var index = 0; index < data.length; index++) {
            var current = data[index];
            if (state == ParseState.beforeKooboo) {
                if (current == "#") {
                    state = ParseState.inKooboo;
                }
            } else if (state == ParseState.inKooboo) {
                if (IsData(current)) {
                    if (result["kooboo"]) {
                        result["kooboo"] = result["kooboo"] + current;
                    } else {
                        result["kooboo"] = current;
                    }
                } else {
                    if (result["kooboo"]) {
                        var kooboovalue = result["kooboo"];
                        if (kooboovalue.indexOf("kooboo") == -1) {
                            return null;
                        } else {
                            state = ParseState.beforeName;
                            name = "";
                            value = "";
                        }
                    }
                }
            } else if (state == ParseState.beforeName) {
                if (IsData(current)) {
                    index--;
                    name = "";
                    state = ParseState.InName;
                }
            } else if (state == ParseState.InName) {
                if (IsData(current)) {
                    name += current;
                } else {
                    state = ParseState.AfterName;
                    index--;
                    result[name] = "";
                }
            } else if (state == ParseState.AfterName) {
                if (current == '=') {
                    state = ParseState.BeforeValue;
                    quote = ""; // clear the quote. 
                } else {
                    if (IsData(current)) {
                        state = ParseState.InName;
                        name = "";
                        value = "";
                        index--;
                    }
                }
            } else if (state == ParseState.BeforeValue) {
                if (IsData(current)) {
                    if (current == "\"" || current == "\'") {
                        quote = current;
                        state = ParseState.InValue;
                        value = "";
                    } else {
                        quote = "";
                        value = "";
                        state = ParseState.InValue;
                        index--;
                    }
                }
            } else if (state == ParseState.InValue) {
                if (quote) {
                    if (current == quote) {
                        result[name] = value;
                        name = "";
                        value = "";
                        state = ParseState.beforeName;
                    } else {
                        value += current;
                    }
                } else {
                    if (IsData(current)) {
                        value += current;
                    } else {
                        if (value) {
                            result[name] = value;
                            value = "";
                        }
                        name = "";
                        state = ParseState.beforeName;
                    }
                }
            }
        }
        return result;
    };


     var IsData = function(onechar) {
        if (!onechar) {
            return false;
        }
        if (Kooboo.charHelper.isSpaceChar(onechar) || onechar == '-' || onechar == '=' || onechar == '>' || onechar == "!") {
            return false;
        }

        return true;
    };
 

    return {

        Parse: ParseComment,
        ParseCommentLine: ParseCommentLine
    }

}
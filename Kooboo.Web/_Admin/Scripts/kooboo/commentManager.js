(function() {
    var commentManager = (function() {
        var charHelper = Kooboo.charHelper;

        function commentManager() {}
        //only comment that start  with <!-- #kooboo-- are parsed. 
        commentManager.prototype.ParseComment = function(comment) {
            var result = this.ParseCommentLine(comment.data);

            if (result && !result["node"]) {
                result["node"] = comment;
            }
            return result;
        };
        commentManager.prototype.ParseCommentLine = function(data) {
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
                    if (this.IsData(current)) {
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
                    if (this.IsData(current)) {
                        index--;
                        name = "";
                        state = ParseState.InName;
                    }
                } else if (state == ParseState.InName) {
                    if (this.IsData(current)) {
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
                        if (this.IsData(current)) {
                            state = ParseState.InName;
                            name = "";
                            value = "";
                            index--;
                        }
                    }
                } else if (state == ParseState.BeforeValue) {
                    if (this.IsData(current)) {
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
                        if (this.IsData(current)) {
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
        commentManager.prototype.IsData = function(onechar) {
            if (charHelper.prototype.isSpaceChar(onechar) || onechar == '-' || onechar == '=' || onechar == '>' || onechar == "!") {
                return false;
            }
            if (!onechar) {
                return false;
            }
            return true;
        };
        return commentManager;
    }());
    Kooboo.CommentManager = commentManager;
    var ParseState;
    (function(ParseState) {
        ParseState[ParseState["beforeKooboo"] = 0] = "beforeKooboo";
        ParseState[ParseState["inKooboo"] = 1] = "inKooboo";
        ParseState[ParseState["beforeName"] = 2] = "beforeName";
        ParseState[ParseState["InName"] = 3] = "InName";
        ParseState[ParseState["AfterName"] = 4] = "AfterName";
        ParseState[ParseState["BeforeValue"] = 5] = "BeforeValue";
        ParseState[ParseState["InValue"] = 6] = "InValue";
    })(ParseState || (ParseState = {}));
})();
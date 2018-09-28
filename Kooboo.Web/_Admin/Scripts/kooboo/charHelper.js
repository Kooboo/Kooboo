(function() {
    var charHelper = (function() {
        function charHelper() {}
        //The White_Space characters are those that have the Unicode property "White_Space" in the Unicode PropList.txt data file. [UNICODE]
        //This should not be confused with the "White_Space" value (abbreviated "WS") of the "Bidi_Class" property in the Unicode.txt data file.
        //The control characters are those whose Unicode "General_Category" property has the value "Cc" in the Unicode UnicodeData.txt data file. [UNICODE]
        /// The space characters, for the purposes of this specification, are
        /// U+0020 SPACE, "tab" (U+0009), "LF" (U+000A), "FF" (U+000C), and "CR" (U+000D). 
        charHelper.prototype.isSpaceChar = function(char) {
            return (char == '\u0020' || char == '\u0009' || char == '\u000a' || char == '\u000c' || char == '\u000d');
        };
        /// The lowercase ASCII letters are the characters in the range lowercase ASCII letters. a-z 
        charHelper.prototype.isLowercaseAscii = function(onechar) {
            var codeunit = onechar.charCodeAt(0);
            //a-z, ascii 61-122. 
            return (codeunit >= 97 && codeunit <= 122);
        };
        /// The uppercase ASCII letters are the characters in the range uppercase ASCII letters. A-Z 
        charHelper.prototype.isUppercaseAscii = function(onechar) {
            var codeunit = onechar.charCodeAt(0);
            return (codeunit >= 65 && codeunit <= 90);
        };
        charHelper.prototype.isAscii = function(onechar) {
            return this.isUppercaseAscii(onechar) || this.isLowercaseAscii(onechar);
        };
        charHelper.prototype.isAsciiDigit = function(inputstring) {
            if (!inputstring) {
                return false;
            }
            for (var index = 0; index < inputstring.length; index++) {
                var onechar = inputstring[index];
                if (!this.isAsciiDigit(onechar)) {
                    if (onechar != '.' && onechar != ',') {
                        return false;
                    }
                }
            }
            return true;
        };
        /// The ASCII digits are the characters in the range ASCII digits.
        charHelper.prototype.isAsciiDigitChar = function(onechar) {
            var codeunit = onechar.charCodeAt(0);
            //0-9, acsii 48-57. 
            return (codeunit >= 48 && codeunit <= 57);
        };
        /// The alphanumeric ASCII characters are those that are either uppercase ASCII letters, lowercase ASCII letters, or ASCII digits.
        charHelper.prototype.isAlphanumeric = function(onechar) {
            return (this.isUppercaseAscii(onechar) || this.isLowercaseAscii(onechar) || this.isAsciiDigit(onechar));
        };
        /// <summary>
        /// The ASCII hex digits are the characters in the ranges ASCII digits, U+0041 LATIN CAPITAL LETTER A to U+0046 LATIN CAPITAL LETTER F, and U+0061 LATIN SMALL LETTER A to U+0066 LATIN SMALL LETTER F.
        /// </summary>
        /// <param name="chr"></param>
        charHelper.prototype.isAsciiHexDigit = function(onechar) {
            if (this.isAsciiDigit(onechar)) {
                return true;
            }
            var codeunit = onechar.charCodeAt(0);
            // if (chr >= 41 && chr <= 46)
            if (codeunit >= 65 && codeunit <= 70) {
                return true;
            }
            if (codeunit >= 97 && codeunit <= 102)
            //  if (chr >= 61 && chr <= 66)
            {
                return true;
            }
            return false;
        };
        /// The uppercase ASCII hex digits are the characters in the ranges ASCII digits and U+0041 LATIN CAPITAL LETTER A to U+0046 LATIN CAPITAL LETTER F only.
        charHelper.prototype.isUppercaseAsciiHexDigit = function(onechar) {
            if (this.isAsciiDigit(onechar)) {
                return true;
            }
            var codeunit = onechar.charCodeAt(0);
            // if (chr >= 41 && chr <= 46)
            if (codeunit >= 65 && codeunit <= 70) {
                return true;
            }
            return false;
        };
        /// The lowercase ASCII hex digits are the characters in the ranges ASCII digits and U+0061 LATIN SMALL LETTER A to U+0066 LATIN SMALL LETTER F only.
        charHelper.prototype.isLowercaseAsciiHexDigits = function(onechar) {
            if (this.isAsciiDigit(onechar)) {
                return true;
            }
            var codeunit = onechar.charCodeAt(0);
            //if (chr >= 61 && chr <= 66)
            if (codeunit >= 97 && codeunit <= 102) {
                return true;
            }
            return false;
        };
        return charHelper;
    }());
    Kooboo.charHelper = charHelper;
})();
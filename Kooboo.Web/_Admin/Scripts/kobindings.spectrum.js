(function() {

    $.fn.setColorGrid = function(color, setValue, origValue) {
        var element = this,
            initColor = color,
            origValueString = origValue,
            isChanged = false;

        var origColor = '';

        setValue.subscribe(function(newValue) {
            origValueString = newValue;
        })

        var container = $("<div>");
        $(container).addClass("sp-color-editor-placeholder").css({
            width: 19,
            height: 19,
            float: 'right',
            marginRight: 2,
            padding: 2,
            border: '1px solid #ccc',
            overflow: 'hidden',
            display: 'inline-block',
            background: '#eee',
            cursor: 'pointer'
        })
        var colorGrid = $("<div>");
        $(colorGrid).css({
            width: '100%',
            height: '100%',
            border: '1px solid #222',
            background: initColor !== "transparent" ? initColor : "url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAIAAADZF8uwAAAAGUlEQVQYV2M4gwH+YwCGIasIUwhT25BVBADtzYNYrHvv4gAAAABJRU5ErkJggg==)"
        })
        $(container).append(colorGrid);

        $(element).after(container);

        $(container).on('click', function(e) {
            $(container).remove();
            $(element).spectrum({
                color: initColor,
                preferredFormat: "hex",
                showInput: true,
                localStorageKey: "Color_Editor_ColorItem",
                showButtons: true,
                showAlpha: true,
                showPalette: true,
                allowEmpty: true,
                cancelText: Kooboo.text.common.cancel,
                chooseText: "OK",
                palette: [
                    ["#000", "#444", "#666", "#999", "#ccc", "#eee", "#f3f3f3", "#fff"],
                    ["#f00", "#f90", "#ff0", "#0f0", "#0ff", "#00f", "#90f", "#f0f"],
                    ["#f4cccc", "#fce5cd", "#fff2cc", "#d9ead3", "#d0e0e3", "#cfe2f3", "#d9d2e9", "#ead1dc"],
                    ["#ea9999", "#f9cb9c", "#ffe599", "#b6d7a8", "#a2c4c9", "#9fc5e8", "#b4a7d6", "#d5a6bd"],
                    ["#e06666", "#f6b26b", "#ffd966", "#93c47d", "#76a5af", "#6fa8dc", "#8e7cc3", "#c27ba0"],
                    ["#c00", "#e69138", "#f1c232", "#6aa84f", "#45818e", "#3d85c6", "#674ea7", "#a64d79"],
                    ["#900", "#b45f06", "#bf9000", "#38761d", "#134f5c", "#0b5394", "#351c75", "#741b47"],
                    ["#600", "#783f04", "#7f6000", "#274e13", "#0c343d", "#073763", "#20124d", "#4c1130"]
                ],
                move: function(tinycolor) {
                    var newColor = _convertColor(tinycolor);
                    ko.bindingHandlers.spectrum.update(element, setValue, newColor);
                },
                okClick: function() {
                    isChanged = true;
                    origValueString = finalValue;
                    origColor = finalColor;
                    setValue(finalValue);
                },
                show: function() {
                    var color = Kooboo.Color.searchString(origValueString);

                    if (color) {
                        $(element).spectrum("set", color);
                        origColor = color;
                    }
                },
                hide: function() {
                    if (!isChanged) {
                        $(element).spectrum("set", origColor);
                        setValue(origValueString);
                    } else {
                        isChanged = !isChanged;
                    }
                },
                cancelClick: function() {
                    $(element).spectrum("set", origColor);
                    setValue(origValueString);
                }
            });
            setTimeout(function() {
                $(element).spectrum("toggle");
            }, 100);
        })
    }

    function _convertColor(tinycolor) {
        if (!tinycolor) {
            return "initial";
        }
        var value = tinycolor.toHexString();
        if (tinycolor._a == 0) {
            value = "transparent";
        } else if (tinycolor._a < 1) {
            value = tinycolor.toRgbString();
        }
        return value;
    }

    var finalColor = null,
        finalValue = null;

    ko.bindingHandlers.spectrum = {
        init: function(element, valueAccessor) {
            // $(".sp-container").remove();
            var valueString = ko.unwrap(valueAccessor()),
                origValueString = "",
                origColor = "",
                isChanged = false;

            valueString = _.trim(valueString);
            origValueString = _.cloneDeep(valueString);

            var color = Kooboo.Color.searchString(valueString);

            origColor = color;

            $(element).setColorGrid(color || "transparent", valueAccessor(), origValueString);

            debugger;
        },
        update: function(element, valueAccessor, newColor) {

            if (typeof newColor !== "function") {
                var valueString = ko.unwrap(valueAccessor()),
                    color = Kooboo.Color.searchString(valueString);

                var origValueArr = valueString.split(/\s(?![^()]*\))/),
                    origIdx = origValueArr.indexOf(color);

                if (origIdx > -1) {
                    origValueArr[origIdx] = newColor;
                } else {
                    origValueArr.splice(0, 0, newColor);
                }
                var newValue = origValueArr.join(" ");

                finalValue = _.trim(newValue);
                finalColor = newColor;
                // valueAccessor()(finalValue);
                debugger;

            }
        }
    }

})();
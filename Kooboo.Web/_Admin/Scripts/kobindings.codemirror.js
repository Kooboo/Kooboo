(function() {
    ko.bindingHandlers.codeMirror = {
        init: function(element, valueAccessor) {
            var options = ko.unwrap(valueAccessor());
            element.editor = CodeMirror.fromTextArea(element, $.extend({
                lineNumbers: true,
                indentUnit: 4,
                tabSize: 4,
                lineSeparator: '\r\n'
            }, ko.toJS(options)));

            if (options.hasOwnProperty("height")) {
                element.editor.setSize("100%", options.height);
            }

            element.editor.on('change', function(cm) {
                options.value(cm.getValue());
            });
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                var wrapper = element.editor.getWrapperElement();
                wrapper.parentNode.removeChild(wrapper);
            });
        },
        update: function(element, valueAccessor) {
            var value = ko.toJS(valueAccessor()).value;

            if (value.indexOf('\r\n') == -1) {
                value = value.split('\n').join('\r\n');
            }

            if (element.editor) {
                var cur = element.editor.getCursor();
                element.editor.setValue(value);
                element.editor.setCursor(cur);
                element.editor.refresh();
            }
        }
    }
})();
(function() {
    var names = {};
    Kooboo.columnEditor.controlTypes.forEach(function(t) {
        names[t.value.toLowerCase()] = t.displayName;
    });
    Kooboo.columnEditor.controlTypeNames = names;
})()
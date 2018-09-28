(function() {
    var Position = Kooboo.pageEditor.util.Position;

    function scanAttrPositions(node, container, positions) {
        $('[k-placeholder]', node).each(function(ix, it) {
            var name = $(it).attr('k-placeholder');
            positions[name] = new Position({
                name: name,
                elem: it,
                cntr: container
            });
        });
    }

    Kooboo.pageEditor.util.PositionScanner = {
        scan: function(node, container) {
            var positions = {};
            scanAttrPositions(node, container, positions);
            return positions;
        }
    };
})();
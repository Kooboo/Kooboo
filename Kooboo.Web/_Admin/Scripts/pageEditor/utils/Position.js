(function() {

    var Widget = Kooboo.Tool;

    function Position(cfg) {
        this.type = 'attr';
        this.name = cfg.name;
        this.elem = cfg.elem;
        this.cntr = cfg.cntr;
        this.fragments = {
            hasInit: false
        };
        this.holders = {};
    }

    var proto = Position.prototype;
    var hasInit = false;

    proto.setContent = function (ctx) {
        var self = this;
        if (!self.fragments.hasInit) {
            !self.fragments.initHTML && (self.fragments.initHTML = $(self.elem).html());
            $(self.elem).empty();
            self.fragments.hasInit = true;
        }

        var nodeString = '',
            node = null;

        if (ctx.type.toLowerCase() == 'script') {
            node = $('<script engine="kscript">' + ctx.html + '<\/script>');
        } else {
            nodeString = '<' + ctx.type.toLowerCase() + ' ' +
                (ctx.engine ? 'engine="' + ctx.engine + '"' : '') + ' ' +
                "style='display:block'>\n" +
                ctx.html.split('\n').join('') +
                "\n</" + ctx.type.toLowerCase() + ">"
        }

        if (!node) {
            node = $.parseHTML(nodeString);
        }

        $(node).data('kb-comp', ctx);

        $(self.elem).append(node);
        if (node[0].nodeType == 1 &&
            $(node).is(":visible")) {

            var wgt = new Widget.Lighter({
                borderColor: '#666',
                borderStyle: 'dashed',
                zIndex: 1
            });

            wgt.appendTo(this.cntr);

            wgt.mask({ el: node[0] });
            this.holders[ctx.id] = wgt;
        }
        this.fragments[ctx.id] = node[0];

        this.resize();
    }

    proto.remove = function(id) {
        this.holders[id] && this.holders[id].destroy();
        $(this.fragments[id]).remove();
        delete this.holders[id];
        delete this.fragments[id];

        if (Object.keys(this.fragments).length == 2) {
            try {
                $(this.elem).html(this.fragments.initHTML);
            } catch (ex) {

            }
            this.fragments.hasInit = false;
        }

        $(window).trigger("resize");
    }

    proto.resize = function() {
        var self = this;
        var holders = this.holders;
        _.forEach(this.fragments, function(nodes, id) {
            holders[id] && holders[id].mask({ el: nodes });
        });
    };

    Kooboo.pageEditor.util.Position = Position;
})();
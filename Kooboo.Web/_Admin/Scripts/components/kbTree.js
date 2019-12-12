
(function() {
    Vue.component("kb-tree", {
        template: Kooboo.getTemplate(
            "/_Admin/Scripts/components/kbTree.html"
        ),
        props: {
            nodes: {type: Array}
        },
        data: function () {
            return {
                isdraw: false
            }},
        watch:{
        },
        mounted: function() {

            if( !this.isdraw || this.nodes) {
                this.initTree();
                this.isdraw = true
            }else{

            }

        },
        methods: {

            initTree: function() {
                var self = this;
                $('#kb-tree').jstree({
                    "core": {
                        "strings": { 'Loading ...': Kooboo.text.common.loading + ' ...' },
                    },
                    "themes": {
                        "responsive": true
                    },
                    "plugins": ["wholerow", "checkbox", "types"],
                    "types": {
                        "default": {
                            "icon": "fa fa-file icon-state-warning"
                        }
                    }
                }).on('changed.jstree', function(e, data) {
                    var i, j, r = [];
                    for (i = 0, j = data.selected.length; i < j; i++) {
                        data.instance.get_node(data.selected[i]).data.itemid && r.push(data.instance.get_node(data.selected[i]).data.itemid);
                    }
                    self.$emit('change', r);
                }).on('ready.jstree', function() {
                    $(this).find('.jstree-anchor').each(function() {
                        if ($(this).parent().data("title")) {
                            $(this).popover({
                                container: "body",
                                placement: "top",
                                trigger: "hover",
                                title: $(this).parent().data("title"),
                                content: $(this).parent().data("content")
                            })
                        }
                    })
                }).on('after_open.jstree', function() {
                    $(this).find('.jstree-anchor').each(function() {
                        if ($(this).parent().data("title")) {
                            $(this).popover({
                                container: "body",
                                placement: "top",
                                trigger: "hover",
                                title: $(this).parent().data("title"),
                                content: $(this).parent().data("content")
                            })
                        }
                    })
                });
            }
        }
    });
})();

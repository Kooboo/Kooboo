(function() {
    Kooboo.loadJS([
        '/_Admin/Scripts/vue/components/kbTable/row/index.js'
    ]);

    Kooboo.vue.component.kbTable = Vue.component('kb-table', {
        props: {
            data: Object,
            deleteTrigger: Boolean
        },
        data: function() {
            return {
                selectedDocs: []
            }
        },
        watch: {
            selectedDocs: function(ids) {
                this.$emit('select', ids);
            },
            deleteTrigger: function(trigged) {
                var self = this;
                if (trigged) {
                    if (this.data.onDelete) {
                        this.data.onDelete(function() {
                            self.selectedDocs = [];
                            self.$emit('delete-trigged')
                        })
                    } else {

                    }
                }
            },
            'data.docs': function() {
                this.selectedDocs = [];
            }
        },
        methods: {
            onToggleSelectDoc: function(id) {
                var idx = this.selectedDocs.indexOf(id);
                if (idx == -1) {
                    this.selectedDocs.push(id);
                } else {
                    this.selectedDocs.splice(idx, 1);
                }
            }
        },
        computed: {
            allSelected: {
                get: function() {
                    if (this.data.docs && this.data.docs.length) {
                        return this.selectedDocs.length == this.data.docs.length;
                    } else {
                        return false;
                    }
                },
                set: function(val) {
                    var self = this;
                    if (this.data.docs && this.data.docs.length) {
                        if (val) {
                            this.data.docs.forEach(function(doc) {
                                if (self.selectedDocs.indexOf(doc.id) == -1) {
                                    self.selectedDocs.push(doc.id);
                                }
                            })
                        } else {
                            self.selectedDocs = [];
                        }
                    } else {
                        self.selectedDocs = [];
                    }
                }
            }
        },
        components: {
            'kb-table': Kooboo.vue.component.kbTableRow
        },
        template: Kooboo.getTemplate('/_Admin/Scripts/vue/components/kbTable/index.html')
    })
})()
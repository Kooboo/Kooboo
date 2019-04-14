(function() {
    Kooboo.loadJS([
        '/_Admin/Scripts/vue/components/kbTable/row/cell/array.js',
        '/_Admin/Scripts/vue/components/kbTable/row/cell/badge.js',
        '/_Admin/Scripts/vue/components/kbTable/row/cell/button.js',
        '/_Admin/Scripts/vue/components/kbTable/row/cell/label.js',
        '/_Admin/Scripts/vue/components/kbTable/row/cell/link.js',
        '/_Admin/Scripts/vue/components/kbTable/row/cell/icon.js',
        '/_Admin/Scripts/vue/components/kbTable/row/cell/icon-text.js',
        '/_Admin/Scripts/vue/components/kbTable/row/cell/summary.js',
        '/_Admin/Scripts/vue/components/kbTable/row/cell/text.js'
    ]);

    var component = Kooboo.vue.component;

    Kooboo.vue.component.kbTableRow = Vue.component('kb-table-row', {
        props: ['doc', 'cols', 'acts', 'selectable', 'selectedDocs'],
        methods: {
            getCellData: function(data) {
                return this.doc[data.fieldName]
            },
            onToggleSelectDoc: function() {
                this.$emit('toggleSelectDoc', this.doc.id);
            },
            onPublishEvent: function(data) {
                Kooboo.EventBus.publish(data.url, this.doc);
            },
            onCellClick: function(cb) {
                cb && cb(this.doc);
            }
        },
        components: {
            'kb-table-cell-array': component.kbTableCellArray,
            'kb-table-cell-text': component.kbTableCellText,
            'kb-table-cell-badge': component.kbTableCellBadge,
            'kb-table-cell-label': component.kbTableCellLabel,
            'kb-table-cell-link': component.kbTableCellLink,
            'kb-table-cell-icon': component.kbTableCellIcon,
            'kb-table-cell-iconText': component.kbTableCellIconText,
            'kb-table-cell-button': component.kbTableCellButton,
            'kb-table-cell-summary': component.kbTableCellSummary,
        },
        template: Kooboo.getTemplate('/_Admin/Scripts/vue/components/kbTable/row/index.html')
    })
})()
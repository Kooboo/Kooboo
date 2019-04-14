(function() {
    Kooboo.vue.component.kbTableCellText = Vue.component('kb-table-cell-text', {
        props: ['data', 'col'],
        render: function(h) {
            return h('td', {
                domProps: {
                    innerHTML: this.data
                }
            })
        }
    })
})()
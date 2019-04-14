(function() {
    Kooboo.vue.component.kbTableCellIcon = Vue.component('kb-table-cell-icon', {
        props: ['col', 'data'],
        render: function(createElement) {
            return createElement('td', {
                attrs: { style: 'width: 20px' }
            }, this.data ? [
                createElement('i', {
                    class: 'fa ' + (this.data.class || ''),
                    attrs: {
                        title: this.data.title || '',
                    }
                })
            ] : null)
        }
    })
})()
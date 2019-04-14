(function() {
    Kooboo.vue.component.kbTableCellIconText = Vue.component('kb-table-cell-icon-text', {
        props: ['col', 'data'],
        render: function(createElement) {
            if (this.data) {
                return createElement('td', {
                    attrs: {
                        title: this.data.title || ''
                    }
                }, [
                    createElement('i', {
                        class: 'fa ' + (this.data.icon || ''),
                    }),
                    createElement('span', {
                        domProps: {
                            innerHTML: this.data.text
                        }
                    })
                ])
            } else {
                return createElement('td');
            }
        }
    })
})()
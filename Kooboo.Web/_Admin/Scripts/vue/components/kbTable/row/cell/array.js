(function() {
    Kooboo.vue.component.kbTableCellArray = Vue.component('kb-table-cell-array', {
        props: ['col', 'data'],
        render: function(createElement) {
            var self = this;
            if (this.data && this.data.length) {
                return createElement('td', this.data.map(function(item) {
                    return createElement('a', {
                        class: item.class,
                        attrs: {
                            href: '#',
                            style: item.style
                        },
                        domProps: {
                            innerHTML: item.text
                        },
                        on: {
                            click: function(e) {
                                e.stopPropagation();
                                e.preventDefault();
                                item.onClick(item);
                            }
                        }
                    })
                }))
            } else {
                return createElement('td');
            }
        }
    })
})()
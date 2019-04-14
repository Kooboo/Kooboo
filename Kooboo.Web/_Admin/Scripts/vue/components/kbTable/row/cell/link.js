(function() {
    Kooboo.vue.component.kbTableCellLink = Vue.component('kb-table-cell-link', {
        props: ['data', 'col'],
        render: function(createElement) {
            var self = this;
            if (this.data) {
                return createElement('td', [
                    createElement('a', {
                        attrs: {
                            title: this.data.title || '',
                            href: this.data.url || '#',
                            target: this.data.inNewWindow ? '_blank' : '_self'
                        },
                        domProps: {
                            innerHTML: this.data.text
                        },
                        on: {
                            click: function(e) {
                                e.stopPropagation();
                                if (self.data.onClick) {
                                    e.preventDefault();
                                    self.$emit('onClick', self.data.onClick);
                                }
                            }
                        }
                    })
                ])
            } else {
                return createElement('td');
            }
        }
    })
})()
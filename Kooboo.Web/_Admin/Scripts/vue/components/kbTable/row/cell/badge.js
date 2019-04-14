(function() {
    Kooboo.vue.component.kbTableCellBadge = Vue.component('kb-table-cell-badge', {
        props: ['col', 'data'],
        render: function(createElement) {
            var self = this;
            if (this.data.onClick || this.data.url) {
                return createElement('td', [
                    createElement('a', {
                        class: 'badge ' + this.data.class,
                        attrs: {
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
                return createElement('td', [
                    createElement('span', {
                        class: 'badge ' + this.data.class,
                        domProps: {
                            innerHTML: this.data.text
                        }
                    })
                ]);
            }
        }
    })
})()
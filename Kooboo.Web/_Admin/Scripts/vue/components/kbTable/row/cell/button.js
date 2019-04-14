(function() {
    Kooboo.vue.component.kbTableCellButton = Vue.component('kb-table-cell-button', {
        props: ['col', 'data'],
        render: function(createElement) {
            var self = this;
            if (this.data.onClick || this.data.url) {
                return createElement('td', [
                    createElement('a', {
                        class: 'btn ' + this.data.class,
                        attrs: {
                            href: this.data.url || '#',
                            target: this.data.inNewWindow ? '_blank' : '_self'
                        },
                        //this will cause element i don't render the second time.
                        // domProps: {
                        //     innerHTML: this.data.text
                        // },
                        on: {
                            click: function(e) {
                                e.stopPropagation();
                                if (self.data.onClick) {
                                    e.preventDefault();
                                    self.$emit('onClick', self.data.onClick);
                                }
                            }
                        }
                    }, this.data.icon ? [
                        createElement('i', {
                            class: 'fa ' + this.data.icon
                        })
                    ] : null)
                ])
            }
        }
    })
})()
(function() {
    Kooboo.vue.component.kbTableCellSummary = Vue.component('kb-table-cell-summary', {
        props: ['data', 'col'],
        render: function(createElement) {
            var self = this;
            if (this.data) {
                return createElement('td', {
                    class: 'summary'
                }, [
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
                    }, [
                        createElement('p', {
                            class: 'title',
                            domProps: {
                                innerHTML: this.data.title
                            }
                        }),
                        createElement('p', {
                            domProps: {
                                innerHTML: this.data.description
                            }
                        })
                    ])
                ])
            } else {
                return createElement('td');
            }
        }
    })
})()
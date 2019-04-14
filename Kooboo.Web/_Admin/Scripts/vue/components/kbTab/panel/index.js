(function() {
    Kooboo.vue.component.kbTabPanel = Vue.component('kb-tab-pane', {
        props: {
            show: Boolean
        },
        data: function() {
            return {
                inClass: false
            }
        },
        watch: {
            show: function(show) {
                if (show) {
                    var self = this;
                    setTimeout(function() {
                        self.inClass = show
                    }, 50)
                } else {
                    this.inClass = show;
                }
            }
        },
        template: Kooboo.getTemplate('/_Admin/Scripts/vue/components/kbTab/panel/index.html')
    })
})()
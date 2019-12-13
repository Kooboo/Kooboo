(function() {
  Vue.component("kb-pager", {
    template: Kooboo.getTemplate("/_Admin/Scripts/components/kbPager.html"),
    props: {
      pageNr: Number,
      totalPages: Number
    },
    computed: {
      pages: function() {
        var _pages = [];
        if (this.pageNr && this.totalPages) {
          if (this.totalPages > 5) {
            if (this.pageNr > 3 && this.pageNr + 2 < this.totalPages) {
              _pages.push({
                count: "«",
                target: this.pageNr - 3,
                active: false
              });

              for (var i = 0; i < 5; i++) {
                _pages.push({
                  count: this.pageNr - 2 + i,
                  target: this.pageNr - 2 + i,
                  active: i == 2
                });
              }

              _pages.push({
                count: "»",
                target: this.pageNr + 3,
                active: false
              });
            } else {
              if (this.pageNr <= 3) {
                for (var i = 1; i <= 5; i++) {
                  _pages.push({
                    count: i,
                    target: i,
                    active: i == this.pageNr
                  });
                }

                _pages.push({
                  count: "»",
                  target: 6,
                  active: false
                });
              }

              if (this.pageNr + 2 >= this.totalPages) {
                for (var i = this.totalPages; i > this.totalPages - 5; i--) {
                  _pages.push({
                    count: i,
                    target: i,
                    active: i == this.pageNr
                  });
                }

                _pages.push({
                  count: "«",
                  target: this.totalPages - 5,
                  active: false
                });

                _pages.reverse();
              }
            }
          } else if (this.totalPages <= 5) {
            for (var i = 1; i <= this.totalPages; i++) {
              _pages.push({
                count: i,
                target: i,
                active: i == this.pageNr
              });
            }
          }
        }
        return _pages;
      }
    },
    methods: {
      changePage: function(page) {
        this.$emit('change', page.target);
        Kooboo.EventBus.publish("kb/pager/change", page.target);
      }
    }
  });
})();

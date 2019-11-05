(function() {
  new Vue({
    el: "#app",
    data: function() {
      return {
        text: {
          transferTask: Kooboo.text.common.TransferTask,
          sites: Kooboo.text.component.breadCrumb.sites,
          dashboard: Kooboo.text.component.breadCrumb.dashboard
        },
        tableData: [],
        tableDataSelectedRows: []
      };
    },
    created: function() {
      this.breads = [
        {
          name: this.text.sites
        },
        {
          name: this.text.dashboard
        },
        {
          name: this.text.transferTask
        }
      ];
      this.getTableData();
    },

    methods: {
      selectChangeHandle: function(event){
        console.log(event);
      },
      getTableData: function() {
        var vm = this;
        Kooboo.TransferTask.getList().then(function(res) {
          if (res.success) {
            res.model = [
              {
                id: 1,
                fullStartUrl: "url 1",
                lastModified: "October 13, 1975 11:13:00"
              },
              {
                id: 2,
                fullStartUrl: "url 2",
                lastModified: "October 13, 1975 11:13:00"
              },
              {
                id: 3,
                fullStartUrl: "url 3",
                lastModified: "October 13, 1975 11:13:00"
              }
            ];
            vm.tableData = res.model.map(function(item) {
              var date = new Date(item.lastModified);
              return {
                id: item.id,
                url: item.fullStartUrl,
                date: date.toDefaultLangString()
              };
            });
          }
        });
      }
    }
  });
})();

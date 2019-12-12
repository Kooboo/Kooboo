(function() {
  var template = Kooboo.getTemplate(
      "/_Admin/Scripts/viewEditor/components/link.html"
    ),
    PageStore = Kooboo.viewEditor.store.PageStore,
    DataContext = Kooboo.viewEditor.DataContext;
  bindingType = "link";

  Vue.component("kb-view-link", {
    template: template,
    data: function() {
      return {
        isShow: false,
        elem: null,
        dynaParams: [],
        dataSourceId: [],
        mode: "normal",
        link: "",
        pages: [],
        views: [],
        extLink: "",
        isExternalLink: false,
        parameters: [],
        href: ""
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.EventBus.subscribe("binding/edit", function(data) {
        if (data.bindingType == bindingType) {
          self.elem = data.elem;
          self.href = data.href;

          var dataSource = DataContext.get(data.elem).getDataSource();
          if (dataSource && dataSource.length && paginationDS(dataSource)) {
            self.mode = "pagination";
            var ds = dataSource[0];
            self.dataSourceId = ds.dataId;
            self.dynaParams.push({
              name: "pageNumber",
              value: ds.name
            });
          } else {
            self.mode = "normal";
          }

          !self.pages.length && (self.pages = PageStore.getAll().pages);
          !self.views.length && (self.views = PageStore.getAll().views);
          setDefaultValue();
          self.isShow = true;
        }

        function paginationDS(ds) {
          var find = _.find(ds, function(d) {
            return d.isPagedResult && d.name.indexOf("Pages_Item") > -1;
          });

          return !!find;
        }
      });
      function setDefaultValue() {
        if (!self.href) {
          self.link = self.externalLink;
        } else {
          var _find = _.findLast(_.concat(self.pages, self.views), function(
            link
          ) {
            return link.url == self.href;
          });

          if (!_find) {
            if (self.href.indexOf("?") > -1) {
              var temp = self.href.split("?");
              var possibleLink = temp[0],
                possibleParams = temp[1];

              var __find = _.findLast(
                _.concat(self.pages, self.views),
                function(link) {
                  return link.url == possibleLink;
                }
              );

              if (__find) {
                var params = __find.parameters.map(function(p) {
                    return { name: p, value: "" };
                  }),
                  exist = possibleParams.split("&").map(function(pa) {
                    var sp = pa.split("=");
                    return { name: sp[0], value: sp[1] };
                  }),
                  list = [];

                params.forEach(function(p) {
                  var find = _.find(exist, function(e) {
                    return e.name == p.name;
                  });

                  list.push(find || p);
                });

                self.setLink({
                  url: possibleLink,
                  parameters: list
                });
              }
            } else if (self.href.indexOf("Pager(") > -1) {
              if (!self.dynaParams.length) {
                var tester = self.href.match(/Pager\((\S*)\)/);
                if (tester) {
                  var paraStr = tester[1];

                  self.dynaParams.push({
                    name: "pageNumber",
                    value: paraStr
                  });
                }
              }
            } else {
              self.link = "__external_link";
              self.extLink = self.href;
            }
          } else {
            self.setLink(_find);
          }
        }
      }
    },
    methods: {
      save: function() {
        var self = this;
        if (self.link == "__external_link" && !self.extLink) {
          return alert(Kooboo.text.validation.required);
        }

        self.$emit("on-save", {
          elem: self.elem,
          bindingType: bindingType,
          href: self.getFinalHref()
        });

        if (self.mode == "pagination") {
          Kooboo.EventBus.publish("kb/view/editor/data/pager", {
            elem: self.elem,
            pager: self.getRawData(),
            dataSourceId: self.dataSourceId
          });
        }
        self.isShow = false;
      },
      getFinalHref: function() {
        var self = this;
        if (self.mode == "normal") {
          if (self.link == "__external_link") {
            return self.extLink;
          } else {
            if (self.parameters.length) {
              var query = [];
              self.parameters.forEach(function(para) {
                if (para.value) {
                  query.push(para.name + "=" + para.value);
                }
              });
              return self.link + (query.length ? "?" + query.join("&") : "");
            } else {
              return self.link;
            }
          }
        } else {
          var paras = {};
          self.dynaParams.forEach(function(dp) {
            paras[dp.name] = dp.value;
          });
          return "Pager(" + paras.pageNumber + ")";
        }
      },
      getRawData: function() {
        var temp = this.getFinalHref();
        return temp.match(/Pager\((\S*)\)/)[1];
      },
      setParamsters: function(parameters) {
        var _list = [];
        _.forEach(parameters, function(para) {
          if (typeof para == "string") {
            para = { name: para, value: "" };
          }
          _list.push(para);
        });
        this.parameters = _list;
      },
      setLink: function(link) {
        this.link = link.url;
        this.setParamsters(link.parameters || []);
      }
    },
    watch: {
      isShow: function(value) {
        if (value) return;
        this.elem = null;
        this.dynaParams = [];
        this.dataSourceId = [];
      },
      link: function(link) {
        var self = this;
        var find = _.find(_.concat(self.pages, self.views), function(l) {
          return l.url == link;
        });

        if (find) self.setParamsters(find.parameters);
      }
    }
  });
})();

(function() {
  Vue.component("kb-control-mediafile", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/controlType/MediaFile.html"
    ),
    props: {
      field: Object
    },
    inject: ["kbFormItem"],
    data: function() {
      return {
        pics: [],
        mediaDialogData: null
      };
    },
    mounted: function() {
      var self = this;
      var fieldValue = this.kbFormItem.kbForm.model[self.kbFormItem.prop];
      if (fieldValue) {
        if (self.field.isMultipleValue) {
          self.pics = fieldValue.map(function(p) {
            return {
              thumbnail: p + "?SiteId=" + Kooboo.getQueryString("SiteId"),
              url: p
            };
          });
        } else {
          self.pics = [
            {
              thumbnail:
                fieldValue + "?SiteId=" + Kooboo.getQueryString("SiteId"),
              url: fieldValue
            }
          ];
        }
      } else {
        self.pics = [];
      }
    },
    methods: {
      selectFile: function() {
        var self = this;
        Kooboo.Media.getList().then(function(res) {
          if (res.success) {
            res.model["show"] = true;
            res.model["context"] = self;
            res.model["onAdd"] = function(selected) {
              if (self.field.isMultipleValue) {
                _.forEach(selected, function(sel) {
                  if (!_.some(self.pics, { url: sel.url })) {
                    self.pics.push({
                      url: sel.url,
                      thumbnail: sel.thumbnail
                    });
                  }
                });
              } else {
                self.pics = [
                  {
                    url: selected.url,
                    thumbnail: selected.thumbnail
                  }
                ];
              }
            };
            self.mediaDialogData = res.model;
          }
        });
      },
      removePic: function(pic) {
        var self = this;
        self.pics = _.without(self.pics, pic);
      }
    },
    watch: {
      pics: function(value) {
        var self = this;
        if (self.field.isMultipleValue) {
          self.kbFormItem.kbForm.model[self.kbFormItem.prop] = value.map(
            function(v) {
              return v.url;
            }
          );
        } else {
          self.kbFormItem.kbForm.model[self.kbFormItem.prop] = value.length
            ? value[0].url
            : null;
        }
      }
    }
  });
})();

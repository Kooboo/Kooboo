$(function() {
  var cropper, self;
  var id = Kooboo.getQueryString("Id");
  new Vue({
    el: "#main",
    data: function() {
      self = this;
      return {
        filename: "",
        link: "",
        linkText: "",
        altText: "",
        _altText: "",
        src: "",
        _src: "",
        imageEditing: false,
        aspectRatioArray: [
          {
            text: Kooboo.text.site.images.free,
            ratio: 0
          },
          {
            text: "16 : 9",
            ratio: 16 / 9
          },
          {
            text: "4 : 3",
            ratio: 4 / 3
          },
          {
            text: "1 : 1",
            ratio: 1 / 1
          },
          {
            text: "2 : 3",
            ratio: 2 / 3
          }
        ],
        aspectRatio: 0,
        width: 0,
        height: 0,
        rotate: 0,
        isImageChange: false
      };
    },
    mounted: function() {
      if (!id) {
        return;
      }
      Kooboo.Media.Get({
        Id: id
      }).then(function(res) {
        if (res.success) {
          self.filename = res.model.name;
          self.linkText = res.model.url;
          self.link = res.model.fullUrl;
          self.altText = res.model.alt || "";
          self._altText = self.altText;
          self.src = res.model.siteUrl;
          self._src = self.src;
        }
      });
    },
    methods: {
      toggleMode: function() {
        if (!self.imageEditing) {
          self.imageEditing = true;
          self.$nextTick(function() {
            var image = document.getElementById("editable-img");
            cropper = new Cropper(image, {
              aspectRatio: self.aspectRatio,
              viewMode: 1,
              preview: ".thumbnail",
              crop: function(e) {
                self.width = Math.round(e.detail.width);
                self.height = Math.round(e.detail.height);
                self.rotate = Math.round(e.detail.rotate);
              }
            });
          });
        } else {
          if (confirm(Kooboo.text.confirm.exit)) {
            self.imageEditing = false;
            cropper.destroy();
          }
        }
      },
      actionClick: function(action) {
        switch (action.name) {
          case "zoom":
            cropper.zoom(action.value);
            break;
          case "move":
            cropper.move(action.value[0], action.value[1]);
            break;
          case "rotate":
            cropper.rotate(action.value);
            break;
          case "scaleX":
            cropper.scaleX(-cropper.getData().scaleX);
            break;
          case "scaleY":
            cropper.scaleY(-cropper.getData().scaleY);
            break;
        }
      },
      changeRatio: function(m) {
        if (m.ratio !== self.aspectRatio) {
          self.aspectRatio = m.ratio;
        }
      },
      saveEditing: function() {
        var canvas = cropper.getCroppedCanvas(),
          base64Image = canvas.toDataURL();

        self.src = base64Image;
        cropper.destroy();
        $(".thumbnail").removeAttr("style");
        self.imageEditing = false;
        self.isImageChange = true;
      },
      resetCropper: function() {
        var data = cropper.getData();
        data.width = Number(self.width);
        data.height = Number(self.height);
        data.rotate = Number(self.rotate);
        cropper.setData(data);
      },
      attrAction: function(action) {
        switch (action.name) {
          case "width":
            action.op == "-"
              ? (self.width = self.width - 1)
              : (self.width = self.width + 1);
            break;
          case "height":
            action.op == "-"
              ? (self.height = self.height - 1)
              : (self.height = self.height + 1);
            break;
          case "rotate":
            action.op == "-"
              ? (self.rotate = self.rotate - 1)
              : (self.rotate = self.rotate + 1);
            break;
        }
        self.resetCropper();
      },
      submitEdit: function() {
        Kooboo.Media.imageUpdate(self.getUpdateData()).then(function(res) {
          if (res.success) {
            self.goBack();
          }
        });
      },
      getUpdateData: function() {
        var data = {
          id: id,
          alt: self.altText
        };

        if (self.isImageChange) {
          data["base64"] = self.src;
        }

        return data;
      },
      userCancel: function() {
        if (!self.isChanged) {
          self.goBack();
        } else {
          if (confirm(Kooboo.text.confirm.beforeReturn)) {
            self.goBack();
          }
        }
      },
      goBack: function() {
        location.href =
          Kooboo.Route.Get(Kooboo.Route.Image.ListPage) + location.hash;
      }
    },
    computed: {
      isChanged: function() {
        return self.altText !== self._altText || self.src !== self._src;
      }
    },
    watch: {
      aspectRatio: function(ratio) {
        cropper.setAspectRatio(ratio);
      }
    },
    beforeDestory: function() {
      self = null;
    }
  });
});

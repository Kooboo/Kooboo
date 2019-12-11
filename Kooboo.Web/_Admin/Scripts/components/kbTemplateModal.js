(function() {
  Kooboo.loadJS(["/_Admin/Scripts/components/kbForm.js"]);

  Vue.component("kb-template-modal", {
    template: Kooboo.getTemplate(
      "/_Admin/Scripts/components/kbTemplateModal.html"
    ),
    props: {
      value: Boolean,
      tempData: Object
    },
    data: function() {
      var self = this;
      return {
        isShow: false,
        data: null,
        selected: false,
        domains: [],
        model: {
          siteName: "",
          subDomain: "",
          rootDomain: "",
          downloadCode: ""
        },
        rules: {
          siteName: [
            { required: Kooboo.text.validation.required },
            {
              pattern: /^[A-Za-z][\w\-]*$/,
              message: Kooboo.text.validation.siteNameInvalid
            },
            {
              remote: {
                url: Kooboo.Site.isUniqueName(),
                data: function() {
                  return {
                    name: self.model.siteName
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ],
          subDomain: [
            { required: Kooboo.text.validation.required },
            {
              min: 1,
              max: 63,
              message:
                Kooboo.text.validation.minLength +
                1 +
                ", " +
                Kooboo.text.validation.maxLength +
                63
            },
            {
              pattern: /^[A-Za-z][\w\-]*$/,
              message: Kooboo.text.validation.siteNameInvalid
            },
            {
              remote: {
                url: Kooboo.Site.CheckDomainBindingAvailable(),
                data: function() {
                  return {
                    SubDomain: self.model.subDomain,
                    RootDomain: self.model.rootDomain
                  };
                }
              },
              message: Kooboo.text.validation.taken
            }
          ]
        }
      };
    },
    mounted: function() {
      var self = this;
      Kooboo.Domain.getAvailable().then(function(res) {
        if (res.success) {
          self.domains = res.model.map(function(domain) {
            return {
              displayText: "." + domain.domainName,
              value: domain.domainName
            };
          });
          if (self.domains.length) {
            self.model.rootDomain = self.domains[0].value;
          }
        }
      });
    },
    methods: {
      onImport: function() {
        if (this.$refs.form.validate()) {
          Kooboo.Template.Use(this.model).then(function(res) {
            if (res.success) {
              location.href = Kooboo.Route.Get(Kooboo.Route.Site.DetailPage, {
                SiteId: res.model
              });
            }
          });
        }
      }
    },
    watch: {
      value: function(value) {
        this.isShow = value;
      },
      isShow: function(value) {
        this.$emit("input", value);
        if (!value) {
          this.selected = false;
          this.model.siteName = "";
          this.model.subDomain = "";
          this.data = null;
          if (this.$refs.form && this.$refs.form.clearValid) {
            this.$refs.form.clearValid();
          }
        }
      },
      "model.siteName": function(value) {
        this.model.subDomain = value;
      },
      tempData: function(temp) {
        var date = new Date(temp.lastModified),
          size = Kooboo.bytesToSize(temp.size);

        temp.size = size;
        temp.lastModified = date.toDefaultLangString();
        temp.allDynamicCount =
          temp.layoutCount +
          temp.menuCount +
          temp.pageCount +
          temp.viewCount +
          temp.imageCount +
          temp.contentCount;

        this.data = temp;
        this.model.downloadCode = this.data.downloadCode;
      },
      selected: function(value) {
        var self = this;
        if (value) {
          this.$nextTick(function() {
            self.$refs.siteName.focus();
          });
        } else {
          this.model.siteName = "";
          this.model.subDomain = "";
          this.$refs.form.clearValid();
        }
      }
    }
  });
})();
